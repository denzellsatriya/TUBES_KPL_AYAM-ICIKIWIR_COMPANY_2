using DB_Repos;

namespace GUI;

public partial class Form1 : Form
{
    private readonly UserRepository _userRepo = UserRepository.Instance;
    private readonly UserConfigRepository _configRepo = UserConfigRepository.Instance;

    public Form1()
    {
        InitializeComponent();

        // Tampilkan nama perpustakaan di title bar
        try
        {
            var setting = _configRepo.GetPerpusSetting();
            this.Text = $"Login — {setting.NamaPerpustakaan}";
            lblNamaPerpus.Text = setting.NamaPerpustakaan;
        }
        catch
        {
            this.Text = "Login — Perpustakaan";
        }
    }

    // ─── Event: tombol Login ───────────────────────────────────────────────
    private void btnLogin_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Username dan password tidak boleh kosong.", "Peringatan",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            User? user = _userRepo.Login(username, password);
            if (user == null)
            {
                MessageBox.Show("Username atau password salah.", "Login Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            this.Hide();

            if (user.Role == "Staff")
            {
                var formStaff = new FormStaff(user);
                formStaff.FormClosed += (s, _) => Application.Exit();
                formStaff.Show();
            }
            else
            {
                var formPerpus = new FormPerpus(user);
                formPerpus.FormClosed += (s, _) => Application.Exit();
                formPerpus.Show();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Terjadi kesalahan:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ─── Event: Enter di password field ───────────────────────────────────
    private void txtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter) btnLogin_Click(sender, e);
    }

    // ─── Event: tombol Daftar ──────────────────────────────────────────────
    private void btnRegister_Click(object sender, EventArgs e)
    {
        var formRegister = new FormRegister(_userRepo);
        this.Hide();
        formRegister.ShowDialog(this);
        this.Show();
    }
}
