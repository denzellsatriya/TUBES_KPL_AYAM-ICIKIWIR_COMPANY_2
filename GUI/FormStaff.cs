using DB_Repos;

namespace GUI;

/// <summary>
/// Form untuk role Staff/Admin:
/// - Tab Buku: lihat semua buku, tambah, edit judul, hapus, pinjam, kembalikan, lapor hilang
/// - Tab User: lihat, tambah, hapus user
/// - Tab Log Buku: lihat riwayat aktivitas buku
/// - Tab Log User: lihat riwayat login/aktivitas user
/// - Tab Pengaturan: ubah nama perpus, durasi pinjam, denda
/// </summary>
public partial class FormStaff : Form
{
    private readonly User _staffUser;
    private readonly BukuRepository _bukuRepo = BukuRepository.Instance;
    private readonly UserRepository _userRepo = UserRepository.Instance;
    private readonly LogBukuRepository _logBukuRepo = LogBukuRepository.Instance;
    private readonly LogUserRepository _logUserRepo = LogUserRepository.Instance;
    private readonly UserConfigRepository _configRepo = UserConfigRepository.Instance;

    public FormStaff(User user)
    {
        _staffUser = user;
        InitializeComponent();

        try
        {
            var s = _configRepo.GetPerpusSetting();
            this.Text = $"{s.NamaPerpustakaan} — Staff ({user.Username})";
            IsiFormulirSetting(s);
        }
        catch { }

        MuatBuku();
        MuatUser();
        MuatLogBuku();
        MuatLogUser();
    }

    // ════════════════════════════════════════════════════════════════════
    // TAB BUKU
    // ════════════════════════════════════════════════════════════════════

    private void MuatBuku(string keyword = "")
    {
        try
        {
            var list = string.IsNullOrWhiteSpace(keyword)
                ? _bukuRepo.GetAll()
                : _bukuRepo.Cari(keyword);

            dgvBuku.DataSource = null;
            dgvBuku.DataSource = list.Select(b => new
            {
                b.Id,
                b.Judul,
                Status = b.StatusLabel,
                TanggalPinjam = b.TanggalPinjam?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                TanggalDibuat = b.TanggalDibuat?.ToString("dd/MM/yyyy") ?? "-",
            }).ToList();

            AturKolom(dgvBuku, new[] { ("Id", 45), ("Judul", 280), ("Status", 85), ("TanggalPinjam", 130), ("TanggalDibuat", 100) });
        }
        catch (Exception ex) { TampilError(ex); }
    }

    private void btnTambahBuku_Click(object sender, EventArgs e)
    {
        string judul = txtJudulBaru.Text.Trim();
        if (string.IsNullOrEmpty(judul))
        {
            MessageBox.Show("Masukkan judul buku.", "Peringatan",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            int newId = _bukuRepo.Tambah(judul, _staffUser.Username);
            MessageBox.Show($"Buku '{judul}' berhasil ditambahkan (ID: {newId}).",
                "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtJudulBaru.Clear();
            MuatBuku();
            MuatLogBuku();
        }
        catch (Exception ex) { TampilError(ex); }
    }

    private void btnEditJudul_Click(object sender, EventArgs e)
    {
        int? id = IdDariGrid(dgvBuku);
        if (id == null) return;

        string judulLama = dgvBuku.CurrentRow!.Cells["Judul"].Value?.ToString() ?? "";
        string judulBaru = Microsoft.VisualBasic.Interaction.InputBox(
            "Masukkan judul baru:", "Edit Judul Buku", judulLama);

        if (string.IsNullOrWhiteSpace(judulBaru) || judulBaru == judulLama) return;

        try
        {
            _bukuRepo.UpdateJudul(id.Value, judulBaru.Trim(), _staffUser.Username);
            MessageBox.Show("Judul berhasil diubah.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            MuatBuku();
        }
        catch (Exception ex) { TampilError(ex); }
    }

    private void btnHapusBuku_Click(object sender, EventArgs e)
    {
        int? id = IdDariGrid(dgvBuku);
        if (id == null) return;

        string judul = dgvBuku.CurrentRow!.Cells["Judul"].Value?.ToString() ?? "";
        var konfirm = MessageBox.Show(
            $"Hapus buku \"{judul}\" beserta semua log-nya?\nTindakan ini tidak dapat dibatalkan.",
            "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (konfirm != DialogResult.Yes) return;

        try
        {
            _bukuRepo.Hapus(id.Value, _staffUser.Username);
            MessageBox.Show("Buku berhasil dihapus.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            MuatBuku();
            MuatLogBuku();
        }
        catch (Exception ex) { TampilError(ex); }
    }

    private void btnStaffKembali_Click(object sender, EventArgs e)
    {
        int? id = IdDariGrid(dgvBuku);
        if (id == null) return;
        try
        {
            _bukuRepo.Kembalikan(id.Value, _staffUser.Username);
            MessageBox.Show("Buku berhasil dikembalikan.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            MuatBuku(); MuatLogBuku();
        }
        catch (Exception ex) { TampilError(ex); }
    }

    private void btnStaffHilang_Click(object sender, EventArgs e)
    {
        int? id = IdDariGrid(dgvBuku);
        if (id == null) return;
        try
        {
            _bukuRepo.LaporHilang(id.Value, _staffUser.Username);
            MessageBox.Show("Buku dilaporkan hilang.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            MuatBuku(); MuatLogBuku();
        }
        catch (Exception ex) { TampilError(ex); }
    }

    private void btnStaffRestock_Click(object sender, EventArgs e)
    {
        int? id = IdDariGrid(dgvBuku);
        if (id == null) return;
        try
        {
            _bukuRepo.Restock(id.Value, _staffUser.Username);
            MessageBox.Show("Buku berhasil di restock.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            MuatBuku(); MuatLogBuku();
        }
        catch (Exception ex) { TampilError(ex); }
    }

    private void btnCariBuku_Click(object sender, EventArgs e)
        => MuatBuku(txtCariBuku.Text.Trim());

    private void txtCariBuku_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter) MuatBuku(txtCariBuku.Text.Trim());
    }

    // ════════════════════════════════════════════════════════════════════
    // TAB USER
    // ════════════════════════════════════════════════════════════════════

    private void MuatUser()
    {
        try
        {
            var list = _userRepo.GetAll();
            dgvUser.DataSource = null;
            dgvUser.DataSource = list.Select(u => new
            {
                u.Username,
                u.Role,
                u.NomorIdentitas,
                u.Email,
                TanggalDibuat = u.TanggalDibuat?.ToString("dd/MM/yyyy") ?? "-",
            }).ToList();

            AturKolom(dgvUser, new[]
            {
                ("Username",110),("Role",90),("NomorIdentitas",130),
                ("Email",180),("TanggalDibuat",100)
            });
        }
        catch (Exception ex) { TampilError(ex); }
    }

    private void btnTambahUser_Click(object sender, EventArgs e)
    {
        string username = txtUsernameBaru.Text.Trim();
        string password = txtPasswordBaru.Text.Trim();
        string role = cmbRoleBaru.SelectedItem?.ToString() ?? "Pengunjung";
        string nim = txtNimBaru.Text.Trim();
        string email = txtEmailBaru.Text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Username dan password tidak boleh kosong.", "Peringatan",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var user = new User
            {
                Username = username,
                Password = password,
                Role = role,
                NomorIdentitas = string.IsNullOrEmpty(nim) ? null : nim,
                Email = string.IsNullOrEmpty(email) ? null : email,
            };
            _userRepo.Tambah(user, _staffUser.Username);
            MessageBox.Show($"User '{username}' berhasil didaftarkan.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtUsernameBaru.Clear(); txtPasswordBaru.Clear();
            txtNimBaru.Clear(); txtEmailBaru.Clear();
            MuatUser();
        }
        catch (Exception ex) { TampilError(ex); }
    }

    private void btnHapusUser_Click(object sender, EventArgs e)
    {
        if (dgvUser.CurrentRow == null)
        {
            MessageBox.Show("Pilih user terlebih dahulu.");
            return;
        }
        string username = dgvUser.CurrentRow.Cells["Username"].Value?.ToString() ?? "";
        if (username == _staffUser.Username)
        {
            MessageBox.Show("Tidak bisa menghapus akun sendiri.", "Peringatan",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var konfirm = MessageBox.Show($"Hapus user '{username}'?",
            "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (konfirm != DialogResult.Yes) return;

        try
        {
            _userRepo.Hapus(username, _staffUser.Username);
            MessageBox.Show("User berhasil dihapus.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            MuatUser();
        }
        catch (Exception ex) { TampilError(ex); }
    }

    // ════════════════════════════════════════════════════════════════════
    // TAB LOG
    // ════════════════════════════════════════════════════════════════════

    private void MuatLogBuku()
    {
        try
        {
            var logs = _logBukuRepo.GetAll();
            dgvLogBuku.DataSource = null;
            dgvLogBuku.DataSource = logs.Select(l => new
            {
                l.Id,
                BukuId = l.BukuId?.ToString() ?? "-",
                Waktu = l.Waktu?.ToString("dd/MM/yyyy HH:mm:ss") ?? "-",
                l.Aksi,
                OlehSiapa = l.OlehSiapa ?? "-",
                l.Keterangan,
            }).ToList();

            AturKolom(dgvLogBuku, new[]
            {
                ("Id",45),("BukuId",55),("Waktu",135),
                ("Aksi",110),("OlehSiapa",100),("Keterangan",300)
            });
        }
        catch (Exception ex) { TampilError(ex); }
    }

    private void MuatLogUser()
    {
        try
        {
            var logs = _logUserRepo.GetAll();
            dgvLogUser.DataSource = null;
            dgvLogUser.DataSource = logs.Select(l => new
            {
                l.Id,
                Username = l.Username ?? "-",
                Waktu = l.Waktu?.ToString("dd/MM/yyyy HH:mm:ss") ?? "-",
                l.Aksi,
                l.Keterangan,
            }).ToList();

            AturKolom(dgvLogUser, new[]
            {
                ("Id",45),("Username",110),("Waktu",135),
                ("Aksi",100),("Keterangan",350)
            });
        }
        catch (Exception ex) { TampilError(ex); }
    }

    private void btnRefreshLogBuku_Click(object sender, EventArgs e) => MuatLogBuku();
    private void btnRefreshLogUser_Click(object sender, EventArgs e) => MuatLogUser();

    // ════════════════════════════════════════════════════════════════════
    // TAB PENGATURAN
    // ════════════════════════════════════════════════════════════════════

    private void IsiFormulirSetting(PerpusSetting s)
    {
        txtNamaPerpus.Text = s.NamaPerpustakaan;
        numDurasi.Value = s.DurasiPinjamHari;
        numDendaHari.Value = s.DendaPerHari;
        numDendaHilang.Value = s.DendaBukuHilang;
    }

    private void btnSimpanSetting_Click(object sender, EventArgs e)
    {
        try
        {
            var s = new PerpusSetting
            {
                NamaPerpustakaan = txtNamaPerpus.Text.Trim(),
                DurasiPinjamHari = (int)numDurasi.Value,
                DendaPerHari = numDendaHari.Value,
                DendaBukuHilang = numDendaHilang.Value,
            };

            if (string.IsNullOrEmpty(s.NamaPerpustakaan))
            {
                MessageBox.Show("Nama perpustakaan tidak boleh kosong.");
                return;
            }

            _configRepo.SavePerpusSetting(s);
            this.Text = $"{s.NamaPerpustakaan} — Staff ({_staffUser.Username})";
            MessageBox.Show("Pengaturan berhasil disimpan.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { TampilError(ex); }
    }

    // ════════════════════════════════════════════════════════════════════
    // HELPERS
    // ════════════════════════════════════════════════════════════════════

    private static int? IdDariGrid(DataGridView dgv)
    {
        if (dgv.CurrentRow == null)
        {
            MessageBox.Show("Pilih baris terlebih dahulu.", "Peringatan",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }
        return Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
    }

    private static void AturKolom(DataGridView dgv, (string name, int width)[] cols)
    {
        foreach (var (name, width) in cols)
            if (dgv.Columns.Contains(name))
                dgv.Columns[name].Width = width;
    }

    private static void TampilError(Exception ex)
        => MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

    private void btnLogout_Click(object sender, EventArgs e)
    {
        var konfirm = MessageBox.Show("Yakin ingin logout?", "Logout",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (konfirm == DialogResult.Yes) Application.Restart();
    }
}
