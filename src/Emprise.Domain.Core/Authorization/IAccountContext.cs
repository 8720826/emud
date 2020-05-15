using Emprise.Domain.Core.Interfaces.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Authorization
{
    public interface IAccountContext: IScoped
    {
        /// <summary>
        /// 登录ID
        /// </summary>
        int UserId { get;  }

        /// <summary>
        /// 登录名
        /// </summary>
        string Email { get;  }

        /// <summary>
        /// 聊天id
        /// </summary>
        string ConnectionId { get;  }

        /// <summary>
        /// 角色id
        /// </summary>
        int PlayerId { get;  }


        /// <summary>
        /// 角色名
        /// </summary>
        string PlayerName { get;  }

        /// <summary>
        /// 当前房间
        /// </summary>
         int RoomId { get; }

        /// <summary>
        /// 当前门派
        /// </summary>
        int FactionId { get; }


    }
}
