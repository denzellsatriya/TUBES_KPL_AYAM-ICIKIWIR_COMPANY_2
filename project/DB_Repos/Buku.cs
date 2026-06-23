namespace DB_Repos;

public class Buku
{
    public int Id { get; set; }
    public string Judul { get; set; } = string.Empty;
    public int Status { get; set; } = 0;
    public string StatusLabel => Status switch
    {
        0 => "Tersedia",
        1 => "Dipinjam",
        2 => "Hilang",
        _ => "Tidak Diketahui"
    };
    public DateTime? TanggalPinjam { get; set; }
    public DateTime? TanggalDibuat { get; set; }
}
