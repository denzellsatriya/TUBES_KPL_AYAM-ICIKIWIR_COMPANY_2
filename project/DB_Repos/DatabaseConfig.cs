using MySql.Data.MySqlClient;

namespace DB_Repos;

public static class DatabaseConfig
{
    // Sesuaikan host, port, user, dan password dengan setup MySQL lokal Anda
    private const string Host = "localhost";
    private const int Port = 3306;
    private const string DatabaseName = "kpl_perpus";
    private const string DbUser = "root";
    private const string DbPassword = "";

    public static string ConnectionString =>
        $"Server={Host};Port={Port};Database={DatabaseName};" +
        $"Uid={DbUser};Pwd={DbPassword};CharSet=utf8mb4;";

    /// Membuka koneksi baru ke database kpl_perpus.
    /// Caller bertanggung jawab menutup koneksi (gunakan 'using').
    public static MySqlConnection GetConnection()
    {
        var conn = new MySqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}
