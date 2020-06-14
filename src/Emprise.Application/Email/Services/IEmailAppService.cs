using Emprise.Application.Email.Dtos;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Email.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Email.Services
{
    public interface IEmailAppService : IBaseService
    {
        Task<EmailEntity> Get(int id);

        Task<ResultDto> Add(EmailInput item);

        Task<ResultDto> Update(int id, EmailInput item);

        Task<ResultDto> Delete(int id);

        Task<Paging<EmailEntity>> GetPaging(string keyword, int pageIndex);
    }
}
