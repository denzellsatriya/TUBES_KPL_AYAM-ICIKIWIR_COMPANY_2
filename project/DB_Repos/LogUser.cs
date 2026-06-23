namespace DB_Repos;

public class LogUser
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public DateTime? Waktu { get; set; }
    public string Aksi { get; set; } = string.Empty;
    public string? Keterangan { get; set; }
    public string? Message { get; set; }
    public DateTime? Timestamp { get; set; }
}
