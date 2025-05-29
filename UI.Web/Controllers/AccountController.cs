using AutoMapper;
using Core.Concretes.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Web.Models.Account;

namespace UI.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment env;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper, IWebHostEnvironment env)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
            this.env = env;
        }


        #region Member Profile:
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if(!ModelState.IsValid) return View(model);

            var user = model.UserNameorEmail.Contains("@")
                ? await userManager.FindByEmailAsync(model.UserNameorEmail)
                : await userManager.FindByNameAsync(model.UserNameorEmail);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt! UserName or Email Address not found.");
                return View(model);
            }
            
            var result = await signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);

            if(result.Succeeded) return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);

            ModelState.AddModelError(string.Empty, "Invalid login attempt! Incorrect information.");
            return View(model);
        }
        #endregion

        #region Register
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var user = mapper.Map<ApplicationUser>(model);

            if(model.ProfilePicture != null)
            {
                // Buraya resim yukleme gelecek.
            }

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Kayit basariliysa, login islemini otomatik yap.
                await signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        #endregion

        #region Reset Forgatten Password

        [HttpGet]
        public IActionResult ForgatPassword() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgatPassword(ForgotPasswordViewModel model)
        {
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation() => View();

        [HttpGet]
        public IActionResult AccessDenied() => View();

        #endregion

        #region Logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        #endregion

    }
}
