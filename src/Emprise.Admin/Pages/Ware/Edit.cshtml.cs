using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Ware;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Ware.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Ware
{
    public class EditModel : BasePageModel
    {
        public EditModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<EditModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {
            Endpoint = _appConfig.Aliyun.Endpoint;
            AliyunOssHost = _appConfig.Aliyun.AliyunOssHost;
        }

        [BindProperty]
        public WareInput Ware { get; set; }

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

            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var ware = await _db.Wares.FindAsync(id);
                if (ware == null)
                {
                    ErrorMessage = $"物品 {id} 不存在！";
                    return Page();
                }
                var content = DifferenceComparison(ware, Ware);
                _mapper.Map(Ware, ware);

                if (ware.Effect == null)
                {
                    ware.Effect = "";
                }
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改物品,
                    Content = $"Id = {id},Data = {content}"
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改物品,
                    Content = $"Data={JsonConvert.SerializeObject(Ware)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }



            return Redirect(UrlReferer);
        }
    }
}