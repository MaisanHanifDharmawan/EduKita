using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync();

        foreach (var role in new[] { "Admin", "Instructor", "Student" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = await EnsureUserAsync(userManager, "admin@edukita.test", "Admin123!", "Admin EduKita", "Admin");
        var budi = await EnsureUserAsync(userManager, "budi@edukita.test", "Instruktur123!", "Budi Santoso", "Instructor",
            "Software engineer dengan 10 tahun pengalaman membangun aplikasi .NET untuk perusahaan finansial.");
        var siti = await EnsureUserAsync(userManager, "siti@edukita.test", "Student123!", "Siti Rahma", "Student");
        var andi = await EnsureUserAsync(userManager, "andi@edukita.test", "Student123!", "Andi Pratama", "Student");

        if (await db.Categories.AnyAsync() || await db.Courses.AnyAsync())
        {
            return;
        }

        var programming = new Category { Name = "Pemrograman", Icon = "code", Description = "Belajar bahasa pemrograman populer dari dasar." };
        var webDev = new Category { Name = "Web Development", Icon = "globe", Description = "Bangun website dan web app modern." };
        var dataAi = new Category { Name = "Data & AI", Icon = "data", Description = "Analisis data, machine learning, dan AI." };
        var design = new Category { Name = "Desain & Kreatif", Icon = "brush", Description = "UI/UX, grafis, dan konten kreatif." };
        var business = new Category { Name = "Bisnis & Karier", Icon = "briefcase", Description = "Keterampilan bisnis dan pengembangan karier." };
        db.Categories.AddRange(programming, webDev, dataAi, design, business);

        var csharpCourse = new Course
        {
            Title = "C# & .NET untuk Pemula: dari Nol sampai Bisa Buat Program",
            Summary = "Kuasai dasar pemrograman C# dan platform .NET dengan praktik langsung, tanpa perlu pengalaman coding sebelumnya.",
            Description = "<p>Kursus ini dirancang untuk Anda yang benar-benar baru di dunia pemrograman. Kita akan mulai dari nol: mengenal apa itu C# dan .NET, memasang tools, menulis program pertama, hingga memahami konsep object-oriented programming yang menjadi fondasi pengembangan aplikasi modern.</p><p>Semua materi disampaikan dengan bahasa yang sederhana dan dilengkapi contoh kode yang bisa langsung Anda coba di komputer sendiri.</p>",
            Level = "Pemula",
            Category = programming,
            InstructorId = budi.Id,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow.AddDays(-40),
            Modules =
            [
                new CourseModule { Position = 1, Title = "Persiapan & Pengenalan", Lessons =
                [
                    Lesson("Apa itu C# dan .NET?", 8, false, "<p>C# adalah bahasa pemrograman modern yang dikembangkan oleh Microsoft, sementara .NET adalah platform tempat program C# berjalan. Kombinasi keduanya digunakan jutaan developer di dunia untuk membangun website, aplikasi desktop, game, hingga layanan cloud.</p><p>Di pelajaran ini kita memahami posisi C# di antara bahasa lain dan mengapa ia cocok untuk pemula yang serius ingin menjadi developer.</p>"),
                    Lesson("Menyiapkan Lingkungan Development", 10, false, "<p>Sebelum menulis kode, kita perlu menyiapkan 'dapur' kita: mengunduh dan memasang .NET SDK, lalu code editor Visual Studio Code dengan ekstensi C#.</p><p>Ikuti langkah instalasi di video, dan pastikan perintah <code>dotnet --version</code> berjalan di terminal Anda sebelum lanjut ke pelajaran berikutnya.</p>"),
                    Lesson("Program Pertama: Hello World", 7, true, "<p>Saatnya menulis kode pertama Anda! Kita akan membuat program sederhana yang menampilkan teks ke layar.</p><pre><code>using System;\n\nclass Program\n{\n    static void Main()\n    {\n        Console.WriteLine(\"Hello, World!\");\n    }\n}</code></pre><p>Tekan tombol run, dan lihat teks <em>Hello, World!</em> muncul di konsol. Selamat, Anda resmi menjadi programmer! 🎉</p>"),
                    Lesson("Memahami Struktur Program C#", 9, false, "<p>Pada pelajaran ini kita bedah program Hello World tadi: apa itu <code>class</code>, apa itu method <code>Main</code>, mengapa ada tanda kurung kurawal, dan bagaimana C# membaca program Anda dari atas ke bawah.</p><p>Memahami struktur ini penting agar Anda tidak bingung saat program kita mulai membesar.</p>"),
                ]},
                new CourseModule { Position = 2, Title = "Fundamental Pemrograman", Lessons =
                [
                    Lesson("Variabel & Tipe Data", 12, false, "<p>Variabel adalah 'kotak' untuk menyimpan data. C# memiliki berbagai tipe data: <code>int</code> untuk angka bulat, <code>double</code> untuk desimal, <code>string</code> untuk teks, dan <code>bool</code> untuk benar/salah.</p><pre><code>string nama = \"Siti\";\nint umur = 20;\ndouble tinggi = 165.5;\nbool sudahMenikah = false;</code></pre><p>Latihan: buat variabel untuk menyimpan data diri Anda sendiri.</p>"),
                    Lesson("Operator & Ekspresi", 10, false, "<p>Operator adalah simbol untuk mengolah data: aritmatika (<code>+ - * /</code>), perbandingan (<code>== != &lt; &gt;</code>), dan logika (<code>&amp;&amp; || !</code>). Kita akan banyak menggunakannya sepanjang kursus ini.</p>"),
                    Lesson("Pengambilan Keputusan: if/else dan switch", 14, false, "<p>Program menjadi 'pintar' karena bisa mengambil keputusan. Dengan <code>if/else</code> program Anda bisa bereaksi berbeda tergantung kondisi.</p><pre><code>int nilai = 75;\n\nif (nilai >= 70)\n{\n    Console.WriteLine(\"Lulus!\");\n}\nelse\n{\n    Console.WriteLine(\"Coba lagi ya.\");\n}</code></pre>"),
                    Lesson("Perulangan: for, while, dan foreach", 15, false, "<p>Komputer unggul dalam mengulang. Perulangan <code>for</code> cocok untuk jumlah yang sudah diketahui, <code>while</code> untuk kondisi dinamis, dan <code>foreach</code> untuk menelusuri kumpulan data seperti list.</p><pre><code>for (int i = 1; i &lt;= 5; i++)\n{\n    Console.WriteLine($\"Perulangan ke-{i}\");\n}</code></pre>"),
                    Lesson("Method & Parameter", 16, false, "<p>Method adalah blok kode yang bisa dipanggil berulang kali — cara utama menjaga program tetap rapi. Kita belajar membuat method, mengirim parameter, dan mengembalikan nilai dengan <code>return</code>.</p><pre><code>static int Tambah(int a, int b)\n{\n    return a + b;\n}\n\nint hasil = Tambah(3, 4); // 7</code></pre>"),
                ]},
                new CourseModule { Position = 3, Title = "Konsep OOP Dasar", Lessons =
                [
                    Lesson("Class & Object", 12, false, "<p>Object-Oriented Programming adalah cara berpikir bahwa program terdiri dari <em>objek</em> yang memiliki data dan perilaku. <code>class</code> adalah cetak birunya, <code>object</code> adalah wujud nyatanya.</p><pre><code>class Mahasiswa\n{\n    public string Nama;\n    public void Perkenalan()\n    {\n        Console.WriteLine($\"Halo, saya {Nama}\");\n    }\n}</code></pre>"),
                    Lesson("Property & Enkapsulasi", 10, false, "<p>Property adalah cara elegan mengontrol akses ke data dalam class. Dengan enkapsulasi, data sensitif dilindungi dan hanya bisa diubah melalui 'pintu' yang kita tentukan.</p>"),
                    Lesson("Pewarisan (Inheritance)", 13, false, "<p>Inheritance memungkinkan sebuah class mewarisi data dan perilaku class lain. Contohnya: class <code>MahasiswaS2</code> mewarisi <code>Mahasiswa</code>, lalu menambahkan fitur sendiri — kode tidak perlu ditulis ulang.</p>"),
                    Lesson("Polimorfisme & Interface", 15, false, "<p>Polimorfisme membuat objek yang berbeda merespons perintah yang sama dengan caranya masing-masing. Interface adalah kontrak yang menstandarkan perilaku tersebut — konsep kunci untuk menulis kode yang fleksibel.</p>"),
                ]},
            ]
        };

        var razorCourse = new Course
        {
            Title = "Membangun Website dengan ASP.NET Core Razor Pages",
            Summary = "Bangun website full-stack dengan ASP.NET Core: Razor Pages, Entity Framework Core, hingga autentikasi pengguna.",
            Description = "<p>Setelah paham dasar C#, saatnya naik level! Di kursus ini kita membangun website lengkap dengan ASP.NET Core — mulai dari routing, mengakses database dengan EF Core, sampai melindungi halaman dengan login dan role.</p><p>Proyek akhir: website mini yang siap di-deploy. Persis seperti yang Anda gunakan sekarang ini! 😉</p>",
            Level = "Menengah",
            Category = webDev,
            InstructorId = budi.Id,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow.AddDays(-20),
            Modules =
            [
                new CourseModule { Position = 1, Title = "Dasar ASP.NET Core", Lessons =
                [
                    Lesson("Pengenalan ASP.NET Core & Razor Pages", 10, true, "<p>ASP.NET Core adalah framework web lintas platform dari Microsoft. Razor Pages adalah model pemrogramannya yang paling ramah pemula: satu halaman = satu file <code>.cshtml</code> (tampilan) + satu file <code>.cshtml.cs</code> (logika).</p>"),
                    Lesson("Struktur Proyek & Middleware", 12, false, "<p>Kita membedah isi proyek ASP.NET Core: <code>Program.cs</code> sebagai titik awal, folder <code>Pages</code>, <code>wwwroot</code> untuk file statis, dan konsep <em>middleware pipeline</em> yang memproses setiap request.</p>"),
                    Lesson("Routing & Page Handler", 11, false, "<p>Bagaimana URL seperti <code>/Kursus/Detail/5</code> menemukan halaman yang tepat? Di sini kita pelajari routing convention, parameter, dan handler <code>OnGet</code>/<code>OnPost</code>.</p>"),
                ]},
                new CourseModule { Position = 2, Title = "Menggunakan Data dengan EF Core", Lessons =
                [
                    Lesson("Pengenalan Entity Framework Core", 12, false, "<p>EF Core adalah ORM (Object-Relational Mapper): Anda bekerja dengan objek C#, EF Core yang menerjemahkannya menjadi query SQL ke database.</p><pre><code>var kursus = await db.Courses\n    .Where(c =&gt; c.IsPublished)\n    .ToListAsync();</code></pre>"),
                    Lesson("Migrasi Database", 14, false, "<p>Skema database berkembang seiring aplikasi. Migrasi EF Core memungkinkan Anda mengubah model lalu membuat script perubahan otomatis dengan <code>dotnet ef migrations add</code>.</p>"),
                    Lesson("CRUD dengan Razor Pages", 18, false, "<p>Materi paling penting: membuat halaman Create, Read, Update, Delete lengkap dengan validasi form. Setelah pelajaran ini Anda sudah bisa membuat aplikasi data sederhana sendiri.</p>"),
                ]},
                new CourseModule { Position = 3, Title = "Autentikasi & Otorisasi", Lessons =
                [
                    Lesson("ASP.NET Core Identity", 13, false, "<p>Identity menangani registrasi, login, dan manajemen pengguna secara aman — password di-hash, sesi di-cookie. Kita integrasikan Identity ke DbContext aplikasi.</p>"),
                    Lesson("Role-Based Authorization", 12, false, "<p>Tidak semua pengguna boleh membuka semua halaman. Dengan role (misalnya Admin, Instruktur, Siswa) dan atribut <code>[Authorize]</code>, kita batasi akses per halaman.</p>"),
                    Lesson("Mengamankan Halaman & Data", 10, false, "<p>Prinsip terakhir: selalu verifikasi kepemilikan data di server — jangan percaya input dari browser. Kita audit halaman kita dan menambal celahnya.</p>"),
                ]},
            ]
        };

        var pythonCourse = new Course
        {
            Title = "Python untuk Analisis Data",
            Summary = "Langkah pertama menjadi Data Analyst: Python, Pandas, dan visualisasi data dari nol.",
            Description = "<p>Data adalah mata uang baru, dan Python adalah alat paling populer untuk mengolahnya. Kursus ini membawa Anda dari sintaks dasar Python hingga mampu membersihkan, menganalisis, dan memvisualisasikan data nyata dengan Pandas dan Matplotlib.</p>",
            Level = "Pemula",
            Category = dataAi,
            InstructorId = budi.Id,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            Modules =
            [
                new CourseModule { Position = 1, Title = "Dasar-Dasar Python", Lessons =
                [
                    Lesson("Mengapa Python untuk Data?", 6, true, "<p>Python mendominasi dunia data science karena sintaksnya sederhana dan ekosistem library-nya sangat lengkap: NumPy, Pandas, Matplotlib, Scikit-learn. Di pelajaran ini kita lihat gambaran besar karier data dan peta belajarnya.</p>"),
                    Lesson("Variabel & Struktur Data", 15, false, "<p>List, tuple, dictionary, dan set — empat struktur data bawaan Python yang dipakai setiap hari oleh data analyst. Kita praktik dengan data sederhana seperti daftar nilai ujian.</p><pre><code>nilai = [80, 75, 90, 65]\nrata_rata = sum(nilai) / len(nilai)\nprint(rata_rata)  # 77.5</code></pre>"),
                    Lesson("Fungsi & Modul", 12, false, "<p>Tulis sekali, pakai berkali-kali. Kita belajar membuat fungsi dengan <code>def</code>, memakai modul bawaan, dan menginstal package dengan pip.</p>"),
                ]},
                new CourseModule { Position = 2, Title = "Analisis dengan Pandas", Lessons =
                [
                    Lesson("Pengenalan Pandas & DataFrame", 14, false, "<p>DataFrame adalah 'tabel' super canggih: baca file CSV/Excel, filter baris, pilih kolom, dan hitung statistik dalam satu baris kode.</p><pre><code>import pandas as pd\n\ndf = pd.read_csv(\"penjualan.csv\")\nprint(df.head())\nprint(df.describe())</code></pre>"),
                    Lesson("Membersihkan Data (Data Cleaning)", 16, false, "<p>Kenyataannya 80% pekerjaan analisis adalah membersihkan data: menangani nilai kosong (<code>NaN</code>), duplikat, dan tipe data yang berantakan. Skill yang membedakan analyst pemula dan profesional.</p>"),
                    Lesson("Agregasi & GroupBy", 15, false, "<p>\"Berapa total penjualan per kota?\" — pertanyaan seperti ini dijawab dengan <code>groupby</code>. Kita belajar agregasi, pivot table, dan menjawab pertanyaan bisnis dari data.</p>"),
                ]},
                new CourseModule { Position = 3, Title = "Visualisasi Data", Lessons =
                [
                    Lesson("Matplotlib Dasar", 12, false, "<p>Grafik yang baik menyampaikan cerita. Kita membuat line chart, bar chart, dan scatter plot dengan Matplotlib, lalu mempercantiknya dengan label, judul, dan warna.</p>"),
                    Lesson("Membuat Dashboard Sederhana", 14, false, "<p>Di pelajaran penutup kita menggabungkan semua skill: membaca data, membersihkan, menganalisis, dan menampilkan hasilnya dalam satu tampilan ringkas seperti dashboard mini.</p>"),
                ]},
            ]
        };

        db.Courses.AddRange(csharpCourse, razorCourse, pythonCourse);
        await db.SaveChangesAsync();

        // Contoh pendaftaran siswa dengan progres
        var enrollSitiCsharp = new Enrollment { UserId = siti.Id, CourseId = csharpCourse.Id, EnrolledAt = DateTime.UtcNow.AddDays(-12) };
        var enrollSitiPython = new Enrollment { UserId = siti.Id, CourseId = pythonCourse.Id, EnrolledAt = DateTime.UtcNow.AddDays(-2) };
        var enrollAndiRazor = new Enrollment { UserId = andi.Id, CourseId = razorCourse.Id, EnrolledAt = DateTime.UtcNow.AddDays(-6) };
        db.Enrollments.AddRange(enrollSitiCsharp, enrollSitiPython, enrollAndiRazor);
        await db.SaveChangesAsync();

        var csharpLessons = csharpCourse.Modules.OrderBy(m => m.Position).SelectMany(m => m.Lessons).ToList();
        foreach (var lesson in csharpLessons.Take(5))
        {
            db.LessonProgresses.Add(new LessonProgress { EnrollmentId = enrollSitiCsharp.Id, LessonId = lesson.Id });
        }
        var razorLessons = razorCourse.Modules.OrderBy(m => m.Position).SelectMany(m => m.Lessons).ToList();
        db.LessonProgresses.Add(new LessonProgress { EnrollmentId = enrollAndiRazor.Id, LessonId = razorLessons.First().Id });
        await db.SaveChangesAsync();
    }

    private static Lesson Lesson(string title, int minutes, bool isPreview, string content) => new()
    {
        Title = title,
        DurationMinutes = minutes,
        IsPreview = isPreview,
        Content = content,
    };

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string fullName,
        string role,
        string? bio = null)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            return user;
        }

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName,
            Bio = bio,
        };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new Exception($"Gagal membuat user seed {email}: {string.Join("; ", result.Errors.Select(e => e.Description))}");
        }
        await userManager.AddToRoleAsync(user, role);
        return user;
    }
}
