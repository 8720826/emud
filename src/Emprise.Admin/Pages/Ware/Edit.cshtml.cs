using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Ware;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Ware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Ware
{
    public class EditModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly IMapper _mapper;
        private readonly AppConfig _appConfig;
        public EditModel(EmpriseDbContext db, IMapper mapper, IOptionsMonitor<AppConfig> appConfig)
        {
            _db = db;
            _mapper = mapper;
            _appConfig = appConfig.CurrentValue;


        }

        [BindProperty]
        public WareInput Ware { get; set; }

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }


        public string Endpoint { get; set; }

        public string AliyunOssHost { get; set; }

        public List<WareEffect> WareEffects { get; set; } = new List<WareEffect>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Ware/Index");
            }


            if (id > 0)
            {
                Endpoint = _appConfig.Aliyun.Endpoint;
                AliyunOssHost = _appConfig.Aliyun.AliyunOssHost;


                var ware = await _db.Wares.FindAsync(id);

                Ware = _mapper.Map<WareInput>(ware);

                if (!string.IsNullOrEmpty(ware.Effect))
                {
                    WareEffects = JsonConvert.DeserializeObject<List<WareEffect>>(ware.Effect);
                }

                return Page();
            }
            else
            {
                return RedirectToPage("/Ware/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            SueccessMessage = "";
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }

            var ware = await _db.Wares.FindAsync(id);
            _mapper.Map(Ware, ware);

            if (ware.Effect == null)
            {
                ware.Effect = "";
            }



            await _db.SaveChangesAsync();



            SueccessMessage = $"添加成功！";

            return Redirect(UrlReferer);
        }
    }
}