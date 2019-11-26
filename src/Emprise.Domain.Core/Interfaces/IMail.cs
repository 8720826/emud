using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Interfaces
{
    public  interface IMail: ISingleton
    {
        Task Send(MailModel message);
    }
}
