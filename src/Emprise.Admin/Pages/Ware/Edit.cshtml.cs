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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Ware
{
    public class EditModel : BasePageModel
    {
        private readonly IWareAppService _wareAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public EditModel(
            ILogger<EditModel> logger,
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

        public async Task<IActionResult> OnPostAsync(int id)
        {

            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _wareAppService.Update(id, Ware);
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
                
                var ware = await _wareAppService.Get(id);
                if (ware == null)
                {
                    ErrorMessage = $"物品 {id} 不存在！";
                    return Page();
                }
                var content = ware.ComparisonTo(Ware);
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
            */
        }
    }
}