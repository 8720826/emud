using AutoMapper;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Log.Services
{

    public class OperatorLogAppService : BaseAppService, IOperatorLogAppService
    {
        private readonly IMapper _mapper;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public OperatorLogAppService(IMapper mapper, IOperatorLogDomainService operatorLogDomainService, IUnitOfWork uow) 
            : base(uow)
        {
            _mapper = mapper;
            _operatorLogDomainService = operatorLogDomainService;
        }



    }
}
