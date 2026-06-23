namespace GUI;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    private Label lblNamaPerpus;
    private Label lblUsername;
    private Label lblPassword;
    private TextBox txtUsername;
    private TextBox txtPassword;
    private Button btnLogin;
    private Button btnRegister;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblNamaPerpus = new Label();
        lblUsername   = new Label();
        lblPassword   = new Label();
        txtUsername   = new TextBox();
        txtPassword   = new TextBox();
        btnLogin      = new Button();
        btnRegister   = new Button();
        SuspendLayout();

        // Form
        ClientSize = new Size(360, 275);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Login";

        // lblNamaPerpus
        lblNamaPerpus.AutoSize = false;
        lblNamaPerpus.Font = new Font("Segoe UI", 13, FontStyle.Bold);
        lblNamaPerpus.Location = new Point(20, 18);
        lblNamaPerpus.Size = new Size(320, 30);
        lblNamaPerpus.TextAlign = ContentAlignment.MiddleCenter;
        lblNamaPerpus.Text = "Perpustakaan";

        // lblUsername
        lblUsername.AutoSize = true;
        lblUsername.Location = new Point(30, 68);
        lblUsername.Text = "Username";

        // txtUsername
        txtUsername.Location = new Point(30, 88);
        txtUsername.Size = new Size(300, 24);

        // lblPassword
        lblPassword.AutoSize = true;
        lblPassword.Location = new Point(30, 122);
        lblPassword.Text = "Password";

        // txtPassword
        txtPassword.Location = new Point(30, 142);
        txtPassword.Size = new Size(300, 24);
        txtPassword.PasswordChar = '●';
        txtPassword.KeyDown += txtPassword_KeyDown;

        // btnLogin
        btnLogin.Location = new Point(30, 178);
        btnLogin.Size = new Size(300, 34);
        btnLogin.Text = "Masuk";
        btnLogin.Font = new Font("Segoe UI", 10);
        btnLogin.Click += btnLogin_Click;

        // btnRegister
        btnRegister.Location = new Point(30, 222);
        btnRegister.Size = new Size(300, 30);
        btnRegister.Text = "Daftar Akun Baru";
        btnRegister.Font = new Font("Segoe UI", 9);
        btnRegister.FlatStyle = FlatStyle.Flat;
        btnRegister.ForeColor = Color.FromArgb(0, 102, 204);
        btnRegister.FlatAppearance.BorderSize = 1;
        btnRegister.Click += btnRegister_Click;

        Controls.AddRange(new Control[]
        {
            lblNamaPerpus, lblUsername, txtUsername,
            lblPassword, txtPassword, btnLogin, btnRegister
        });

        ResumeLayout(false);
        PerformLayout();
    }
}
