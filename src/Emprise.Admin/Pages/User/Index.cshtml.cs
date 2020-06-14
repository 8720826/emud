using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Models;
using Emprise.Domain.User.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.User
{
    public class IndexModel : BasePageModel
    {
        private readonly IUserAppService _userAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public IndexModel(
            ILogger<IndexModel> logger,
            IUserAppService userAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _userAppService = userAppService;
            _appConfig = appConfig.CurrentValue;

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<UserEntity> Paging { get; set; }

        public async Task OnGetAsync(int pageIndex)
        {
            Paging = await _userAppService.GetPaging(Keyword,pageIndex);
        }


        public async Task<IActionResult> OnPostAsync([FromBody]EnableData enableData)
        {
            await _userAppService.SetEnabled(enableData.SId, enableData.IsEnable);


            return await Task.FromResult(new JsonResult(enableData));

        }

        public class EnableData
        {
            public int SId { get; set; }
            public bool IsEnable { get; set; }
        }
    }
}