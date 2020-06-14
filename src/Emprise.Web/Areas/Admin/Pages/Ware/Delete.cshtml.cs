using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Ware.Services;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Ware.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Web.Areas.Admin.Pages.Ware
{
    public class DeleteModel : BaseAdminPageModel
    {
        private readonly IWareAppService _wareAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public DeleteModel(
            ILogger<DeleteModel> logger,
            IWareAppService wareAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _wareAppService = wareAppService;
            _appConfig = appConfig.CurrentValue;

        }

        public WareEntity Ware { get; set; }

        public string ErrorMessage { get; set; }


        public async  Task<IActionResult> OnGetAsync(int id = 0)
        {

            if (id > 0)
            {
                Ware = await _wareAppService.Get(id);
                if (Ware == null)
                {
                    return RedirectToPage("/Ware/Index");
                }
                return Page();
            }
            else
            {
                return RedirectToPage("/Ware/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id = 0)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var result = await _wareAppService.Delete(id);
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
                var ware = await _db.Wares.FindAsync(id);
                if (ware == null)
                {
                    ErrorMessage = $"物品 {id} 不存在！";
                    return Page();
                }
                _db.Wares.Remove(ware);
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.删除物品,
                    Content = JsonConvert.SerializeObject(ware)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.删除物品,
                    Content = $"id={id}，ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
            */
        }
    }
}