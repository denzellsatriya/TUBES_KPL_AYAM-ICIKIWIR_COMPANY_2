using BCrypt.Net;

namespace DB_Repos
{
    public static class PasswordHelper
    {
        private const int WorkFactor = 12; // semakin tinggi = semakin aman tapi lebih lambat

        /// Hash password plain text menjadi BCrypt hash.
        /// Gunakan saat: registrasi user baru, ganti password.
        public static string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);
        }

        /// Verifikasi password input user terhadap hash di database.
        /// Gunakan saat: login.
        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}
