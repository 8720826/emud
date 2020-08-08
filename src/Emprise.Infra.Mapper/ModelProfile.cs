using AutoMapper;
using Emprise.Application.Email.Dtos;
using Emprise.Application.ItemDrop.Dtos;
using Emprise.Application.Map.Dtos;
using Emprise.Application.Npc.Dtos;
using Emprise.Application.Quest.Dtos;
using Emprise.Application.Room.Models;
using Emprise.Application.User.Models;
using Emprise.Application.Ware.Dtos;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Email.Entity;
using Emprise.Domain.ItemDrop.Entity;
using Emprise.Domain.Map.Entity;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Models;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Quest.Models;
using Emprise.Domain.Room.Entity;
using Emprise.Domain.Room.Models;
using Emprise.Domain.User.Entity;
using Emprise.Domain.Ware.Entity;
using Emprise.Domain.Ware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emprise.Infra.Mapper
{

    public class ModelProfile : Profile
    {
        public ModelProfile()
        {


            CreateMap<UserEntity, UserModel>();
            CreateMap<RoomEntity, RoomModel>();
          

            CreateMap<PlayerEntity, MyInfo>()
                .ForMember(x => x.Age, y => y.MapFrom(s => s.Age.ToAge()))
                .ForMember(x => x.Money, y => y.MapFrom(s => s.Money.ToMoney()))
                .ForMember(x => x._int, y => y.MapFrom(s => s.Int));


            CreateMap<WareEntity, WareModel>();
            CreateMap<WareEntity, Weapon>();
            CreateMap<WareEntity, WareInput>();
            CreateMap<WareInput, WareEntity>();


            CreateMap<EmailEntity, EmailInput>();
            CreateMap<EmailInput, EmailEntity>();


            CreateMap<NpcEntity, NpcInput>();
            CreateMap<NpcInput, NpcEntity>();

            CreateMap<ItemDropRateEntity, ItemDropRateInput>();
            CreateMap<ItemDropRateInput, ItemDropRateEntity>();

            CreateMap<QuestEntity, QuestModel>();
            CreateMap<QuestEntity, QuestDetailModel>();

            CreateMap<QuestEntity, QuestInput>();
            CreateMap<QuestInput, QuestEntity>();


            CreateMap<MapEntity, MapInput>();
            CreateMap<MapInput, MapEntity>();

            CreateMap<RoomEntity, RoomInput>();
            CreateMap<RoomInput, RoomEntity>();
        }
    }
}
