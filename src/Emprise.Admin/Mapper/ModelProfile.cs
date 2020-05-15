using AutoMapper;
using Emprise.Admin.Models.Npc;
using Emprise.Admin.Models.NpcScript;
using Emprise.Admin.Models.Room;
using Emprise.Admin.Models.Script;
using Emprise.Admin.Models.Quest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emprise.Admin.Models.Ware;
using Emprise.Admin.Entity;
using Emprise.Admin.Models.Map;
using Emprise.Admin.Models.Player;

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
        }
    }
}
