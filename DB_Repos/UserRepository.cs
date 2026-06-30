using MySql.Data.MySqlClient;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace DB_Repos;

public sealed class UserRepository
{
    private static readonly Lazy<UserRepository> _instance = new(() => new UserRepository());

    public static UserRepository Instance => _instance.Value;

    private UserRepository()
    {
    }

    // ─────────────────────────────────────────────
    // AUTH
    // ─────────────────────────────────────────────

    /// <summary>
    /// Login: cocokkan username+password dari tabel users.
    /// Mengembalikan objek User jika berhasil, null jika gagal.
    /// Otomatis mencatat log LOGIN.
    /// </summary>
    public User? Login(string username, string password)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT username, password, nomor_identitas, email, role, tanggal_dibuat " +
            "FROM users WHERE username = @u", conn);
        cmd.Parameters.AddWithValue("@u", username);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        var user = MapRow(r);
        // Verifikasi password input terhadap hash di database
        if (!PasswordHelper.VerifyPassword(password, user.Password))
            return null; // password salah
        r.Close();

        // Catat log login
        CatatLogUser(conn, null, username, "LOGIN", $"Login sebagai {user.Role}", DateTime.Now);
        return user;
    }

    // ─────────────────────────────────────────────
    // READ
    // ─────────────────────────────────────────────

    public List<User> GetAll()
    {
        var list = new List<User>();
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT username, password, nomor_identitas, email, role, tanggal_dibuat " +
            "FROM users ORDER BY username", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(MapRow(r));
        return list;
    }

    public User? GetByUsername(string username)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT username, password, nomor_identitas, email, role, tanggal_dibuat " +
            "FROM users WHERE username = @u", conn);
        cmd.Parameters.AddWithValue("@u", username);
        using var r = cmd.ExecuteReader();
        return r.Read() ? MapRow(r) : null;
    }

    public List<User> GetByRole(string role)
    {
        var list = new List<User>();
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT username, password, nomor_identitas, email, role, tanggal_dibuat " +
            "FROM users WHERE role = @role ORDER BY username", conn);
        cmd.Parameters.AddWithValue("@role", role);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(MapRow(r));
        return list;
    }

    // ─────────────────────────────────────────────
    // CREATE
    // ─────────────────────────────────────────────

    /// <summary>Daftarkan user baru. Throws jika username sudah ada.</summary>
    public void Tambah(User user, string olehSiapa)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            // Cek duplikat
            using var chk = new MySqlCommand(
                "SELECT COUNT(*) FROM users WHERE username = @u", conn, tx);
            chk.Parameters.AddWithValue("@u", user.Username);
            if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                throw new Exception($"Username '{user.Username}' sudah digunakan.");
            if(user.Username.Length > 20)
                throw new Exception($"Username '{user.Username}' terlalu panjang. Maksimal 20 karakter.");
            if(user.Password.Length < 5)
                throw new Exception($"Password terlalu pendek. Minimal 5 karakter.");
            if (user.NomorIdentitas != null && Regex.IsMatch(user.NomorIdentitas, @"^\D") == true)
                throw new Exception($"Nomor identitas salah. Hanya  bisa mengandung angka.");
            if (string.IsNullOrEmpty(user.Email) == false && Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") == false)
                throw new Exception($"Email '{user.Email}' tidak valid.");

            using var cmd = new MySqlCommand(
                "INSERT INTO users (username, password, nomor_identitas, email, role, tanggal_dibuat) " +
                "VALUES (@u, @p, @ni, @e, @r, @tgl)", conn, tx);
            cmd.Parameters.AddWithValue("@u", user.Username);
            string hashedPassword = PasswordHelper.HashPassword(user.Password); // hash dulu!
            cmd.Parameters.AddWithValue("@p", hashedPassword);
            cmd.Parameters.AddWithValue("@ni", (object?)user.NomorIdentitas ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@e", (object?)user.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@r", user.Role);
            var now = DateTime.Now;
            cmd.Parameters.AddWithValue("@tgl", now);
            cmd.ExecuteNonQuery();

            CatatLogUser(conn, tx, user.Username, "DAFTAR",
                $"User '{user.Username}' didaftarkan oleh {olehSiapa}", now);

            tx.Commit();
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

    /// <summary>Update data user (kecuali username yang merupakan PK).</summary>
    public void Update(User user, string olehSiapa)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            using var cmd = new MySqlCommand(
                "UPDATE users SET password=@p, nomor_identitas=@ni, email=@e, role=@r " +
                "WHERE username=@u", conn, tx);
            string hashedPassword = PasswordHelper.HashPassword(user.Password);
            cmd.Parameters.AddWithValue("@p", hashedPassword);
            cmd.Parameters.AddWithValue("@ni", (object?)user.NomorIdentitas ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@e", (object?)user.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@r", user.Role);
            cmd.Parameters.AddWithValue("@u", user.Username);
            cmd.ExecuteNonQuery();

            CatatLogUser(conn, tx, user.Username, "DIUBAH",
                $"Data user '{user.Username}' diubah oleh {olehSiapa}", DateTime.Now);

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

    public void Hapus(string username, string olehSiapa)
    {
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            // Log dulu sebelum hapus (FK ON DELETE SET NULL di log_user)
            CatatLogUser(conn, tx, username, "DIHAPUS",
                $"User '{username}' dihapus oleh {olehSiapa}", DateTime.Now);

            using var cmd = new MySqlCommand(
                "DELETE FROM users WHERE username = @u", conn, tx);
            cmd.Parameters.AddWithValue("@u", username);
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

    private static User MapRow(MySqlDataReader r) => new()
    {
        Username = r.GetString("username"),
        Password = r.GetString("password"),
        NomorIdentitas = r.IsDBNull(r.GetOrdinal("nomor_identitas")) ? null : r.GetString("nomor_identitas"),
        Email = r.IsDBNull(r.GetOrdinal("email")) ? null : r.GetString("email"),
        Role = r.GetString("role"),
        TanggalDibuat = r.IsDBNull(r.GetOrdinal("tanggal_dibuat")) ? null : r.GetDateTime("tanggal_dibuat"),
    };

    internal static void CatatLogUser(
        MySqlConnection conn, MySqlTransaction? tx,
        string username, string aksi, string keterangan, DateTime waktu)
    {
        using var cmd = tx != null
            ? new MySqlCommand(
                "INSERT INTO log_user (username, waktu, aksi, keterangan) VALUES (@u, @w, @a, @k)",
                conn, tx)
            : new MySqlCommand(
                "INSERT INTO log_user (username, waktu, aksi, keterangan) VALUES (@u, @w, @a, @k)",
                conn);
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@w", waktu);
        cmd.Parameters.AddWithValue("@a", aksi);
        cmd.Parameters.AddWithValue("@k", keterangan);
        cmd.ExecuteNonQuery();
    }
}
