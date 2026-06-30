using MySql.Data.MySqlClient;

namespace DB_Repos;

public sealed class BukuRepository
{
    private static readonly Lazy<BukuRepository> _instance = new(() => new BukuRepository());

    public static BukuRepository Instance => _instance.Value;

    private BukuRepository()
    {
    }

    // ─────────────────────────────────────────────
    // READ
    // ─────────────────────────────────────────────

    /// <summary>Ambil semua buku.</summary>
    public List<Buku> GetAll()
    {
        var list = new List<Buku>();
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id, judul, status, tanggal_pinjam, tanggal_dibuat FROM buku ORDER BY id", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapRow(reader));
        return list;
    }

    /// <summary>Cari buku berdasarkan ID.</summary>
    public Buku? GetById(int id)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id, judul, status, tanggal_pinjam, tanggal_dibuat FROM buku WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapRow(reader) : null;
    }

    /// <summary>Cari buku berdasarkan kata kunci judul (LIKE %keyword%).</summary>
    public List<Buku> Cari(string keyword)
    {
        var list = new List<Buku>();
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id, judul, status, tanggal_pinjam, tanggal_dibuat FROM buku " +
            "WHERE judul LIKE @kw ORDER BY id", conn);
        cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapRow(reader));
        return list;
    }

    /// <summary>Ambil buku berdasarkan status (0=Tersedia, 1=Dipinjam, 2=Hilang).</summary>
    public List<Buku> GetByStatus(int status)
    {
        var list = new List<Buku>();
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id, judul, status, tanggal_pinjam, tanggal_dibuat FROM buku " +
            "WHERE status = @status ORDER BY id", conn);
        cmd.Parameters.AddWithValue("@status", status);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapRow(reader));
        return list;
    }

    // ─────────────────────────────────────────────
    // CREATE
    // ─────────────────────────────────────────────

    /// <summary>
    /// Tambah buku baru dan catat log DITAMBAHKAN.
    /// Mengembalikan ID buku yang baru dibuat.
    /// </summary>
    public int Tambah(string judul, string olehSiapa)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            // Dapatkan ID berikutnya
            using var cmdMaxId = new MySqlCommand("SELECT COALESCE(MAX(id), 0) + 1 FROM buku", conn, tx);
            int newId = Convert.ToInt32(cmdMaxId.ExecuteScalar());

            var now = DateTime.Now;

            using var cmd = new MySqlCommand(
                "INSERT INTO buku (id, judul, status, tanggal_pinjam, tanggal_dibuat) " +
                "VALUES (@id, @judul, 0, NULL, @tgl)", conn, tx);
            cmd.Parameters.AddWithValue("@id", newId);
            cmd.Parameters.AddWithValue("@judul", judul);
            cmd.Parameters.AddWithValue("@tgl", now);
            cmd.ExecuteNonQuery();

            // Log
            CatatLogBuku(conn, tx, newId, "DITAMBAHKAN", olehSiapa,
                $"Buku '{judul}' ditambahkan oleh {olehSiapa}", now);

            tx.Commit();
            return newId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // ─────────────────────────────────────────────
    // UPDATE
    // ─────────────────────────────────────────────

    /// <summary>Update judul buku dan catat log DIUBAH.</summary>
    public void UpdateJudul(int id, string judulBaru, string olehSiapa)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            using var cmd = new MySqlCommand(
                "UPDATE buku SET judul = @judul WHERE id = @id", conn, tx);
            cmd.Parameters.AddWithValue("@judul", judulBaru);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            CatatLogBuku(conn, tx, id, "DIUBAH", olehSiapa,
                $"Judul buku ID {id} diubah menjadi '{judulBaru}'", DateTime.Now);

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Pinjam buku: set status=1, tanggal_pinjam=sekarang, catat log PINJAM.
    /// </summary>
    public void Pinjam(int bukuId, string olehSiapa)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            var buku = GetById(bukuId) ?? throw new Exception($"Buku ID {bukuId} tidak ditemukan.");
            if (buku.Status != 0)
                throw new Exception($"Buku '{buku.Judul}' tidak tersedia untuk dipinjam (status: {buku.StatusLabel}).");

            var now = DateTime.Now;
            using var cmd = new MySqlCommand(
                "UPDATE buku SET status = 1, tanggal_pinjam = @tgl WHERE id = @id", conn, tx);
            cmd.Parameters.AddWithValue("@tgl", now);
            cmd.Parameters.AddWithValue("@id", bukuId);
            cmd.ExecuteNonQuery();

            CatatLogBuku(conn, tx, bukuId, "PINJAM", olehSiapa,
                $"Buku '{buku.Judul}' dipinjam oleh {olehSiapa}", now);

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Kembalikan buku: set status=0, tanggal_pinjam=NULL, catat log KEMBALI.
    /// </summary>
    public void Kembalikan(int bukuId, string olehSiapa)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            var buku = GetById(bukuId) ?? throw new Exception($"Buku ID {bukuId} tidak ditemukan.");
            if (buku.Status != 1)
                throw new Exception($"Buku '{buku.Judul}' tidak sedang dipinjam.");

            var now = DateTime.Now;
            using var cmd = new MySqlCommand(
                "UPDATE buku SET status = 0, tanggal_pinjam = NULL WHERE id = @id", conn, tx);
            cmd.Parameters.AddWithValue("@id", bukuId);
            cmd.ExecuteNonQuery();

            CatatLogBuku(conn, tx, bukuId, "KEMBALI", olehSiapa,
                $"Buku '{buku.Judul}' dikembalikan oleh {olehSiapa}", now);

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Laporkan buku hilang: set status=2, catat log LAPOR_HILANG.
    /// </summary>
    public void LaporHilang(int bukuId, string olehSiapa)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            var buku = GetById(bukuId) ?? throw new Exception($"Buku ID {bukuId} tidak ditemukan.");
            if (buku.Status == 2)
                throw new Exception($"Buku '{buku.Judul}' sudah dilaporkan hilang.");

            var now = DateTime.Now;
            using var cmd = new MySqlCommand(
                "UPDATE buku SET status = 2 WHERE id = @id", conn, tx);
            cmd.Parameters.AddWithValue("@id", bukuId);
            cmd.ExecuteNonQuery();

            CatatLogBuku(conn, tx, bukuId, "LAPOR_HILANG", olehSiapa,
                $"Buku '{buku.Judul}' dilaporkan hilang", now);

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public void Restock(int bukuId, string olehSiapa)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            var buku = GetById(bukuId) ?? throw new Exception($"Buku ID {bukuId} tidak ditemukan.");
            if (buku.Status != 2)
                throw new Exception($"Buku '{buku.Judul}' tidak dilaporkan hilang.");

            var now = DateTime.Now;
            using var cmd = new MySqlCommand(
                "UPDATE buku SET status = 0 WHERE id = @id", conn, tx);
            cmd.Parameters.AddWithValue("@id", bukuId);
            cmd.ExecuteNonQuery();

            CatatLogBuku(conn, tx, bukuId, "RESTOCK", olehSiapa,
                $"Buku '{buku.Judul}' berhasil di restock", now);

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
    // ─────────────────────────────────────────────
    // DELETE
    // ─────────────────────────────────────────────

    /// <summary>Hapus buku dan semua log terkait, catat log DIHAPUS sebelum hapus.</summary>
    public void Hapus(int id, string olehSiapa)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            var buku = GetById(id) ?? throw new Exception($"Buku ID {id} tidak ditemukan.");

            // Hapus log terkait terlebih dahulu (FK ON DELETE SET NULL, tapi kita eksplisit)
            using var cmdLog = new MySqlCommand(
                "DELETE FROM log_buku WHERE buku_id = @id", conn, tx);
            cmdLog.Parameters.AddWithValue("@id", id);
            cmdLog.ExecuteNonQuery();

            using var cmd = new MySqlCommand("DELETE FROM buku WHERE id = @id", conn, tx);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // ─────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────

    private static Buku MapRow(MySqlDataReader r) => new()
    {
        Id = r.GetInt32("id"),
        Judul = r.GetString("judul"),
        Status = r.GetInt32("status"),
        TanggalPinjam = r.IsDBNull(r.GetOrdinal("tanggal_pinjam"))
            ? null : r.GetDateTime("tanggal_pinjam"),
        TanggalDibuat = r.IsDBNull(r.GetOrdinal("tanggal_dibuat"))
            ? null : r.GetDateTime("tanggal_dibuat"),
    };

    private static void CatatLogBuku(
        MySqlConnection conn, MySqlTransaction tx,
        int bukuId, string aksi, string olehSiapa, string keterangan, DateTime waktu)
    {
        using var cmd = new MySqlCommand(
            "INSERT INTO log_buku (buku_id, waktu, aksi, oleh_siapa, keterangan) " +
            "VALUES (@bid, @waktu, @aksi, @oleh, @ket)", conn, tx);
        cmd.Parameters.AddWithValue("@bid", bukuId);
        cmd.Parameters.AddWithValue("@waktu", waktu);
        cmd.Parameters.AddWithValue("@aksi", aksi);
        cmd.Parameters.AddWithValue("@oleh", olehSiapa);
        cmd.Parameters.AddWithValue("@ket", keterangan);
        cmd.ExecuteNonQuery();
    }
}
