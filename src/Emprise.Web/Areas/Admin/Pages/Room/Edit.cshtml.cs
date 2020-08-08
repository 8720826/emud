using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Room.Models;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Emprise.Web.Areas.Admin.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Room
{
    public class EditModel : BaseAdminPageModel
    {
        private readonly IRoomAppService _roomAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public EditModel(
            ILogger<EditModel> logger,
            IRoomAppService roomAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _roomAppService = roomAppService;
            _appConfig = appConfig.CurrentValue;

        }

        public int Id { get; set; }



        [BindProperty]
        public RoomInput Room { get; set; }

        public int MapId { get; set; }

        public string ErrorMessage { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {

            if (id > 0)
            {
                Id = id;
                var room = await _roomAppService.Get(id);

                Room = _mapper.Map<RoomInput>(room);

                MapId = room.MapId;

                return Page();
            }
            else
            {
                return RedirectToPage("/Room/Index");
            }
        }


        public async Task<IActionResult> OnPostAsync(int id)
        {
            Id = id;
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _roomAppService.Update(id, Room);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Map/Index");
            }

        }
    }
}