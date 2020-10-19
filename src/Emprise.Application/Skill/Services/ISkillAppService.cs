using Emprise.Application.Skill.Dtos;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Skill.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Skill.Services
{
    public interface ISkillAppService :IBaseService
    {
        Task<SkillEntity> Get(int id);

        Task<ResultDto> Add(SkillInput item);

        Task<ResultDto> Update(int id, SkillInput item);

        Task<ResultDto> Delete(int id);

        Task<Paging<SkillEntity>> GetPaging(string keyword, int pageIndex);
    }
}
