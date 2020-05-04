using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Extensions;
using Emprise.Admin.Models;
using Emprise.Admin.Models.Admin;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace EmpriseAdmin.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly IMapper _mapper;
        private readonly AppConfig _appConfig;
        public LoginModel(EmpriseDbContext db, IMapper mapper, IOptionsMonitor<AppConfig> appConfig)
        {
            _db = db;
            _mapper = mapper;
            _appConfig = appConfig.CurrentValue;
        }

        public bool HasSetAdmin { get; set; }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public LoginInput LoginInput { get; set; }

        public void OnGet()
        {
            HasSetAdmin = _db.Admins.Count() != 0;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }

            AdminEntity admin = null;
            if (_db.Admins.Count() == 0)
            {
                admin = new AdminEntity
                {
                    Name = LoginInput.Name,
                    Password = LoginInput.Password.ToMd5()
                };
                _db.Admins.Add(admin);
                await _db.SaveChangesAsync();
            }
            else
            {
                admin = _db.Admins.FirstOrDefault(x => x.Name == LoginInput.Name);
                if (admin == null)
                {
                    ErrorMessage = "账号或密码错误";
                    return Page();
                }

                if (admin.Password != LoginInput.Password.ToMd5())
                {
                    ErrorMessage = "账号或密码错误";
                    return Page();
                }
            }

            var claims = new[] { new Claim("Name", admin.Name) };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal user = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user, new AuthenticationProperties { AllowRefresh = true, ExpiresUtc = DateTimeOffset.Now.AddDays(30) });

            return RedirectToPage("Index");
        }
    }
}