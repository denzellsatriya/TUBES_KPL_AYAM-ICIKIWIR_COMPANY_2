using DB_Repos;

namespace GUI;

/// <summary>
/// Form untuk role Pengunjung:
/// - Lihat daftar buku (semua / filter status)
/// - Cari buku berdasarkan judul
/// - Pinjam buku (status 0 → 1)
/// - Kembalikan buku (status 1 → 0)
/// - Lapor buku hilang (status → 2)
/// - Lihat riwayat peminjaman pribadi (dari log_buku)
/// </summary>
public partial class FormPerpus : Form
{
    private readonly User _user;
    private readonly BukuRepository _bukuRepo = BukuRepository.Instance;
    private readonly LogBukuRepository _logBukuRepo = LogBukuRepository.Instance;
    private readonly UserConfigRepository _configRepo = UserConfigRepository.Instance;

    private List<Buku> _bukuList = new();

    public FormPerpus(User user)
    {
        _user = user;
        InitializeComponent();

        try
        {
            var s = _configRepo.GetPerpusSetting();
            this.Text = $"{s.NamaPerpustakaan} — Pengunjung ({user.Username})";
            lblInfo.Text = $"Selamat datang, {user.Username}  |  " +
                           $"Durasi pinjam: {s.DurasiPinjamHari} hari  |  " +
                           $"Denda: Rp{s.DendaPerHari:N0}/hari";
        }
        catch
        {
            this.Text = $"Perpustakaan — {user.Username}";
        }

        MuatBuku();
    }

    // ─── Muat / Refresh daftar buku ─────────────────────────────────────
    private void MuatBuku(string keyword = "", int statusFilter = -1)
    {
        try
        {
            _bukuList = string.IsNullOrWhiteSpace(keyword)
                ? _bukuRepo.GetAll()
                : _bukuRepo.Cari(keyword);

            if (statusFilter >= 0)
                _bukuList = _bukuList.Where(b => b.Status == statusFilter).ToList();

            dgvBuku.DataSource = null;
            dgvBuku.DataSource = _bukuList.Select(b => new
            {
                b.Id,
                b.Judul,
                Status = b.StatusLabel,
                TanggalPinjam = b.TanggalPinjam?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                TanggalDibuat = b.TanggalDibuat?.ToString("dd/MM/yyyy") ?? "-",
            }).ToList();

            AturKolomBuku();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Gagal memuat data buku:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AturKolomBuku()
    {
        if (dgvBuku.Columns.Count == 0) return;
        dgvBuku.Columns["Id"].HeaderText = "ID";
        dgvBuku.Columns["Id"].Width = 45;
        dgvBuku.Columns["Judul"].HeaderText = "Judul Buku";
        dgvBuku.Columns["Judul"].Width = 260;
        dgvBuku.Columns["Status"].HeaderText = "Status";
        dgvBuku.Columns["Status"].Width = 90;
        dgvBuku.Columns["TanggalPinjam"].HeaderText = "Tgl. Pinjam";
        dgvBuku.Columns["TanggalPinjam"].Width = 130;
        dgvBuku.Columns["TanggalDibuat"].HeaderText = "Tgl. Masuk";
        dgvBuku.Columns["TanggalDibuat"].Width = 100;
    }

    // ─── Helper: ID buku yang dipilih di grid ───────────────────────────
    private int? IdBukuTerpilih()
    {
        if (dgvBuku.CurrentRow == null) return null;
        return (int)dgvBuku.CurrentRow.Cells["Id"].Value;
    }

    // ─── Events ─────────────────────────────────────────────────────────

    private void btnCari_Click(object sender, EventArgs e)
    {
        int statusFilter = cmbFilter.SelectedIndex - 1; // -1=semua, 0=tersedia, 1=dipinjam, 2=hilang
        MuatBuku(txtCari.Text.Trim(), statusFilter);
    }

    private void txtCari_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter) btnCari_Click(sender, e);
    }

    private void btnPinjam_Click(object sender, EventArgs e)
    {
        int? id = IdBukuTerpilih();
        if (id == null) { MessageBox.Show("Pilih buku terlebih dahulu."); return; }

        var buku = _bukuRepo.GetById(id.Value);
        if (buku == null) return;

        if (buku.Status != 0)
        {
            MessageBox.Show($"Buku '{buku.Judul}' tidak tersedia (status: {buku.StatusLabel}).",
                "Tidak Bisa Dipinjam", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var konfirm = MessageBox.Show(
            $"Pinjam buku:\n\"{buku.Judul}\"?",
            "Konfirmasi Pinjam", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (konfirm != DialogResult.Yes) return;

        try
        {
            _bukuRepo.Pinjam(id.Value, _user.Username);
            MessageBox.Show("Buku berhasil dipinjam!", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            MuatBuku(txtCari.Text.Trim());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Gagal meminjam:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnKembali_Click(object sender, EventArgs e)
    {
        int? id = IdBukuTerpilih();
        if (id == null) { MessageBox.Show("Pilih buku terlebih dahulu."); return; }

        var buku = _bukuRepo.GetById(id.Value);
        if (buku == null) return;

        if (buku.Status != 1)
        {
            MessageBox.Show($"Buku '{buku.Judul}' tidak sedang dipinjam.",
                "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var konfirm = MessageBox.Show(
            $"Kembalikan buku:\n\"{buku.Judul}\"?",
            "Konfirmasi Kembali", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (konfirm != DialogResult.Yes) return;

        try
        {
            _bukuRepo.Kembalikan(id.Value, _user.Username);
            MessageBox.Show("Buku berhasil dikembalikan!", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            MuatBuku(txtCari.Text.Trim());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Gagal mengembalikan:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnLaporHilang_Click(object sender, EventArgs e)
    {
        int? id = IdBukuTerpilih();
        if (id == null) { MessageBox.Show("Pilih buku terlebih dahulu."); return; }

        var buku = _bukuRepo.GetById(id.Value);
        if (buku == null) return;

        var konfirm = MessageBox.Show(
            $"Laporkan buku \"{buku.Judul}\" sebagai HILANG?\nTindakan ini akan mencatat denda buku hilang.",
            "Konfirmasi Lapor Hilang", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (konfirm != DialogResult.Yes) return;

        try
        {
            _bukuRepo.LaporHilang(id.Value, _user.Username);
            MessageBox.Show("Buku telah dilaporkan hilang.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            MuatBuku(txtCari.Text.Trim());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Gagal melaporkan:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnRiwayat_Click(object sender, EventArgs e)
    {
        // Tampilkan riwayat log buku yang dilakukan oleh user ini
        var logs = _logBukuRepo.GetAll()
            .Where(l => l.OlehSiapa == _user.Username)
            .ToList();

        dgvBuku.DataSource = null;
        dgvBuku.DataSource = logs.Select(l => new
        {
            l.Id,
            BukuId = l.BukuId?.ToString() ?? "-",
            Waktu = l.Waktu?.ToString("dd/MM/yyyy HH:mm") ?? "-",
            l.Aksi,
            l.Keterangan,
        }).ToList();

        lblInfo.Text = $"Menampilkan riwayat aktivitas untuk: {_user.Username}";
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
        txtCari.Clear();
        cmbFilter.SelectedIndex = 0;
        MuatBuku();
    }

    private void btnLogout_Click(object sender, EventArgs e)
    {
        var konfirm = MessageBox.Show("Yakin ingin logout?", "Logout",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (konfirm == DialogResult.Yes)
        {
            Application.Restart();
        }
    }
}
