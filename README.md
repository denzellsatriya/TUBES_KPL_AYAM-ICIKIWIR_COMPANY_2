# Perpustakaan Net10 — Panduan Setup

## Prasyarat
- .NET 10 SDK
- MySQL / MariaDB
- Visual Studio 2022+ atau Rider

---

## 1. Import Database

Buka phpMyAdmin (atau MySQL Workbench) lalu jalankan file `kpl_perpus_import.sql`.
File ini akan membuat database `kpl_perpus` beserta semua tabel dan data awal.

---

## 2. Konfigurasi Koneksi

Edit file `DB_Repos/DatabaseConfig.cs`, sesuaikan:

```csharp
private const string Host     = "localhost";
private const int    Port     = 3306;
private const string DbUser   = "root";
private const string DbPassword = "";   // ← isi password MySQL Anda
```

---

## 3. Jalankan Program

```bash
cd GUI
dotnet run
```

---

## Akun Default

| Username | Password    | Role       |
|----------|-------------|------------|
| admin    | admin       | Staff      |
| denzell  | denzell123  | Pengunjung |
| naufal   | naufal123   | Pengunjung |

---

## Struktur Database

| Tabel                  | Kegunaan                                 |
|------------------------|------------------------------------------|
| `buku`                 | Data buku (id, judul, status, tgl_pinjam)|
| `users`                | Akun pengguna (Staff/Pengunjung)         |
| `log_buku`             | Riwayat setiap aksi pada buku            |
| `log_user`             | Riwayat login & aktivitas user           |
| `user_config_accounts` | Konfigurasi akun tambahan                |
| `user_config_settings` | Pengaturan: denda, durasi pinjam, nama   |

### Status Buku
- `0` = Tersedia
- `1` = Dipinjam
- `2` = Hilang

### Aksi Log Buku
`DITAMBAHKAN` · `PINJAM` · `KEMBALI` · `LAPOR_HILANG` · `DIUBAH` · `DIHAPUS`

---

## Fitur per Role

### Staff (Admin)
- Tambah / Edit judul / Hapus buku
- Pinjam · Kembalikan · Lapor Hilang (manual)
- Kelola user (tambah / hapus)
- Lihat log buku & log user
- Ubah pengaturan perpustakaan (nama, durasi, denda)

### Pengunjung
- Lihat & cari semua buku
- Pinjam buku (status 0 → 1)
- Kembalikan buku (status 1 → 0)
- Lapor buku hilang (status → 2)
- Lihat riwayat aktivitas sendiri
