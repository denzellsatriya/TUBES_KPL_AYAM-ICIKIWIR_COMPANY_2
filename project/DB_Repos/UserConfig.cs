namespace DB_Repos;

public class UserConfigAccount
{
    public int Id { get; set; }
    public string AccountType { get; set; } = string.Empty; // "StaffAccounts" atau "PengunjungAccount"
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Nama { get; set; }
}
public class UserConfigSetting
{
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
}
/// Helper class untuk akses settings yang sudah diketahui
public class PerpusSetting
{
    public int DurasiPinjamHari { get; set; } = 7;
    public decimal DendaPerHari { get; set; } = 2500;
    public decimal DendaBukuHilang { get; set; } = 50000;
    public string NamaPerpustakaan { get; set; } = "Perpustakaan";
}
