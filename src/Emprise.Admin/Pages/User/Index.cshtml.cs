using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Extensions;
using Emprise.Admin.Models;
using Emprise.Domain.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly IMapper _mapper;

        protected readonly EmpriseDbContext _db;

        public IndexModel(EmpriseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<UserEntity> Paging { get; set; }

        public void OnGet(int pageIndex)
        {
            var query = _db.Users.OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = _db.Users.Where(x => x.Email.Contains(Keyword)).OrderBy(x => x.Id);
            }

            Paging = query.Paged(pageIndex, 10, query.Count());


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