namespace GUI;

partial class FormRegister
{
    private System.ComponentModel.IContainer components = null;

    private Label lblJudul;
    private Label lblUsername;
    private Label lblPassword;
    private Label lblKonfirmasi;
    private Label lblNim;
    private Label lblEmail;
    private TextBox txtUsername;
    private TextBox txtPassword;
    private TextBox txtKonfirmasi;
    private TextBox txtNim;
    private TextBox txtEmail;
    private Button btnDaftar;
    private Button btnBatal;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblJudul     = new Label();
        lblUsername  = new Label();
        lblPassword  = new Label();
        lblKonfirmasi = new Label();
        lblNim       = new Label();
        lblEmail     = new Label();
        txtUsername  = new TextBox();
        txtPassword  = new TextBox();
        txtKonfirmasi = new TextBox();
        txtNim       = new TextBox();
        txtEmail     = new TextBox();
        btnDaftar    = new Button();
        btnBatal     = new Button();
        SuspendLayout();

        // Form
        ClientSize = new Size(400, 380);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Daftar Akun Baru";

        // lblJudul
        lblJudul.AutoSize = false;
        lblJudul.Font = new Font("Segoe UI", 13, FontStyle.Bold);
        lblJudul.Location = new Point(20, 16);
        lblJudul.Size = new Size(360, 30);
        lblJudul.TextAlign = ContentAlignment.MiddleCenter;
        lblJudul.Text = "Registrasi Akun Pengunjung";

        // lblUsername
        lblUsername.AutoSize = true;
        lblUsername.Location = new Point(30, 62);
        lblUsername.Text = "Username *";

        // txtUsername
        txtUsername.Location = new Point(30, 82);
        txtUsername.Size = new Size(340, 24);
        txtUsername.PlaceholderText = "Masukkan username";

        // lblPassword
        lblPassword.AutoSize = true;
        lblPassword.Location = new Point(30, 116);
        lblPassword.Text = "Password *";

        // txtPassword
        txtPassword.Location = new Point(30, 136);
        txtPassword.Size = new Size(340, 24);
        txtPassword.PasswordChar = '●';
        txtPassword.PlaceholderText = "Masukkan password";

        // lblKonfirmasi
        lblKonfirmasi.AutoSize = true;
        lblKonfirmasi.Location = new Point(30, 170);
        lblKonfirmasi.Text = "Konfirmasi Password *";

        // txtKonfirmasi
        txtKonfirmasi.Location = new Point(30, 190);
        txtKonfirmasi.Size = new Size(340, 24);
        txtKonfirmasi.PasswordChar = '●';
        txtKonfirmasi.PlaceholderText = "Ulangi password";

        // lblNim
        lblNim.AutoSize = true;
        lblNim.Location = new Point(30, 224);
        lblNim.Text = "No. Identitas (opsional)";

        // txtNim
        txtNim.Location = new Point(30, 244);
        txtNim.Size = new Size(340, 24);
        txtNim.PlaceholderText = "Nomor identitas / NIM / NIP";

        // lblEmail
        lblEmail.AutoSize = true;
        lblEmail.Location = new Point(30, 278);
        lblEmail.Text = "Email (opsional)";

        // txtEmail
        txtEmail.Location = new Point(30, 298);
        txtEmail.Size = new Size(340, 24);
        txtEmail.PlaceholderText = "contoh@email.com";

        // btnDaftar
        btnDaftar.Location = new Point(30, 334);
        btnDaftar.Size = new Size(160, 34);
        btnDaftar.Text = "Daftar";
        btnDaftar.Font = new Font("Segoe UI", 10);
        btnDaftar.BackColor = Color.FromArgb(0, 102, 204);
        btnDaftar.ForeColor = Color.White;
        btnDaftar.FlatStyle = FlatStyle.Flat;
        btnDaftar.FlatAppearance.BorderSize = 0;
        btnDaftar.Click += btnDaftar_Click;

        // btnBatal
        btnBatal.Location = new Point(210, 334);
        btnBatal.Size = new Size(160, 34);
        btnBatal.Text = "Batal";
        btnBatal.Font = new Font("Segoe UI", 10);
        btnBatal.FlatStyle = FlatStyle.Flat;
        btnBatal.Click += btnBatal_Click;

        Controls.AddRange(new Control[]
        {
            lblJudul,
            lblUsername, txtUsername,
            lblPassword, txtPassword,
            lblKonfirmasi, txtKonfirmasi,
            lblNim, txtNim,
            lblEmail, txtEmail,
            btnDaftar, btnBatal
        });

        ResumeLayout(false);
        PerformLayout();
    }
}
