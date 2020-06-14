using AutoMapper;
using Emprise.Admin.Models.Npc;
using Emprise.Admin.Models.NpcScript;
using Emprise.Admin.Models.Room;
using Emprise.Admin.Models.Script;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emprise.Domain.ItemDrop.Entity;
using Emprise.Domain.Room.Entity;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Ware.Entity;
using Emprise.Domain.Map.Entity;
using Emprise.Domain.Email.Entity;
using Emprise.Domain.Player.Entity;
using Emprise.Application.Ware.Dtos;
using Emprise.Application.Player.Dtos;
using Emprise.Application.Quest.Dtos;
using Emprise.Application.Map.Dtos;
using Emprise.Application.Email.Dtos;
using Emprise.Application.ItemDrop.Dtos;

namespace Emprise.Admin.Mapper
{

    public class ModelProfile : Profile
    {
        public ModelProfile()
        {


            CreateMap<RoomEntity, RoomInput>();

            CreateMap<RoomInput, RoomEntity>()
                .ForMember(x => x.EastName, y => y.Ignore())
                .ForMember(x => x.WestName, y => y.Ignore())
                .ForMember(x => x.SouthName, y => y.Ignore())
                .ForMember(x => x.NorthName, y => y.Ignore());

            CreateMap<QuestEntity, QuestInput>();
            CreateMap<QuestInput, QuestEntity>();

            CreateMap<ScriptEntity, ScriptInput>()
                ;

            CreateMap<ScriptInput, ScriptEntity>()
                ;

            CreateMap<NpcEntity, NpcInput>();
            CreateMap<NpcInput, NpcEntity>();

            CreateMap<NpcEntity, NpcModel>();


            CreateMap<ScriptCommandEntity, ScriptCommandInput>();

            CreateMap<ScriptCommandInput, ScriptCommandEntity>();

            CreateMap<WareInput, WareEntity>();
            CreateMap<WareEntity, WareInput>();

            CreateMap<MapInput, MapEntity>();
            CreateMap<MapEntity, MapInput>();

            CreateMap<PlayerInput, PlayerEntity>();
            CreateMap<PlayerEntity, PlayerInput>();

            CreateMap<EmailInput, EmailEntity>();
            CreateMap<EmailEntity, EmailInput>();

            CreateMap<ItemDropInput, ItemDropEntity>();
            CreateMap<ItemDropEntity, ItemDropInput>();
            
        }
    }
}
