using AutoMapper;
using Emprise.Application.Admin.Dtos;
using Emprise.Domain.Admin.Entity;
using Emprise.Domain.Admin.Services;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Emprise.Infra.Authorization;

namespace Emprise.Application.Admin.Services
{
    public class AdminAppService : BaseAppService, IAdminAppService
    {
        private readonly IMapper _mapper;
        private readonly IAdminDomainService _adminDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        private readonly IHttpContextAccessor _httpAccessor;
        public AdminAppService(
            IMapper mapper,
            IAdminDomainService adminDomainService,
            IHttpContextAccessor httpAccessor,
            IUnitOfWork uow,
            IOperatorLogDomainService operatorLogDomainService)
            : base(uow)
        {
            _mapper = mapper;
            _adminDomainService = adminDomainService;
            _operatorLogDomainService = operatorLogDomainService;
            _httpAccessor = httpAccessor;
        }

        public async Task<int> GetCount()
        {
            var query = await _adminDomainService.GetAll();
            return await query.CountAsync();
        }

        public async Task<ResultDto> Login(LoginInput input)
        {
            var result = new ResultDto { Message = "" };

            try
            {
                var query = await _adminDomainService.GetAll();
                var adminCount = await query.CountAsync();

                if (adminCount == 0)
                {
                    var admin = new AdminEntity
                    {
                        Name = input.Name,
                        Password = input.Password.ToMd5()
                    };

                    await _adminDomainService.Add(admin);

                    await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                    {
                        Type = OperatorLogType.登录,
                        Content = JsonConvert.SerializeObject(input)
                    });
                    await Commit();
                    result.IsSuccess = true;
                }
                else
                {
                    var admin = await _adminDomainService.Get(x => x.Name == input.Name);
                    if (admin == null)
                    {
                        result.Message = "账号不存在";
                        await _operatorLogDomainService.AddError(new OperatorLogEntity
                        {
                            Type = OperatorLogType.登录,
                            Content = $"Data={JsonConvert.SerializeObject(input)},ErrorMessage={result.Message}"
                        });
                        await Commit();
                    }
                    else if (admin.Password != input.Password.ToMd5())
                    {
                        result.Message = "密码错误";
                        await _operatorLogDomainService.AddError(new OperatorLogEntity
                        {
                            Type = OperatorLogType.登录,
                            Content = $"Data={JsonConvert.SerializeObject(input)},ErrorMessage={result.Message}"
                        });
                        await Commit();
                    }
                    else
                    {
                        var claims = new[] {
                            new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                            new Claim(ClaimTypes.Name, admin.Name)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "Default");
                        ClaimsPrincipal user = new ClaimsPrincipal(claimsIdentity);

                        await _httpAccessor.HttpContext.SignInAsync("admin", user, new AuthenticationProperties { IsPersistent = true, AllowRefresh = true, ExpiresUtc = DateTimeOffset.Now.AddDays(30) });

                        await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                        {
                            Type = OperatorLogType.登录,
                            Content = ""
                        });
                        await Commit();
                    }
                }
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.登录,
                    Content = $"Data={JsonConvert.SerializeObject(input)},ErrorMessage={result.Message}"
                });
                await Commit();
            }

            return result;
        }

        public async Task<ResultDto> Logout()
        {
            var result = new ResultDto { Message = "" };
            try
            {
                await _httpAccessor.HttpContext.SignOutAsync("admin");

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.退出登录,
                    Content = ""
                });
                await Commit();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.退出登录,
                    Content = $"ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> ModifyPassword(string name,ModifyPasswordInput input)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var admin = await _adminDomainService.Get(x => x.Name == name);
                if (admin == null)
                {
                    result.Message = "账号不存在";
                    await _operatorLogDomainService.AddError(new OperatorLogEntity
                    {
                        Type = OperatorLogType.修改密码,
                        Content = $"Data={JsonConvert.SerializeObject(input)},ErrorMessage={result.Message}"
                    });
                    await Commit();
                }
                else if (admin.Password != input.Password.ToMd5())
                {
                    result.Message = "密码错误";
                    await _operatorLogDomainService.AddError(new OperatorLogEntity
                    {
                        Type = OperatorLogType.修改密码,
                        Content = $"Data={JsonConvert.SerializeObject(input)},ErrorMessage={result.Message}"
                    });
                    await Commit();
                }
                else
                {
                    admin.Password = input.NewPassword.ToMd5();
                    await _adminDomainService.Update(admin);

                    await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                    {
                        Type = OperatorLogType.修改密码,
                        Content = $"Data={JsonConvert.SerializeObject(input)}"
                    });
                    await Commit();
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改密码,
                    Content = $"Data={JsonConvert.SerializeObject(input)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }
    }
}
