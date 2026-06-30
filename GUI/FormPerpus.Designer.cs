namespace GUI;

partial class FormPerpus
{
    private System.ComponentModel.IContainer components = null;

    private Label lblInfo;
    private TextBox txtCari;
    private ComboBox cmbFilter;
    private Button btnCari;
    private Button btnRefresh;
    private Button btnPinjam;
    private Button btnKembali;
    private Button btnLaporHilang;
    private Button btnRiwayat;
    private Button btnLogout;
    private DataGridView dgvBuku;
    private Panel panelTop;
    private Panel panelBottom;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblInfo = new Label();
        txtCari = new TextBox();
        cmbFilter = new ComboBox();
        btnCari = new Button();
        btnRefresh = new Button();
        btnPinjam = new Button();
        btnKembali = new Button();
        btnLaporHilang = new Button();
        btnRiwayat = new Button();
        btnLogout = new Button();
        dgvBuku = new DataGridView();
        panelTop = new Panel();
        panelBottom = new Panel();

        SuspendLayout();

        // Form
        ClientSize = new Size(900, 580);
        MinimumSize = new Size(900, 580);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Perpustakaan — Pengunjung";

        // panelTop
        panelTop.Dock = DockStyle.Top;
        panelTop.Height = 90;
        panelTop.Padding = new Padding(10, 8, 10, 8);

        // lblInfo
        lblInfo.AutoSize = false;
        lblInfo.Dock = DockStyle.Top;
        lblInfo.Height = 30;
        lblInfo.Font = new Font("Segoe UI", 9);
        lblInfo.Text = "Memuat...";

        // txtCari
        txtCari.Location = new Point(10, 44);
        txtCari.Size = new Size(320, 24);
        txtCari.PlaceholderText = "Cari judul buku...";
        txtCari.KeyDown += txtCari_KeyDown;

        // cmbFilter
        cmbFilter.Location = new Point(340, 44);
        cmbFilter.Size = new Size(120, 24);
        cmbFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbFilter.Items.AddRange(new object[] { "Semua", "Tersedia", "Dipinjam", "Hilang" });
        cmbFilter.SelectedIndex = 0;

        // btnCari
        btnCari.Location = new Point(470, 44);
        btnCari.Size = new Size(80, 26);
        btnCari.Text = "Cari";
        btnCari.Click += btnCari_Click;

        // btnRefresh
        btnRefresh.Location = new Point(558, 44);
        btnRefresh.Size = new Size(80, 26);
        btnRefresh.Text = "Reset";
        btnRefresh.Click += btnRefresh_Click;

        panelTop.Controls.AddRange(new Control[]
        {
            lblInfo, txtCari, cmbFilter, btnCari, btnRefresh
        });

        // dgvBuku
        dgvBuku.Dock = DockStyle.Fill;
        dgvBuku.ReadOnly = true;
        dgvBuku.AllowUserToAddRows = false;
        dgvBuku.AllowUserToDeleteRows = false;
        dgvBuku.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvBuku.MultiSelect = false;
        dgvBuku.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        dgvBuku.RowHeadersVisible = false;
        dgvBuku.BackgroundColor = SystemColors.Window;

        // panelBottom
        panelBottom.Dock = DockStyle.Bottom;
        panelBottom.Height = 46;
        panelBottom.Padding = new Padding(10, 8, 10, 8);

        void StyleBtn(Button b, string text, Color color)
        {
            b.Text = text;
            b.Size = new Size(130, 30);
            b.BackColor = color;
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Font = new Font("Segoe UI", 9);
        }

        StyleBtn(btnPinjam, "Pinjam Buku", Color.FromArgb(34, 139, 34));
        btnPinjam.Location = new Point(10, 8);
        btnPinjam.Click += btnPinjam_Click;

        StyleBtn(btnKembali, "Kembalikan", Color.FromArgb(30, 100, 200));
        btnKembali.Location = new Point(148, 8);
        btnKembali.Click += btnKembali_Click;

        StyleBtn(btnLaporHilang, "Lapor Hilang", Color.FromArgb(200, 80, 0));
        btnLaporHilang.Location = new Point(286, 8);
        btnLaporHilang.Click += btnLaporHilang_Click;

        StyleBtn(btnRiwayat, "Riwayat Saya", Color.FromArgb(100, 60, 160));
        btnRiwayat.Location = new Point(424, 8);
        btnRiwayat.Click += btnRiwayat_Click;

        StyleBtn(btnLogout, "Logout", Color.FromArgb(180, 40, 40));
        btnLogout.Location = new Point(750, 8);
        btnLogout.Click += btnLogout_Click;

        panelBottom.Controls.AddRange(new Control[]
        {
            btnPinjam, btnKembali, btnLaporHilang, btnRiwayat, btnLogout
        });

        Controls.AddRange(new Control[] { dgvBuku, panelBottom, panelTop });

        ResumeLayout(false);
        PerformLayout();
    }
}
