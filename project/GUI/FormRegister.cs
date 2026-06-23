using DB_Repos;

namespace GUI;
public partial class FormRegister : Form
{
    private readonly UserRepository _userRepo;
    public FormRegister(UserRepository userRepo)
    {
        _userRepo = userRepo;
        InitializeComponent();
    }
    private void btnDaftar_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text.Trim();
        string konfirmasi = txtKonfirmasi.Text.Trim();
        string nim = txtNim.Text.Trim();
        string email = txtEmail.Text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Username dan password tidak boleh kosong.", "Peringatan",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (password != konfirmasi)
        {
            MessageBox.Show("Password dan konfirmasi password tidak cocok.", "Peringatan",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtKonfirmasi.Clear();
            txtKonfirmasi.Focus();
            return;
        }

        try
        {
            var user = new User
            {
                Username = username,
                Password = password,
                Role = "Pengunjung",
                NomorIdentitas = string.IsNullOrEmpty(nim)? null : nim,
                Email = string.IsNullOrEmpty(email)? null : email,
            };

            _userRepo.Tambah(user, username);

            MessageBox.Show($"Akun '{username}' berhasil didaftarkan!\nSilakan login dengan akun Anda.",
                "Registrasi Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Gagal mendaftar:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private void btnBatal_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
