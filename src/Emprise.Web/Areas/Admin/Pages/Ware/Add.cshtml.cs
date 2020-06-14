using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Ware.Dtos;
using Emprise.Application.Ware.Services;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Web.Areas.Admin.Pages.Ware
{
    public class AddModel : BaseAdminPageModel
    {

        private readonly IWareAppService _wareAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public AddModel(
            ILogger<AddModel> logger,
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

        public async Task OnGetAsync()
        {
  
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
                await _wareAppService.Add(Ware);

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.添加物品,
                    Content = JsonConvert.SerializeObject(Ware)
                });
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.添加物品,
                    Content = $"Data={JsonConvert.SerializeObject(Ware)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }



            return RedirectToPage("/Ware/Index");
            */
        }
    }
}