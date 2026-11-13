using System.ComponentModel.DataAnnotations;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduKita.Web.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ingat saya")]
        public bool RememberMe { get; set; }
    }

    public void OnGet(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            _logger.LogInformation("User berhasil masuk: {Email}", Input.Email);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
            var roles = user is null ? new List<string>() : (await _signInManager.UserManager.GetRolesAsync(user)).ToList();
            if (roles.Contains("Admin"))
            {
                return LocalRedirect("~/Admin");
            }
            if (roles.Contains("Instructor"))
            {
                return LocalRedirect("~/Instructor");
            }
            return LocalRedirect("~/Learn");
        }

        ModelState.AddModelError(string.Empty, "Email atau password salah.");
        return Page();
    }
}
