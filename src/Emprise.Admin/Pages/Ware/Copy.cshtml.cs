using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Ware.Dtos;
using Emprise.Application.Ware.Services;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Ware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Ware
{
    public class CopyModel : BasePageModel
    {
        private readonly IWareAppService _wareAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public CopyModel(
            ILogger<CopyModel> logger,
            IWareAppService wareAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _wareAppService = wareAppService;
            _appConfig = appConfig.CurrentValue;

            Endpoint = _appConfig.Aliyun.Endpoint;
            AliyunOssHost = _appConfig.Aliyun.AliyunOssHost;
        }

        [BindProperty]
        public WareInput Ware { get; set; }


        public string ErrorMessage { get; set; }

        public string Endpoint { get; set; }

        public string AliyunOssHost { get; set; }

        public List<WareEffect> WareEffects { get; set; } = new List<WareEffect>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id > 0)
            {
                var ware = await _wareAppService.Get(id);
                if (ware == null)
                {
                    return RedirectToPage("/Ware/Index");
                }
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

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _wareAppService.Add(Ware);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Ware/Index");
            }
            /*
            try
            {
                var ware = _mapper.Map<WareEntity>(Ware);

                if (ware.Effect == null)
                {
                    ware.Effect = "";
                }
                await _db.Wares.AddAsync(ware);

                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.复制物品,
                    Content = JsonConvert.SerializeObject(Ware)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.复制物品,
                    Content = $"Data={JsonConvert.SerializeObject(Ware)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }


            return Redirect(UrlReferer);
            */
        }
    }
}