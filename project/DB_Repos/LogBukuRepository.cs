using MySql.Data.MySqlClient;

namespace DB_Repos;

public class LogBukuRepository
{
    /// Semua log buku, terbaru di atas.
    public List<LogBuku> GetAll()
    {
        var list = new List<LogBuku>();
        using var conn = DatabaseConfig.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id, buku_id, waktu, aksi, oleh_siapa, keterangan, message, timestamp, sumber_file " +
            "FROM log_buku ORDER BY id DESC", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(MapRow(r));
        return list;
    }
    /// Log untuk satu buku tertentu.
    public List<LogBuku> GetByBukuId(int bukuId)
    {
        var list = new List<LogBuku>();
        using var conn = DatabaseConfig.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id, buku_id, waktu, aksi, oleh_siapa, keterangan, message, timestamp, sumber_file " +
            "FROM log_buku WHERE buku_id = @bid ORDER BY id DESC", conn);
        cmd.Parameters.AddWithValue("@bid", bukuId);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(MapRow(r));
        return list;
    }
    /// Log berdasarkan jenis aksi (DITAMBAHKAN, PINJAM, KEMBALI, LAPOR_HILANG, dst).
    public List<LogBuku> GetByAksi(string aksi)
    {
        var list = new List<LogBuku>();
        using var conn = DatabaseConfig.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id, buku_id, waktu, aksi, oleh_siapa, keterangan, message, timestamp, sumber_file " +
            "FROM log_buku WHERE aksi = @aksi ORDER BY id DESC", conn);
        cmd.Parameters.AddWithValue("@aksi", aksi);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(MapRow(r));
        return list;
    }
    private static LogBuku MapRow(MySqlDataReader r) => new()
    {
        Id = r.GetInt32("id"),
        BukuId = r.IsDBNull(r.GetOrdinal("buku_id")) ? null : r.GetInt32("buku_id"),
        Waktu = r.IsDBNull(r.GetOrdinal("waktu")) ? null : r.GetDateTime("waktu"),
        Aksi = r.GetString("aksi"),
        OlehSiapa = r.IsDBNull(r.GetOrdinal("oleh_siapa")) ? null : r.GetString("oleh_siapa"),
        Keterangan = r.IsDBNull(r.GetOrdinal("keterangan")) ? null : r.GetString("keterangan"),
        Message = r.IsDBNull(r.GetOrdinal("message")) ? null : r.GetString("message"),
        Timestamp = r.IsDBNull(r.GetOrdinal("timestamp")) ? null : r.GetDateTime("timestamp"),
        SumberFile = r.IsDBNull(r.GetOrdinal("sumber_file")) ? null : r.GetString("sumber_file"),
    };
}
