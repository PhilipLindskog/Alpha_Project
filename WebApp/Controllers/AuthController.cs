using Business.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

    [Route("auth/signup")]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    [Route("auth/signup")]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        if (ModelState.IsValid)
        {
            var data = new SignUpFormData
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
            };
            
            


            //var formData = model.MapTo<SignUpFormData>();
            var result = await _authService.SignUpAsync(data);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        return View(model);
    }

    [Route("auth/login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    [Route("auth/login")]
    public IActionResult Login(string returnUrl = null!)
    {
        ViewBag.ReturnUrl = returnUrl ?? Url.Content("~/");
        return View();
    }

    [HttpPost]
    [Route("auth/login")]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null!)
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl;


        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            returnUrl = Url.Content("~/");
        }


        if (ModelState.IsValid)
        {
            var formData = model.MapTo<SignInFormData>();
            var result = await _authService.SignInAsync(formData);

            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
                
            ViewBag.ErrorMessage = "Incorrect email or password";
        }
       

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return LocalRedirect("~/");
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
