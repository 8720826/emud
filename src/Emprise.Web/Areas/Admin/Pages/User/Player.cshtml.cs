using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.User.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Areas.Admin.Pages.User
{
    public class PlayerModel : BaseAdminPageModel
    {

        private readonly IUserAppService _userAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public PlayerModel(
            ILogger<PlayerModel> logger,
            IUserAppService userAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _userAppService = userAppService;
            _appConfig = appConfig.CurrentValue;
        }

        public  UserEntity User { get; set; }
        public List<PlayerEntity> Players { get; set; }

        public async Task OnGetAsync(int userId)
        {
            Players = await _userAppService.GetPlayers(userId);
        }
    }
}