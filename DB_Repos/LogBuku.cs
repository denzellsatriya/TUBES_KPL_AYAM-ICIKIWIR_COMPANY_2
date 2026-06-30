namespace DB_Repos;

public class LogBuku
{
    public int Id { get; set; }
    public int? BukuId { get; set; }
    public DateTime? Waktu { get; set; }
    public string Aksi { get; set; } = string.Empty;
    public string? OlehSiapa { get; set; }
    public string? Keterangan { get; set; }
    public string? Message { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? SumberFile { get; set; }
}
