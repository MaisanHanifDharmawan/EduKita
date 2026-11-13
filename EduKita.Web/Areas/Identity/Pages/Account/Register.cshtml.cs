using System.ComponentModel.DataAnnotations;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduKita.Web.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Nama lengkap wajib diisi")]
        [StringLength(100, ErrorMessage = "Nama maksimal 100 karakter")]
        [Display(Name = "Nama Lengkap")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password minimal 8 karakter")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Konfirmasi password tidak sama")]
        [Display(Name = "Ulangi Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pilih peran Anda")]
        public string Role { get; set; } = "Student";
    }

    public void OnGet(string? role = null, string? returnUrl = null)
    {
        if (role == "Instructor")
        {
            Input.Role = "Instructor";
        }
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        if (Input.Role != "Instructor" && Input.Role != "Student")
        {
            Input.Role = "Student";
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            EmailConfirmed = true,
            FullName = Input.FullName,
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("User baru terdaftar: {Email}", Input.Email);
            await _userManager.AddToRoleAsync(user, Input.Role);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(Input.Role == "Instructor" ? "/Instructor" : returnUrl);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, TranslateError(error.Description));
        }

        return Page();
    }

    private static string TranslateError(string error) => error switch
    {
        var e when e.Contains("is already taken") => "Email sudah terdaftar, silakan masuk atau gunakan email lain.",
        var e when e.Contains("must have at least one uppercase") => "Password harus mengandung minimal satu huruf kapital.",
        var e when e.Contains("must have at least one lowercase") => "Password harus mengandung minimal satu huruf kecil.",
        var e when e.Contains("must have at least one digit") => "Password harus mengandung minimal satu angka.",
        var e when e.Contains("must have at least one non alphanumeric") => "Password harus mengandung minimal satu karakter khusus.",
        var e when e.Contains("following errors were detected") => "Password melebihi panjang maksimal.",
        _ => error,
    };
}
