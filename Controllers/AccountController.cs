using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Salon360App.Data;
using Salon360App.Enums;
using Salon360App.Models;
using Salon360App.Services;
using Salon360App.Services.Interfaces;
using Salon360App.ViewModels.AccountViewModels;
using Salon360App.ViewModels.LoginViewModels;
using Salon360App.ViewModels.ProfileViewModels;
using System.Security.Claims;
using System.Transactions;

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
            var customer = new Customer
            {
                UserId = user.Id,
                CustomerTypeId = (int)DefaultCustomerType.Regular, // Adjust as needed
            };

            // For new registrations, we don't have a current user ID since they're anonymous
            // So we'll use the newly created user's ID as the creator
            EntryHelper.SetCreatedAudit(customer, user.Id);

            _context.Customers.Add(customer);
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
            try
            {
                _logger.LogInformation("Starting staff registration process");
                var staffRoles = await _staffRoleService.GetAllAsync();
                var model = new RegisterStaffViewModel
                {
                    AvailableRoles = staffRoles.Select(r => new SelectListItem
                    {
                        Value = r.StaffRoleId.ToString(),
                        Text = r.Name
                    })
                };
                _logger.LogInformation("Staff registration form loaded successfully with {Count} available roles", staffRoles.Count());
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff registration form");
                TempData["Error"] = "An error occurred while loading the registration form";
                return RedirectToAction("Index", "Staff");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStaff(RegisterStaffViewModel model)
        {
            _logger.LogInformation("Starting staff registration for email: {Email}", model.Email);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Invalid model state for staff registration: {Email}. Errors: {Errors}",
                    model.Email, string.Join(", ", errors));

                // Log each field's state
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        _logger.LogWarning("Field {Field} validation errors: {Errors}",
                            key, string.Join(", ", state.Errors.Select(e => e.ErrorMessage)));
                    }
                }

                return await RedisplayForm(model);
            }

            // Validate StaffRole exists
            var staffRole = await _staffRoleService.GetByIdAsync(model.StaffRoleId);
            if (staffRole == null)
            {
                _logger.LogWarning("Invalid staff role selected: {StaffRoleId}", model.StaffRoleId);
                ModelState.AddModelError("StaffRoleId", "Selected role does not exist");
                return await RedisplayForm(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Handle profile image upload
                string profileImageUrl = null;
                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    _logger.LogInformation("Processing profile image upload for {Email}", model.Email);

                    if (model.ProfileImage.Length > 5 * 1024 * 1024) // 5MB limit
                    {
                        _logger.LogWarning("Profile image too large for {Email}: {Size} bytes", model.Email, model.ProfileImage.Length);
                        ModelState.AddModelError("ProfileImage", "Image size cannot exceed 5MB");
                        return await RedisplayForm(model);
                    }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(model.ProfileImage.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        _logger.LogWarning("Invalid file extension for {Email}: {Extension}", model.Email, fileExtension);
                        ModelState.AddModelError("ProfileImage", "Only JPG, JPEG, PNG, and GIF files are allowed");
                        return await RedisplayForm(model);
                    }

                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadPath))
                    {
                        _logger.LogInformation("Creating upload directory: {Path}", uploadPath);
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImage.CopyToAsync(stream);
                    }
                    profileImageUrl = $"/uploads/{fileName}";
                    _logger.LogInformation("Profile image uploaded successfully for {Email}: {Path}", model.Email, profileImageUrl);
                }

                // Create user
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    ProfileImageUrl = profileImageUrl,
                    UserType = UserType.Staff,
                    RegisteredAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                _logger.LogInformation("Creating user account for {Email}", model.Email);
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to create user account for {Email}: {Errors}",
                        model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    AddErrors(result);
                    await transaction.RollbackAsync();
                    return await RedisplayForm(model);
                }

                // Add to Staff role
                if (!await _roleManager.RoleExistsAsync("Staff"))
                {
                    _logger.LogInformation("Creating Staff role");
                    await _roleManager.CreateAsync(new IdentityRole<int>("Staff"));
                }
                await _userManager.AddToRoleAsync(user, "Staff");
                _logger.LogInformation("Added {Email} to Staff role", model.Email);

                // Create staff record
                var staff = new Staff
                {
                    UserId = user.Id,
                    StaffRoleId = model.StaffRoleId,
                    Bio = model.Bio,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Creating staff record for {Email}", model.Email);
                if (!await _staffService.CreateAsync(staff))
                {
                    _logger.LogError("Failed to create staff record for {Email}", model.Email);
                    ModelState.AddModelError("", "Failed to create staff record");
                    await transaction.RollbackAsync();
                    return await RedisplayForm(model);
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Staff member {Email} registered successfully", model.Email);
                TempData["Success"] = "Staff member registered successfully";
                return RedirectToAction("Index", "Staff");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering staff member {Email}", model.Email);
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "An error occurred while registering the staff member");
                return await RedisplayForm(model);
            }
        }

        private async Task<IActionResult> RedisplayForm(RegisterStaffViewModel model)
        {
            try
            {
                _logger.LogInformation("Redisplaying registration form for {Email}", model.Email);
                var staffRoles = await _staffRoleService.GetAllAsync();
                model.AvailableRoles = staffRoles.Select(r => new SelectListItem
                {
                    Value = r.StaffRoleId.ToString(),
                    Text = r.Name
                });
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redisplaying registration form for {Email}", model.Email);
                TempData["Error"] = "An error occurred while loading the form";
                return RedirectToAction("Index", "Staff");
            }
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
