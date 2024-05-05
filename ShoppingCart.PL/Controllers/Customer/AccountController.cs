using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Models;
using ShoppingCart.PL.Helpers.interfaces;
using ShoppingCart.PL.ViewModels;

using ShoppingCart.PL.Controllers.Admin;

namespace ShoppingCart.PL.Controllers.Customer
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IEmailSettings _emailSettings;


        #region Constuctor
        public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUnitOfWork unitOfWork,
        IEmailSettings emailSettings
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _emailSettings = emailSettings;
        }
        #endregion


        #region Sign In 

        public async Task<IActionResult> SignIn(string? ReturnUrl = null)
        {
            var model = new SignInViewModel
            {
                ReturnUrl = ReturnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(SignInViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);

                if (user is not null)
                {
                    var checkAtPassword = await _userManager.CheckPasswordAsync(user, viewModel.Password);

                    if (checkAtPassword == true)
                    {
                        var checkSignIn = await _signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, false);

                        if (checkSignIn.Succeeded)
                            return RedirectToAction(nameof(CategoryController.Index), "Home");

                    }
                }
                else
                    ModelState.AddModelError(string.Empty, "Invalid Login");
            }

            return View(viewModel);
        }

        #endregion


        #region Sign Up
        public IActionResult SignUp() => View();

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);

                if (user == null)
                {
                    var identityUser = new ApplicationUser
                    {
                        FirstName = viewModel.FirstName,

                        LastName = viewModel.LastName,

                        UserName = viewModel.UserName,

                        Email = viewModel.Email,

                        Address = viewModel.Address,

                        City = viewModel.City,

                        Phone = viewModel.Phone
                    };

                    var resultOfCreatingUserIdentity =
                        await _userManager.CreateAsync(identityUser, viewModel.Password);

                    if (resultOfCreatingUserIdentity.Succeeded == true)
                    {
                        await _signInManager.SignInAsync(identityUser, isPersistent: false);

                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }

                    else
                        foreach (var error in resultOfCreatingUserIdentity.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);


                }

                else
                    ModelState.AddModelError(string.Empty, "User name is already Exist ");
            }

            return View();



        }

        #endregion


        #region Sign Out

        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
        #endregion


        #region Forget Password
        public async Task<IActionResult> ForgetPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // 1. I must make check that Email is Exist . , Then Get User of This Email 
                var appUser = await _userManager.FindByEmailAsync(viewModel.Email);

                if (appUser is not null)
                {
                    // Note When i make Forget Password , There is Link Send To me At This Email .
                    // This Link Contain Some Info : [Token]


                    // 1 => Generate Token For This user. [Token For Reset password for this user]
                    //   => this token contain some info about user. 
                    var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);


                    // 2. here I need to send [Action && Controller && Email && Token] At Link ,
                    // When User Clink At this link => he will go At  Some Action . 
                    // here i make anonymoust object , Then i send The Data to 
                    var resetPasswordLink = Url.Action(nameof(ResetPassword), "Account",
                        new
                        {
                            Email = viewModel.Email,
                            Token = token
                        },
                        Request.Scheme); // Send http Request [Type Of Request  || The Request That i use]


                    var email = new Email()
                    {
                        Subject = "Reset Password",
                        Body = resetPasswordLink,
                        To = viewModel.Email
                    };

                    _emailSettings.SendEmail(email);

                    return RedirectToAction("CompleteForgetPassword");
                }

                ModelState.AddModelError(string.Empty, "Invalid Email");
            }

            return View(viewModel);
        }

        #endregion


        #region Complete Forget Password || Reset Password

        public async Task<IActionResult> CompleteForgetPassword() => View();

        // when i open reset password view , i have 2 info [Email && token]
        // he take this value from root || from link [from url].
        public async Task<IActionResult> ResetPassword(string email, string token) // will take email and Token
                => View();

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel) // => i need to make view model for reset password 
        {
            if (ModelState.IsValid)
            {
                // At First I have Email 
                var appUser = await _userManager.FindByEmailAsync(viewModel.Email);

                if (appUser is not null)
                {
                    // i will call Function that make Reset for Password [Built In Function]
                    var result = await _userManager.ResetPasswordAsync(appUser, viewModel.Token, viewModel.Password);

                    if (result.Succeeded)
                        return RedirectToAction(nameof(SignIn));

                    else
                        foreach (var error in result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(viewModel);

        }

        #endregion


        #region User name Exist ? 
        public async Task<bool> UserNameExist(string userName)
        {
            var userNameExist = await _userManager.FindByNameAsync(userName);

            if (userNameExist is null)
                return false;
            else
                return true;


        }
        #endregion


        #region External Login       
        public IActionResult GoogleSignIn()
        {
            // i need to make varaible from [atuhproperty ] ,this preoperty make redirect at url [Another Action.]. 
            // Google Response Action 

            var authProp = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            // here i make return challange 
            return Challenge(authProp, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme); // Get Auth Async.

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                // There Are 4 [claim issure , claim original , cliam type , claim value , ]
                // becuase at last i willl return claim . 
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });


            return RedirectToAction("Index", "Home");
        }

        //public IActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
        //        new { ReturnUrl = returnUrl });
        //    var properties = _signInManager
        //        .ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //    return new ChallengeResult(provider, properties);
        //}
        #endregion



        #region External Login [Generic]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            //This call will generate a URL that directs to the ExternalLoginCallback action method in the Account controller
            //with a route parameter of ReturnUrl set to the value of returnUrl.
            var redirectUrl = Url.Action(action: "ExternalLoginCallback", controller: "Account", values: new { ReturnUrl = returnUrl });

            // Configure the redirect URL, provider and other properties
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            //This will redirect the user to the external provider's login page
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl, string? remoteError)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var loginViewModel = new SignInViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", loginViewModel);
            }

            // If the user already has a login (i.e., if there is a record in AspNetUserLogins table)
            // then sign-in the user with this external login provider
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            // If there is no record in AspNetUserLogins table, the user may not have a local account
            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                        };

                        //This will create a new user into the AspNetUsers table without password
                        await _userManager.CreateAsync(user);
                    }

                    // Add a login (i.e., insert a row for the user in AspNetUserLogins table)
                    await _userManager.AddLoginAsync(user, info);

                    //Then Signin the User
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on info@dotnettutorials.net";

                return View("Error");
            }
        }

        #endregion
        public void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
