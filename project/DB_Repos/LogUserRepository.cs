using MySql.Data.MySqlClient;

namespace DB_Repos;

public sealed class LogUserRepository
{
    private static readonly Lazy<LogUserRepository> _instance = new(() => new LogUserRepository());

    public static LogUserRepository Instance => _instance.Value;

    private LogUserRepository()
    {
    }

    /// <summary>Semua log user, terbaru di atas.</summary>
    public List<LogUser> GetAll()
    {
        var list = new List<LogUser>();
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id, username, waktu, aksi, keterangan, message, timestamp " +
            "FROM log_user ORDER BY id DESC", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(MapRow(r));
        return list;
    }

    /// <summary>Log aktivitas untuk user tertentu.</summary>
    public List<LogUser> GetByUsername(string username)
    {
        var list = new List<LogUser>();
        using var conn = DatabaseConfig.Instance.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id, username, waktu, aksi, keterangan, message, timestamp " +
            "FROM log_user WHERE username = @u ORDER BY id DESC", conn);
        cmd.Parameters.AddWithValue("@u", username);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(MapRow(r));
        return list;
    }

    private static LogUser MapRow(MySqlDataReader r) => new()
    {
        Id = r.GetInt32("id"),
        Username = r.IsDBNull(r.GetOrdinal("username")) ? null : r.GetString("username"),
        Waktu = r.IsDBNull(r.GetOrdinal("waktu")) ? null : r.GetDateTime("waktu"),
        Aksi = r.GetString("aksi"),
        Keterangan = r.IsDBNull(r.GetOrdinal("keterangan")) ? null : r.GetString("keterangan"),
        Message = r.IsDBNull(r.GetOrdinal("message")) ? null : r.GetString("message"),
        Timestamp = r.IsDBNull(r.GetOrdinal("timestamp")) ? null : r.GetDateTime("timestamp"),
    };
}
