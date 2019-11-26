using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.User.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Log.Services
{
    public interface IChatLogDomainService : IBaseService
    {

        Task Add(ChatLogEntity user);

    }
}
