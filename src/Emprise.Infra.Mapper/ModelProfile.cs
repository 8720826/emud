using AutoMapper;
using Emprise.Application.User.Models;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Models;
using Emprise.Domain.Room.Entity;
using Emprise.Domain.Room.Models;
using Emprise.Domain.User.Entity;
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
                .ForMember(x => x._int, y => y.MapFrom(s => s.Int))
                .ForMember(x => x.Auths, y => y.MapFrom(s => string.IsNullOrEmpty(s.Auths)?new List<string>(): s.Auths.Split(',', StringSplitOptions.None).ToList()));
        }
    }
}
