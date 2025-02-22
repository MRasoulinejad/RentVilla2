using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using RentVilla.Application.Common.Interfaces;
using RentVilla.Application.Common.Utility;
using RentVilla.Application.Services.Interface;
using RentVilla.Domain.Entities;
using RentVilla.Infrastructure.Repository;
using RentVilla.web.ViewModels;

namespace RentVilla.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISmtpEmailService _SmtpEmailService;
        public AccountController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ISmtpEmailService smtpEmailService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _SmtpEmailService = smtpEmailService;

        }
        public IActionResult Login(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            LoginVM loginVM = new()
            {
                RedirectUrl = returnUrl,
            };

            return View(loginVM);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Register(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            RegisterVM registerVM = new()
            {

                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }),
                RedirectUrl = returnUrl,

            };

            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Name = registerVM.Name,
                    Email = registerVM.Email,
                    PhoneNumber = registerVM.PhoneNumber,
                    NormalizedEmail = registerVM.Email.ToUpper(),
                    UserName = registerVM.Email,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now,

                };

                var result = await _userManager.CreateAsync(user, registerVM.Password);

                if (result.Succeeded)
                {
                    //if (!string.IsNullOrEmpty(registerVM.Role))
                    //{
                    //    await _userManager.AddToRoleAsync(user, registerVM.Role);
                    //}
                    //else
                    //{
                    //    await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                    //}

                    await _userManager.AddToRoleAsync(user, SD.Role_Customer);

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (string.IsNullOrEmpty(registerVM.RedirectUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return LocalRedirect(registerVM.RedirectUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            //registerVM.RoleList = _roleManager.Roles.Select(x => new SelectListItem
            //{
            //    Text = x.Name,
            //    Value = x.Name
            //});

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM LoginVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager
                    .PasswordSignInAsync(LoginVM.Email, LoginVM.Password, LoginVM.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(LoginVM.Email);
                    if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(LoginVM.RedirectUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return LocalRedirect(LoginVM.RedirectUrl);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                }
            }
            return View(LoginVM);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // For security, we don't reveal whether the email exists
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Create password reset link
            var callbackUrl = Url.Action(
                "ResetPassword",
                "Account",
                new { email = user.Email, token = token },
                protocol: HttpContext.Request.Scheme);

            // Create email template
            var emailBody = $@"
            <h2>Reset Your Password</h2>
            <p>To reset your password, please click the link below:</p>
            <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Reset Password</a></p>
            <p>If you didn't request a password reset, please ignore this email.</p>";

            await _SmtpEmailService.SendEmailAsync(
                model.Email,
                "Password Reset Request",
                emailBody);

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
