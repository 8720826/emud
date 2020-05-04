using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Helper;
using Emprise.Admin.Models;
using Emprise.Admin.Models.Ware;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Ware
{
    public class AddModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly IMapper _mapper;
        private readonly AppConfig _appConfig;
        public AddModel(EmpriseDbContext db, IMapper mapper, IOptionsMonitor<AppConfig> appConfig)
        {
            _db = db;
            _mapper = mapper;
            _appConfig = appConfig.CurrentValue;

            Endpoint = _appConfig.Aliyun.Endpoint;
        }

        [BindProperty]
        public WareInput Ware { get; set; }

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }


        public string Endpoint { get; set; }

        public async Task OnGetAsync()
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Ware/Index");
            }


        }

        public async Task<IActionResult> OnPostAsync()
        {
            SueccessMessage = "";
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }

            var ware = _mapper.Map<WareEntity>(Ware);

            if (ware.Effect == null)
            {
                ware.Effect = "";
            }

            await _db.Wares.AddAsync(ware);

            await _db.SaveChangesAsync();



            SueccessMessage = $"添加成功！";

            return Redirect(UrlReferer);
        }
    }
}