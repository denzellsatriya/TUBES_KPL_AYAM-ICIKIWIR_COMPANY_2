namespace DB_Repos;

public class Buku
{
    public int Id { get; set; }
    public string Judul { get; set; } = string.Empty;

    // 0 = Tersedia, 1 = Dipinjam, 2 = Hilang
    public int Status { get; set; } = 0;

    public DateTime? TanggalPinjam { get; set; }
    public DateTime? TanggalDibuat { get; set; }

    public string StatusLabel => Status switch
    {
        0 => "Tersedia",
        1 => "Dipinjam",
        2 => "Hilang",
        _ => "Tidak Diketahui"
    };
}
