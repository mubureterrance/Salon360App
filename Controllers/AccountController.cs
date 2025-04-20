using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Salon360App.Data;
using Salon360App.Enums;
using Salon360App.Models;
using Salon360App.Services;
using Salon360App.Services.Interfaces;
using Salon360App.ViewModels.AccountViewModels;
using Salon360App.ViewModels.LoginViewModels;
using Salon360App.ViewModels.ProfileViewModels;
using System.Security.Claims;

namespace Salon360App.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SalonDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IStaffRoleService _staffRoleService;
        private readonly IStaffService _staffService;

        public AccountController(
            ILogger<AccountController> logger,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            SalonDbContext context,
            IWebHostEnvironment env,
            IStaffRoleService staffRoleService,
            IStaffService staffService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _env = env;
            _staffRoleService = staffRoleService;
            _staffService = staffService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["Error"] = "Invalid login attempt.";
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Welcome back, {user.FullName ?? user.Email}!";
                return user.UserType switch
                {
                    UserType.Admin => RedirectToAction("Index", "AdminDashboard"),
                    UserType.Staff => RedirectToAction("Index", "Staff"),
                    UserType.Customer => RedirectToAction("Index", "CustomerDashboard"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            TempData["Error"] = "Invalid login attempt.";
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                ProfileImageUrl = model.ProfileImageUrl,
                RegisteredAt = DateTime.UtcNow,
                EmailConfirmed = true,
                UserType = UserType.Customer
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View(model);
            }

            // Add role
            if (!await _roleManager.RoleExistsAsync(UserType.Customer.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(UserType.Customer.ToString()));
            }

            await _userManager.AddToRoleAsync(user, UserType.Customer.ToString());

            // Add Claim for Navbar "Welcome"
            await _userManager.AddClaimAsync(user, new Claim("given_name", user.Firstname));

            // Create Customer record with default CustomerType
            _context.Customers.Add(new Customer
            {
                UserId = user.Id,
                CustomerTypeId = (int)DefaultCustomerType.Regular, // Adjust as needed
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Account created successfully. You can now log in.";
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "You've been logged out successfully.";
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth ?? DateTime.MinValue,
                Address = user.Address,
                ProfileImageUrl = user.ProfileImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model, IFormFile ProfileImage)
        {
            if (!ModelState.IsValid) return View("Profile", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.DateOfBirth = model.DateOfBirth;
            user.Address = model.Address;
            user.Gender = model.Gender;

            // Handle image upload
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ProfileImage.FileName)}";
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImageUrl = $"/uploads/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            TempData["Error"] = "Something went wrong while updating your profile.";
            return View("Profile", model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                user.LastPasswordChangeDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                TempData["Success"] = "Your password has been changed successfully.";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(
                "ResetPassword",
                "Account",
                new { userId = user.Id, code = code },
                protocol: Request.Scheme);

            // TODO: Send email with reset link
            // await _emailSender.SendEmailAsync(model.Email, "Reset Password",
            //     $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }

            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                user.LastPasswordChangeDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EnableTwoFactor()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var model = new TwoFactorViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactor(TwoFactorViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
            if (result.Succeeded)
            {
                user.TwoFactorEnabled = true;
                await _userManager.UpdateAsync(user);

                TempData["Success"] = "Two-factor authentication has been enabled.";
                return RedirectToAction("Profile");
            }

            TempData["Error"] = "Failed to enable two-factor authentication.";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactor()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (result.Succeeded)
            {
                user.TwoFactorEnabled = false;
                user.TwoFactorSecret = null;
                await _userManager.UpdateAsync(user);

                TempData["Success"] = "Two-factor authentication has been disabled.";
            }
            else
            {
                TempData["Error"] = "Failed to disable two-factor authentication.";
            }

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(DeleteAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                TempData["Success"] = "Your account has been deleted successfully.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RegisterStaff()
        {
            var staffRoles = await _staffRoleService.GetAllAsync();
            ViewBag.StaffRoles = staffRoles;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStaff(RegisterStaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    ProfileImageUrl = model.ProfileImageUrl,
                    UserType = UserType.Staff


                };

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email is already in use.");
                    return View(model);
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var currentUser = await _userManager.GetUserAsync(User);

                    var staff = new Staff
                    {
                        UserId = user.Id,
                        StaffRoleId = model.StaffRoleId,
                        Bio = model.Bio
                    };

                    EntryHelper.SetCreatedAudit(staff, currentUser.Id);

                    if (!await _roleManager.RoleExistsAsync("Staff"))
                    {
                        ModelState.AddModelError("", "Staff role does not exist.");
                        return await RedisplayForm(model);
                    }

                    await _userManager.AddToRoleAsync(user, "Staff");

                    await _staffService.CreateAsync(staff);
                    _logger.LogInformation("Staff user {Email} created by {AdminId}", user.Email, currentUser.Id);

                    return RedirectToAction("Index", "Staff");
                }

                AddErrors(result);
            }

            return await RedisplayForm(model);
        }

        private async Task<IActionResult> RedisplayForm(RegisterStaffViewModel model)
        {
            model.AvailableRoles = await _staffRoleService.GetAllAsync();
            return View("RegisterStaff", model);
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _logger.LogWarning("Identity error: {Error}", error.Description); // if logger is available
            }
        }



    }
}
