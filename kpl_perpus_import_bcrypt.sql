-- SQL hasil konversi dari file JSON: Buku, LogBuku, LogUser, user, userConfig
-- Database target: kpl_perpus
-- PASSWORD TELAH DI-HASH DENGAN BCrypt (work factor 12)
-- Bisa di-import lewat phpMyAdmin > database kpl_perpus > tab Import/SQL.
--
-- PENTING: Setelah import ini, update kode C# untuk menggunakan BCrypt.Net-Next
--   Install NuGet: BCrypt.Net-Next
--   Verify password: BCrypt.Net.BCrypt.Verify(inputPassword, hashDariDB)
--   Hash password baru: BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 12)

CREATE DATABASE IF NOT EXISTS `kpl_perpustakaan` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `kpl_perpus`;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS `log_user`;
DROP TABLE IF EXISTS `log_buku`;
DROP TABLE IF EXISTS `user_config_settings`;
DROP TABLE IF EXISTS `user_config_accounts`;
DROP TABLE IF EXISTS `users`;
DROP TABLE IF EXISTS `buku`;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE `buku` (
  `id` INT NOT NULL,
  `judul` VARCHAR(255) NOT NULL,
  `status` INT NOT NULL DEFAULT 0,
  `tanggal_pinjam` DATETIME(6) NULL,
  `tanggal_dibuat` DATETIME(6) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `users` (
  `username` VARCHAR(50) NOT NULL,
  `password` VARCHAR(255) NOT NULL,   -- BCrypt hash, selalu 60 karakter
  `nomor_identitas` VARCHAR(50) NULL,
  `email` VARCHAR(100) NULL,
  `role` VARCHAR(50) NOT NULL,
  `tanggal_dibuat` DATETIME(6) NULL,
  PRIMARY KEY (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `log_buku` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `buku_id` INT NULL,
  `waktu` DATETIME(6) NULL,
  `aksi` VARCHAR(50) NOT NULL,
  `oleh_siapa` VARCHAR(100) NULL,
  `keterangan` TEXT NULL,
  `message` TEXT NULL,
  `timestamp` DATETIME(6) NULL,
  `sumber_file` VARCHAR(50) NULL,
  PRIMARY KEY (`id`),
  INDEX `idx_log_buku_buku_id` (`buku_id`),
  CONSTRAINT `fk_log_buku_buku`
    FOREIGN KEY (`buku_id`) REFERENCES `buku` (`id`)
    ON UPDATE CASCADE ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `log_user` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(50) NULL,
  `waktu` DATETIME(6) NULL,
  `aksi` VARCHAR(50) NOT NULL,
  `keterangan` TEXT NULL,
  `message` TEXT NULL,
  `timestamp` DATETIME(6) NULL,
  PRIMARY KEY (`id`),
  INDEX `idx_log_user_username` (`username`),
  CONSTRAINT `fk_log_user_users`
    FOREIGN KEY (`username`) REFERENCES `users` (`username`)
    ON UPDATE CASCADE ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `user_config_accounts` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `account_type` VARCHAR(50) NOT NULL,
  `username` VARCHAR(50) NOT NULL,
  `password` VARCHAR(255) NOT NULL,   -- BCrypt hash, selalu 60 karakter
  `role` VARCHAR(50) NOT NULL,
  `nama` VARCHAR(100) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `user_config_settings` (
  `setting_key` VARCHAR(100) NOT NULL,
  `setting_value` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`setting_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- DATA BUKU (tidak berubah)
-- ============================================================
INSERT INTO `buku` (`id`, `judul`, `status`, `tanggal_pinjam`, `tanggal_dibuat`) VALUES (1, 'tutorial coding Java', 2, NULL, '2026-06-03 15:10:03.372642');
INSERT INTO `buku` (`id`, `judul`, `status`, `tanggal_pinjam`, `tanggal_dibuat`) VALUES (2, 'tips oacaran tanpa selingkuh', 0, NULL, '2026-06-03 15:10:21.792810');
INSERT INTO `buku` (`id`, `judul`, `status`, `tanggal_pinjam`, `tanggal_dibuat`) VALUES (3, 'kiat hidup sukses', 0, NULL, '2026-06-03 15:10:47.166676');
INSERT INTO `buku` (`id`, `judul`, `status`, `tanggal_pinjam`, `tanggal_dibuat`) VALUES (4, 'cara biar ga iri', 0, NULL, '2026-06-03 15:10:52.953269');
INSERT INTO `buku` (`id`, `judul`, `status`, `tanggal_pinjam`, `tanggal_dibuat`) VALUES (5, 'kisah cintaku', 0, NULL, '2026-06-03 15:11:07.902485');
INSERT INTO `buku` (`id`, `judul`, `status`, `tanggal_pinjam`, `tanggal_dibuat`) VALUES (6, 'biografi Albert Einstein', 0, NULL, '2026-06-03 15:11:25.152748');
INSERT INTO `buku` (`id`, `judul`, `status`, `tanggal_pinjam`, `tanggal_dibuat`) VALUES (7, 'Karen masa lalu atau masa depan?', 0, NULL, '2026-06-03 15:12:04.207740');
INSERT INTO `buku` (`id`, `judul`, `status`, `tanggal_pinjam`, `tanggal_dibuat`) VALUES (8, 'Tama mengguncang dunia', 0, NULL, '2026-06-03 15:12:36.953328');
INSERT INTO `buku` (`id`, `judul`, `status`, `tanggal_pinjam`, `tanggal_dibuat`) VALUES (9, 'kenangan yang terindah', 0, NULL, '2026-06-03 15:12:55.918937');
INSERT INTO `buku` (`id`, `judul`, `status`, `tanggal_pinjam`, `tanggal_dibuat`) VALUES (10, 'sang putri selir', 0, NULL, '2026-06-03 15:13:20.008703');
INSERT INTO `buku` (`id`, `judul`, `status`, `tanggal_pinjam`, `tanggal_dibuat`) VALUES (11, 'Cei lucu', 0, NULL, '2026-06-18 22:10:22.834657');

-- ============================================================
-- DATA USERS — password di-hash BCrypt (work factor 12)
-- Plain text asli:
--   admin    -> 'admin'
--   denzell  -> 'denzell123'
--   naufal   -> 'naufal123'
-- ============================================================
INSERT INTO `users` (`username`, `password`, `nomor_identitas`, `email`, `role`, `tanggal_dibuat`) VALUES
  ('admin',
   '$2b$12$kPufaVzDrXHciwSEZlTtWu73MLgi0.A3zRt/kl/e79IdE9BReCWqy',
   NULL, NULL, 'Staff', '2026-06-03 14:20:13.092160');

INSERT INTO `users` (`username`, `password`, `nomor_identitas`, `email`, `role`, `tanggal_dibuat`) VALUES
  ('denzell',
   '$2b$12$VWkskgLP6CL8NzW3iF1ST.1fNCIrE9XRXKSnIW7dvHs.H74xn7GdG',
   '103022400034', 'denzellsatriya@gmail.com', 'Pengunjung', '2026-06-03 14:34:30.616129');

INSERT INTO `users` (`username`, `password`, `nomor_identitas`, `email`, `role`, `tanggal_dibuat`) VALUES
  ('naufal',
   '$2b$12$t3NjdXHYw.xuxRof6/nVXOxAnZIn8EurCoTGq4rehGQ2xiI.WRT0.',
   '103022400059', 'naufal@gmail.com', 'Pengunjung', '2026-06-18 20:33:46.207914');

-- ============================================================
-- DATA LOG BUKU (tidak berubah)
-- ============================================================
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (1, '2026-06-03 15:10:03.401077', 'DITAMBAHKAN', 'admin', 'Buku ''tutorial coding Java'' ditambahkan', NULL, NULL, 'Buku.json/Logs');
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (1, '2026-06-03 15:33:08.057839', 'LAPOR_HILANG', 'denzell', 'Buku ''tutorial coding Java'' dilaporkan hilang', NULL, NULL, 'Buku.json/Logs');
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (2, '2026-06-03 15:10:21.796449', 'DITAMBAHKAN', 'admin', 'Buku ''tips oacaran tanpa selingkuh'' ditambahkan', NULL, NULL, 'Buku.json/Logs');
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (3, '2026-06-03 15:10:47.168217', 'DITAMBAHKAN', 'admin', 'Buku ''kiat hidup sukses'' ditambahkan', NULL, NULL, 'Buku.json/Logs');
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (3, '2026-06-18 21:41:14.254262', 'PINJAM', 'admin', 'Buku ''kiat hidup sukses'' dipinjam oleh admin', NULL, NULL, 'Buku.json/Logs');
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (4, '2026-06-03 15:10:52.955122', 'DITAMBAHKAN', 'admin', 'Buku ''cara biar ga iri'' ditambahkan', NULL, NULL, 'Buku.json/Logs');
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (5, '2026-06-03 15:11:07.904105', 'DITAMBAHKAN', 'admin', 'Buku ''kisah cintaku'' ditambahkan', NULL, NULL, 'Buku.json/Logs');
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (6, '2026-06-03 15:11:25.154250', 'DITAMBAHKAN', 'admin', 'Buku ''biografi Albert Einstein'' ditambahkan', NULL, NULL, 'Buku.json/Logs');
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (7, '2026-06-03 15:12:04.209056', 'DITAMBAHKAN', 'admin', 'Buku ''Karen masa lalu atau masa depan?'' ditambahkan', NULL, NULL, 'Buku.json/Logs');
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (8, '2026-06-03 15:12:36.955100', 'DITAMBAHKAN', 'admin', 'Buku ''Tama mengguncang dunia'' ditambahkan', NULL, NULL, 'Buku.json/Logs');
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (9, '2026-06-03 15:12:55.920654', 'DITAMBAHKAN', 'admin', 'Buku ''kenangan yang terindah'' ditambahkan', NULL, NULL, 'Buku.json/Logs');
INSERT INTO `log_buku` (`buku_id`, `waktu`, `aksi`, `oleh_siapa`, `keterangan`, `message`, `timestamp`, `sumber_file`) VALUES (11, '2026-06-18 22:10:22.842666', 'DITAMBAHKAN', 'admin', 'Buku ''Cei lucu'' ditambahkan oleh admin', NULL, NULL, 'LogBuku.json');

-- ============================================================
-- DATA LOG USER (tidak berubah)
-- ============================================================
INSERT INTO `log_user` (`username`, `waktu`, `aksi`, `keterangan`, `message`, `timestamp`) VALUES ('admin', '2026-06-20 11:42:13.155345', 'LOGIN', 'Login sebagai Staff', NULL, NULL);

-- ============================================================
-- USER CONFIG ACCOUNTS — password di-hash BCrypt (work factor 12)
-- Plain text asli:
--   admin  -> 'admin'
--   karen  -> 'karen123'
-- ============================================================
INSERT INTO `user_config_accounts` (`account_type`, `username`, `password`, `role`, `nama`) VALUES
  ('StaffAccounts', 'admin',
   '$2b$12$kPufaVzDrXHciwSEZlTtWu73MLgi0.A3zRt/kl/e79IdE9BReCWqy',
   'Staff', 'Admin Perpus');

INSERT INTO `user_config_accounts` (`account_type`, `username`, `password`, `role`, `nama`) VALUES
  ('PengunjungAccount', 'karen',
   '$2b$12$S9UZEz09O/eJNA3GhFhYVuiQ9bixc1gh2ZPKsE/XPKPngEJOa2JaS',
   'Pengunjung', NULL);

-- ============================================================
-- SETTINGS (tidak berubah)
-- ============================================================
INSERT INTO `user_config_settings` (`setting_key`, `setting_value`) VALUES ('DurasiPinjamHari', '7');
INSERT INTO `user_config_settings` (`setting_key`, `setting_value`) VALUES ('DendaPerHari', '2500');
INSERT INTO `user_config_settings` (`setting_key`, `setting_value`) VALUES ('DendaBukuHilang', '50000');
INSERT INTO `user_config_settings` (`setting_key`, `setting_value`) VALUES ('NamaPerpustakaan', 'Perpustakaan Ayam Icikiwir');

-- Cek hasil
SELECT COUNT(*) AS total_buku FROM `buku`;
SELECT COUNT(*) AS total_users FROM `users`;
SELECT COUNT(*) AS total_log_buku FROM `log_buku`;
SELECT COUNT(*) AS total_log_user FROM `log_user`;
