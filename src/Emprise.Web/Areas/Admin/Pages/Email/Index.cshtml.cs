using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Email.Services;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Email.Entity;
using Emprise.Infra.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Areas.Admin.Pages.Email
{
    public class IndexModel : BaseAdminPageModel
    {
        private readonly IEmailAppService _emailAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public IndexModel(
            ILogger<IndexModel> logger,
            IEmailAppService emailAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _emailAppService = emailAppService;
            _appConfig = appConfig.CurrentValue;

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<EmailEntity> Paging { get; set; }

        public async Task OnGetAsync(int pageIndex)
        {
            Paging = await _emailAppService.GetPaging(Keyword, pageIndex);
        }


    }
}