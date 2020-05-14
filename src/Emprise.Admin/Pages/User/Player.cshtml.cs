using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.User
{
    public class PlayerModel : PageModel
    {
        private readonly IMapper _mapper;

        protected readonly EmpriseDbContext _db;

        public PlayerModel(EmpriseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
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