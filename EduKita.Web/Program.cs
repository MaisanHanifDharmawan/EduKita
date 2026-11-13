using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
    options.AddPolicy("RequireInstructor", p => p.RequireRole("Instructor", "Admin"));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Learn");
    options.Conventions.AuthorizeFolder("/Instructor", "RequireInstructor");
    options.Conventions.AuthorizeFolder("/Admin", "RequireAdmin");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
}

var cultureInfo = new System.Globalization.CultureInfo("id-ID");
app.UseRequestLocalization(new Microsoft.AspNetCore.Builder.RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(cultureInfo),
    SupportedCultures = new[] { cultureInfo },
    SupportedUICultures = new[] { cultureInfo }
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

await DbInitializer.SeedAsync(app.Services);

app.Run();
