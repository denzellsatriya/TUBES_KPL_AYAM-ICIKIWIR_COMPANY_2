namespace DB_Repos;

public class User
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? NomorIdentitas { get; set; }
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty; // "Staff" atau "Pengunjung"
    public DateTime? TanggalDibuat { get; set; }
}
