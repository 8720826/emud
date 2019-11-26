using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.User
{
    public class ModifyPasswordModel : BasePageModel
    {
        public ModifyPasswordModel(IOptions<AppConfig> appConfig) : base(appConfig)
        {

        }

        public void OnGet()
        {

        }
    }
}