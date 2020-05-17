using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.User
{
    public class PlayerModel : BasePageModel
    {

        public PlayerModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<PlayerModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }

        [BindProperty]
        public string UrlReferer { get; set; }

        public  UserEntity User { get; set; }
        public List<PlayerEntity> Players { get; set; }

        public async Task OnGetAsync(int userId,string @ref)
        {
            UrlReferer = @ref;
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Request.Headers["Referer"].ToString();
            }
            User = await _db.Users.FindAsync(userId);

            var query = _db.Players.Where(x=>x.UserId== userId).OrderBy(x => x.Id);


            Players = query.ToList();


        }
    }
}