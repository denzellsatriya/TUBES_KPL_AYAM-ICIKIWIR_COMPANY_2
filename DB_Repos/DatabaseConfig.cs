using MySql.Data.MySqlClient;

namespace DB_Repos;

/// <summary>
/// Singleton untuk konfigurasi dan pembuatan koneksi database.
/// Catatan: object DatabaseConfig hanya dibuat satu kali, tetapi koneksi MySQL tetap dibuat baru setiap query.
/// Ini lebih aman daripada memakai satu koneksi global yang dipakai bersama-sama.
/// </summary>
public sealed class DatabaseConfig
{
    private static readonly Lazy<DatabaseConfig> _instance = new(() => new DatabaseConfig());

    public static DatabaseConfig Instance => _instance.Value;

    private const string Host = "localhost";
    private const int Port = 3306;
    private const string DatabaseName = "kpl_perpustakaan";
    private const string DbUser = "root";
    private const string DbPassword = "";

    private DatabaseConfig()
    {
    }

    public string ConnectionString =>
        $"Server={Host};Port={Port};Database={DatabaseName};" +
        $"Uid={DbUser};Pwd={DbPassword};CharSet=utf8mb4;";

    /// <summary>
    /// Membuka koneksi baru ke database kpl_perpus.
    /// Caller bertanggung jawab menutup koneksi, gunakan using.
    /// </summary>
    public MySqlConnection GetConnection()
    {
        var conn = new MySqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}
