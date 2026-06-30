using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DB_Repos; 

namespace Perpustakaan.Tests
{
    // ==========================================
    // 1. FITUR KEAMANAN & ENKRIPSI (PASSWORD HELPER)
    // ==========================================
    [TestClass]
    public class PasswordHelperTests
    {
        [TestMethod]
        public void TestHashPassword_ShouldReturnValidBCryptHash()
        {
            string plain = "AyamIcikiwir123";
            string hashed = PasswordHelper.HashPassword(plain);

            Assert.IsNotNull(hashed);
            Assert.AreNotEqual(plain, hashed);
            Assert.IsTrue(PasswordHelper.VerifyPassword(plain, hashed));
        }

        [TestMethod]
        public void TestVerifyPassword_WithWrongPassword_ShouldReturnFalse()
        {
            string plain = "AyamIcikiwir123";
            string hashed = PasswordHelper.HashPassword(plain);

            Assert.IsFalse(PasswordHelper.VerifyPassword("SalahPassword", hashed));
        }
    }

    // ==========================================
    // 2. FITUR MANAGEMENT USER & AUTENTIKASI
    // ==========================================
    [TestClass]
    public class UserFeatureTests
    {
        [TestMethod]
        public void TestUserModelInstantiation_ShouldStoreDataCorrectly()
        {
            var user = new User
            {
                Username = "denzell",
                Password = "HashedPasswordHere",
                NomorIdentitas = "1301210000",
                Email = "denzell@student.telkomuniversity.ac.id",
                Role = "Staff",
                TanggalDibuat = DateTime.Now
            };

            Assert.AreEqual("denzell", user.Username);
            Assert.AreEqual("Staff", user.Role);
            Assert.IsNotNull(user.Email);
        }
    }

    // ==========================================
    // 3. FITUR KATALOG BUKU
    // ==========================================
    [TestClass]
    public class BukuFeatureTests
    {
        [TestMethod]
        public void TestBukuModelInstantiation_ShouldStoreDataCorrectly()
        {
            var buku = new Buku
            {
                Id = 1,
                Judul = "Tutorial Unit Testing KPL",
                Status = 0, // 0 = Tersedia
                TanggalDibuat = DateTime.Now
            };

            Assert.AreEqual(1, buku.Id);
            Assert.AreEqual("Tutorial Unit Testing KPL", buku.Judul);
            Assert.AreEqual("Tersedia", buku.StatusLabel); // Menguji fungsi switch label status banyolan
        }
    }

    // ==========================================
    // 4. FITUR AUDIT LOG (LOG USER & LOG BUKU)
    // ==========================================
    [TestClass]
    public class AuditLogFeatureTests
    {
        [TestMethod]
        public void TestLogUser_ShouldTrackActivityTimestamp()
        {
            var now = DateTime.Now;
            var logUser = new LogUser
            {
                Id = 1,
                Username = "sheva",
                Aksi = "Login Aplikasi",
                Waktu = now
            };

            Assert.AreEqual("sheva", logUser.Username);
            Assert.AreEqual("Login Aplikasi", logUser.Aksi);
            Assert.AreEqual(now, logUser.Waktu);
        }

        [TestMethod]
        public void TestLogBuku_ShouldTrackBookActivity()
        {
            var now = DateTime.Now;
            var logBuku = new LogBuku
            {
                Id = 10,
                BukuId = 1,
                Aksi = "Peminjaman Buku",
                OlehSiapa = "naufal",
                Waktu = now
            };

            Assert.AreEqual(1, logBuku.BukuId);
            Assert.AreEqual("Peminjaman Buku", logBuku.Aksi);
            Assert.AreEqual("naufal", logBuku.OlehSiapa);
        }
    }

    // ==========================================
    // 5. FITUR KONFIGURASI PENGGUNA (USER CONFIG)
    // ==========================================
    [TestClass]
    public class UserConfigFeatureTests
    {
        [TestMethod]
        public void TestUserConfigAccount_ShouldStoreSettingsCorrectly()
        {
            var configAcc = new UserConfigAccount
            {
                Id = 1,
                AccountType = "StaffAccounts",
                Username = "bintang",
                Role = "Admin",
                Nama = "Muhammad Bintang"
            };

            Assert.AreEqual("StaffAccounts", configAcc.AccountType);
            Assert.AreEqual("Muhammad Bintang", configAcc.Nama);
        }

        [TestMethod]
        public void TestPerpusSetting_ShouldHaveDefaultValues()
        {
            var setting = new PerpusSetting();

            Assert.AreEqual(7, setting.DurasiPinjamHari);
            Assert.AreEqual(2500m, setting.DendaPerHari);
            Assert.AreEqual("Perpustakaan", setting.NamaPerpustakaan);
        }
    }
}