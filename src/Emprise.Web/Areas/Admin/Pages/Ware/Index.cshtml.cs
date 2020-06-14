using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.Ware.Services;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Ware.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Emprise.Web.Areas.Admin.Pages.Ware
{
    public class IndexModel : BaseAdminPageModel
    {
        private readonly IWareAppService _wareAppService;
        public IndexModel(
            ILogger<IndexModel> logger,
            IWareAppService wareAppService)
         : base(logger)
        {
            _wareAppService = wareAppService;
        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<WareEntity> Paging { get; set; }

        public async Task OnGetAsync(int pageIndex)
        {
            Paging = await _wareAppService.GetPaging(Keyword, pageIndex);
        }
    }
}