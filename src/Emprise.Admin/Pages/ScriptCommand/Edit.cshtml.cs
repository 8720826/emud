using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.NpcScript;
using Emprise.Domain.Core.Enum;
using Emprise.Domain.Npc.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.ScriptCommand
{
    public class EditModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly IMapper _mapper;

        public EditModel(EmpriseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [BindProperty]
        public ScriptCommandInput ScriptCommand { get; set; }

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public Array Conditions { get; set; }

        public Array Fields { get; set; }

        public Array Relations { get; set; }

        public Array Events { get; set; }

        public Array Commonds { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/ScriptCommand/Index",new { sId = id });
            }

            Conditions = Enum.GetNames(typeof(ConditionTypeEnum));

            Fields = Enum.GetNames(typeof(PlayerConditionFieldEnum));

            Relations = Enum.GetNames(typeof(LogicalRelationTypeEnum));

            Events = Enum.GetNames(typeof(PlayerEventTypeEnum));

            Commonds = Enum.GetNames(typeof(CommondTypeEnum));


            if (id > 0)
            {
                var scriptCommand = await _db.ScriptCommands.FindAsync(id);

                ScriptCommand = _mapper.Map<ScriptCommandInput>(scriptCommand);
                return Page();

            }
            else
            {
                return RedirectToPage("/Script/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            SueccessMessage = "";
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }


            var scriptCommand = await _db.ScriptCommands.FindAsync(id);
            _mapper.Map(ScriptCommand, scriptCommand);
    
            await _db.SaveChangesAsync();



            SueccessMessage = $"修改成功！";

            //return RedirectToPage("/ScriptCommand/Index", new { sId = scriptCommand.ScriptId });

            return Redirect(UrlReferer);
        }
    }
}