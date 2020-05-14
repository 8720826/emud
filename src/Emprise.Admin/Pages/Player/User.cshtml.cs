using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Domain.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.Player
{
    public class UserModel : PageModel
    {
        private readonly IMapper _mapper;

        protected readonly EmpriseDbContext _db;

        public UserModel(EmpriseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public UserEntity User { get; set; }

        public async Task OnGetAsync(int id)
        {
            User = await _db.Users.FindAsync(id);

        }


        public async Task<IActionResult> OnPostAsync([FromBody]EnableData enableData)
        {
            try
            {
                var user = await _db.Users.FindAsync(enableData.SId);
                if (user == null)
                {
                    return await Task.FromResult(new JsonResult(enableData));
                }

                user.Status = enableData.IsEnable ? UserStatusEnum.正常 : UserStatusEnum.封禁;
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "");
            }


            return await Task.FromResult(new JsonResult(enableData));

        }

        public class EnableData
        {
            public int SId { get; set; }
            public bool IsEnable { get; set; }
        }
    }
}