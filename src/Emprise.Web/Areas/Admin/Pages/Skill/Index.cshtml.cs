using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.Skill.Services;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Skill.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Emprise.Web.Areas.Admin.Pages.Skill
{
    public class IndexModel : BaseAdminPageModel
    {
        private readonly ISkillAppService _skillAppService;
        public IndexModel(
            ILogger<IndexModel> logger,
            ISkillAppService skillAppService)
         : base(logger)
        {
            _skillAppService = skillAppService;
        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<SkillEntity> Paging { get; set; }

        public async Task OnGetAsync(int pageIndex)
        {
            Paging = await _skillAppService.GetPaging(Keyword, pageIndex);
        }
    }
}
