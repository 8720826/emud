using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Extensions;
using Emprise.Admin.Models;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Email
{
    public class IndexModel : BasePageModel
    {
        public IndexModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<IndexModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<EmailEntity> Paging { get; set; }

        public void OnGet(int pageIndex)
        {
            var query = _db.Emails.OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = _db.Emails.Where(x => x.Title.Contains(Keyword)|| x.Content.Contains(Keyword)).OrderBy(x => x.Id);
            }

            Paging = query.Paged(pageIndex, 10, query.Count());


        }


    }
}