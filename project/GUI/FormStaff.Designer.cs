namespace GUI;

partial class FormStaff
{
    private System.ComponentModel.IContainer components = null;

    // ─── Tab Buku ────
    private DataGridView dgvBuku;
    private TextBox txtJudulBaru;
    private TextBox txtCariBuku;
    private Button btnTambahBuku, btnEditJudul, btnHapusBuku;
    private Button btnStaffKembali, btnStaffHilang;
    private Button btnCariBuku;

    // ─── Tab User ───
    private DataGridView dgvUser;
    private TextBox txtUsernameBaru, txtPasswordBaru, txtNimBaru, txtEmailBaru;
    private ComboBox cmbRoleBaru;
    private Button btnTambahUser, btnHapusUser;

    // ─── Tab Log ────
    private DataGridView dgvLogBuku, dgvLogUser;
    private Button btnRefreshLogBuku, btnRefreshLogUser;

    // ─── Tab Setting ─
    private TextBox txtNamaPerpus;
    private NumericUpDown numDurasi, numDendaHari, numDendaHilang;
    private Button btnSimpanSetting;

    // ─── Logout ─────
    private Button btnLogout;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var tabs = new TabControl { Dock = DockStyle.Fill };
        var tabBuku    = new TabPage("📚 Buku");
        var tabUser    = new TabPage("👤 User");
        var tabLogBuku = new TabPage("📋 Log Buku");
        var tabLogUser = new TabPage("📋 Log User");
        var tabSetting = new TabPage("⚙️ Pengaturan");

        // ── FORM ────────────────────────────────────────────────────────────
        ClientSize = new Size(1000, 640);
        MinimumSize = new Size(1000, 640);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Perpustakaan — Staff";

        // ── TAB BUKU ────────────────────────────────────────────────────────
        dgvBuku = BuatDGV();
        txtCariBuku = new TextBox { Location = new Point(10,10), Size = new Size(260,26), PlaceholderText = "Cari judul..." };
        txtCariBuku.KeyDown += txtCariBuku_KeyDown;
        btnCariBuku = SmallBtn("Cari", 280, 10); btnCariBuku.Click += btnCariBuku_Click;

        txtJudulBaru = new TextBox { Location = new Point(10, 46), Size = new Size(260, 26), PlaceholderText = "Judul buku baru..." };
        btnTambahBuku = SmallBtn("+ Tambah", 280, 46); btnTambahBuku.Click += btnTambahBuku_Click;
        btnEditJudul  = SmallBtn("✏ Edit",   385, 46); btnEditJudul.Click  += btnEditJudul_Click;
        btnHapusBuku  = SmallBtn("🗑 Hapus",  490, 46); btnHapusBuku.Click  += btnHapusBuku_Click;
        btnStaffKembali = SmallBtn("Kembali",  595, 46); btnStaffKembali.Click += btnStaffKembali_Click;
        btnStaffHilang  = SmallBtn("Hilang",   700, 46); btnStaffHilang.Click  += btnStaffHilang_Click;

        var panBukuTop = new Panel { Dock = DockStyle.Top, Height = 82, Padding = new Padding(0,0,0,4) };
        panBukuTop.Controls.AddRange(new Control[] {
            txtCariBuku, btnCariBuku,
            txtJudulBaru, btnTambahBuku, btnEditJudul, btnHapusBuku,
            btnStaffKembali, btnStaffHilang
        });
        tabBuku.Controls.Add(dgvBuku);
        tabBuku.Controls.Add(panBukuTop);

        // ── TAB USER ────────────────────────────────────────────────────────
        dgvUser = BuatDGV();

        var lblUN = Lab("Username:", 10, 12); var lblPW = Lab("Password:", 200, 12);
        var lblNIM = Lab("No. Identitas:", 10, 48); var lblEmail = Lab("Email:", 200, 48);
        var lblRole = Lab("Role:", 395, 48);

        txtUsernameBaru = new TextBox { Location = new Point(80, 10), Size = new Size(110, 24) };
        txtPasswordBaru = new TextBox { Location = new Point(275, 10), Size = new Size(110, 24), PasswordChar = '●' };
        txtNimBaru  = new TextBox { Location = new Point(100, 46), Size = new Size(90, 24) };
        txtEmailBaru = new TextBox { Location = new Point(275, 46), Size = new Size(110, 24) };
        cmbRoleBaru = new ComboBox { Location = new Point(440, 46), Size = new Size(110, 24), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbRoleBaru.Items.AddRange(new object[] { "Pengunjung", "Staff" });
        cmbRoleBaru.SelectedIndex = 0;

        btnTambahUser = new Button { Text = "➕ Tambah User", Location = new Point(560, 46), Size = new Size(130, 28) };
        btnTambahUser.Click += btnTambahUser_Click;
        btnHapusUser = new Button { Text = "🗑 Hapus User", Location = new Point(700, 46), Size = new Size(120, 28) };
        btnHapusUser.Click += btnHapusUser_Click;

        var panUserTop = new Panel { Dock = DockStyle.Top, Height = 82 };
        panUserTop.Controls.AddRange(new Control[] {
            lblUN, txtUsernameBaru, lblPW, txtPasswordBaru,
            lblNIM, txtNimBaru, lblEmail, txtEmailBaru,
            lblRole, cmbRoleBaru, btnTambahUser, btnHapusUser
        });
        tabUser.Controls.Add(dgvUser);
        tabUser.Controls.Add(panUserTop);

        // ── TAB LOG BUKU ────────────────────────────────────────────────────
        dgvLogBuku = BuatDGV();
        btnRefreshLogBuku = new Button { Text = "🔄 Refresh", Dock = DockStyle.Top, Height = 32 };
        btnRefreshLogBuku.Click += btnRefreshLogBuku_Click;
        tabLogBuku.Controls.Add(dgvLogBuku);
        tabLogBuku.Controls.Add(btnRefreshLogBuku);

        // ── TAB LOG USER ────────────────────────────────────────────────────
        dgvLogUser = BuatDGV();
        btnRefreshLogUser = new Button { Text = "🔄 Refresh", Dock = DockStyle.Top, Height = 32 };
        btnRefreshLogUser.Click += btnRefreshLogUser_Click;
        tabLogUser.Controls.Add(dgvLogUser);
        tabLogUser.Controls.Add(btnRefreshLogUser);

        // ── TAB SETTING ─────────────────────────────────────────────────────
        var panSetting = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

        txtNamaPerpus = new TextBox { Location = new Point(200, 20), Size = new Size(300, 26) };
        numDurasi     = new NumericUpDown { Location = new Point(200, 60), Size = new Size(120, 26), Minimum = 1, Maximum = 365, Value = 7 };
        numDendaHari  = new NumericUpDown { Location = new Point(200,100), Size = new Size(120, 26), Minimum = 0, Maximum = 1000000, Value = 2500, DecimalPlaces = 0 };
        numDendaHilang = new NumericUpDown { Location = new Point(200,140), Size = new Size(120, 26), Minimum = 0, Maximum = 10000000, Value = 50000, DecimalPlaces = 0 };
        btnSimpanSetting = new Button { Text = "💾 Simpan Pengaturan", Location = new Point(200, 185), Size = new Size(180, 34) };
        btnSimpanSetting.Click += btnSimpanSetting_Click;

        panSetting.Controls.AddRange(new Control[] {
            Lab("Nama Perpustakaan:", 20, 22), txtNamaPerpus,
            Lab("Durasi Pinjam (hari):", 20, 62), numDurasi,
            Lab("Denda per Hari (Rp):", 20, 102), numDendaHari,
            Lab("Denda Buku Hilang (Rp):", 20, 142), numDendaHilang,
            btnSimpanSetting,
        });
        tabSetting.Controls.Add(panSetting);

        // ── LOGOUT BUTTON ────────────────────────────────────────────────────
        btnLogout = new Button
        {
            Text = "Logout",
            Dock = DockStyle.Bottom,
            Height = 36,
            BackColor = Color.FromArgb(180, 40, 40),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
        };
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.Click += btnLogout_Click;

        tabs.TabPages.AddRange(new[] { tabBuku, tabUser, tabLogBuku, tabLogUser, tabSetting });
        Controls.Add(tabs);
        Controls.Add(btnLogout);

        SuspendLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    // ─── Helpers layout ─────────────────────────────────────────────────────
    private static DataGridView BuatDGV() => new DataGridView
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
        RowHeadersVisible = false,
        BackgroundColor = SystemColors.Window,
    };

    private static Button SmallBtn(string text, int x, int y) => new Button
    {
        Text = text, Location = new Point(x, y), Size = new Size(96, 26),
        Font = new Font("Segoe UI", 8.5f),
    };

    private static Label Lab(string text, int x, int y) => new Label
    {
        Text = text, Location = new Point(x, y), AutoSize = true,
        Font = new Font("Segoe UI", 9),
    };
}
