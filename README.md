# EduKita — Learning Management System

Website LMS full-stack yang dibangun 100% dengan .NET 8: backend dan frontend sama-sama
menggunakan ASP.NET Core **Razor Pages**.

## Teknologi

| Komponen | Teknologi |
|---|---|
| Backend | ASP.NET Core 8 (Razor Pages) |
| Frontend | Razor Pages + Bootstrap 5 + CSS kustom |
| Database | SQLite via Entity Framework Core 8 |
| Autentikasi | ASP.NET Core Identity + Roles |
| Budaya UI | Bahasa Indonesia (id-ID) |

## Menjalankan Aplikasi

```bash
cd EduKita.Web
dotnet run
```

Lalu buka URL yang muncul di terminal (biasanya `https://localhost:7xxx`).
Database `edukita.db` dibuat & di-seed otomatis saat pertama kali dijalankan.

> .NET SDK 8 terpasang di `C:\Users\<user>\AppData\Local\Microsoft\dotnet`.
> Jika `dotnet` tidak dikenali di terminal, jalankan:
> `setx PATH "%PATH%;%LOCALAPPDATA%\Microsoft\dotnet"` lalu buka terminal baru.

## Akun Demo

| Peran | Email | Password |
|---|---|---|
| Admin | `admin@edukita.test` | `Admin123!` |
| Instruktur | `budi@edukita.test` | `Instruktur123!` |
| Siswa | `siti@edukita.test` | `Student123!` |
| Siswa | `andi@edukita.test` | `Student123!` |

## Fitur

**Publik**
- Beranda dengan statistik, kategori, dan kursus terbaru
- Katalog kursus: pencarian (case-insensitive), filter kategori & level
- Detail kursus: kurikulum, profil instruktur, kursus terkait
- Pelajaran pratinjau gratis tanpa login

**Siswa** (`/Learn`)
- Pendaftaran kursus (enroll)
- Dashboard "Kursus Saya" dengan progress bar
- Player pembelajaran: sidebar kurikulum, video embed, materi HTML
- Tandai pelajaran selesai → otomatis lanjut ke pelajaran berikutnya
- Status "kursus selesai" otomatis saat semua pelajaran tuntas

**Instruktur** (`/Instructor`)
- Statistik dashboard (kursus, siswa, pelajaran)
- CRUD kursus, modul, dan pelajaran (mendukung video & materi HTML)
- Publish/sembunyikan kursus, lihat daftar siswa + progresnya

**Admin** (`/Admin`)
- Statistik platform & pendaftaran terbaru
- CRUD kategori, moderasi semua kursus
- Kelola pengguna & ubah peran (Admin/Instruktur/Siswa)

## Struktur Proyek

```
EduKita.Web/
├── Areas/Identity/Pages/Account/   # Login & Register kustom
├── Data/
│   ├── ApplicationDbContext.cs     # DbContext (Identity + LMS)
│   ├── DbInitializer.cs            # Seed roles, user, kursus contoh
│   └── Migrations/                 # Migrasi EF Core
├── Models/                         # Course, CourseModule, Lesson, Enrollment, dll.
├── Pages/
│   ├── Courses/                    # Katalog & detail (publik)
│   ├── Learn/                      # Dashboard & player siswa
│   ├── Instructor/                 # Panel instruktur (role: Instructor/Admin)
│   └── Admin/                      # Panel admin (role: Admin)
└── wwwroot/css/site.css            # Tema kustom EduKita
```

## Perintah Berguna

```bash
dotnet ef migrations add <Nama>     # buat migrasi baru
dotnet ef database update           # terapkan migrasi
dotnet ef database drop             # reset database (re-seed saat start)
```

## Ide Pengembangan Selanjutnya

- Kuis/ujian per modul dengan penilaian otomatis
- Sertifikat kelulusan (PDF)
- Upload file materi & gambar thumbnail
- Diskusi/tanya jawab per kursus
- Ganti SQLite → SQL Server untuk produksi (cukup ganti provider EF Core)
