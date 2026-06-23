using DB_Repos;

namespace GUI;
public partial class Form1 : Form
{
    private readonly UserRepository _userRepo = new();
    private readonly UserConfigRepository _configRepo = new();
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
    // tombol Login
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
    // Enter di password field
    private void txtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter) btnLogin_Click(sender, e);
    }
    // tombol Daftar
    private void btnRegister_Click(object sender, EventArgs e)
    {
        var formRegister = new FormRegister(_userRepo);
        formRegister.ShowDialog(this);
    }
}
