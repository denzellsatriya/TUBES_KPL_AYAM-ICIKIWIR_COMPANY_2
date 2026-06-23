using MySql.Data.MySqlClient;

namespace DB_Repos;

public class UserConfigRepository
{
    // SETTINGS
    /// Ambil semua settings sebagai dictionary key→value.
    public Dictionary<string, string> GetAllSettings()
    {
        var dict = new Dictionary<string, string>();
        using var conn = DatabaseConfig.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT setting_key, setting_value FROM user_config_settings", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            dict[r.GetString("setting_key")] = r.GetString("setting_value");
        return dict;
    }
    /// Baca PerpusSetting dari tabel user_config_settings.
    public PerpusSetting GetPerpusSetting()
    {
        var dict = GetAllSettings();
        return new PerpusSetting
        {
            DurasiPinjamHari = dict.TryGetValue("DurasiPinjamHari", out var d) ? int.Parse(d) : 7,
            DendaPerHari = dict.TryGetValue("DendaPerHari", out var dph) ? decimal.Parse(dph) : 2500,
            DendaBukuHilang = dict.TryGetValue("DendaBukuHilang", out var dbh) ? decimal.Parse(dbh) : 50000,
            NamaPerpustakaan = dict.TryGetValue("NamaPerpustakaan", out var np) ? np : "Perpustakaan",
        };
    }
    /// Simpan satu setting (INSERT … ON DUPLICATE KEY UPDATE).
    public void SetSetting(string key, string value)
    {
        using var conn = DatabaseConfig.GetConnection();
        using var cmd = new MySqlCommand(
            "INSERT INTO user_config_settings (setting_key, setting_value) VALUES (@k, @v) " +
            "ON DUPLICATE KEY UPDATE setting_value = @v", conn);
        cmd.Parameters.AddWithValue("@k", key);
        cmd.Parameters.AddWithValue("@v", value);
        cmd.ExecuteNonQuery();
    }
    /// Simpan seluruh PerpusSetting sekaligus.
    public void SavePerpusSetting(PerpusSetting s)
    {
        SetSetting("DurasiPinjamHari", s.DurasiPinjamHari.ToString());
        SetSetting("DendaPerHari", s.DendaPerHari.ToString());
        SetSetting("DendaBukuHilang", s.DendaBukuHilang.ToString());
        SetSetting("NamaPerpustakaan", s.NamaPerpustakaan);
    }
    // CONFIG ACCOUNTS
    public List<UserConfigAccount> GetAllAccounts()
    {
        var list = new List<UserConfigAccount>();
        using var conn = DatabaseConfig.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id, account_type, username, password, role, nama " +
            "FROM user_config_accounts ORDER BY id", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new UserConfigAccount
            {
                Id = r.GetInt32("id"),
                AccountType = r.GetString("account_type"),
                Username = r.GetString("username"),
                Password = r.GetString("password"),
                Role = r.GetString("role"),
                Nama = r.IsDBNull(r.GetOrdinal("nama")) ? null : r.GetString("nama"),
            });
        return list;
    }
    public void TambahAccount(UserConfigAccount acc)
    {
        using var conn = DatabaseConfig.GetConnection();
        using var cmd = new MySqlCommand(
            "INSERT INTO user_config_accounts (account_type, username, password, role, nama) " +
            "VALUES (@at, @u, @p, @r, @n)", conn);
        cmd.Parameters.AddWithValue("@at", acc.AccountType);
        cmd.Parameters.AddWithValue("@u", acc.Username);
        cmd.Parameters.AddWithValue("@p", acc.Password);
        cmd.Parameters.AddWithValue("@r", acc.Role);
        cmd.Parameters.AddWithValue("@n", (object?)acc.Nama ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }
    public void HapusAccount(int id)
    {
        using var conn = DatabaseConfig.GetConnection();
        using var cmd = new MySqlCommand(
            "DELETE FROM user_config_accounts WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}
