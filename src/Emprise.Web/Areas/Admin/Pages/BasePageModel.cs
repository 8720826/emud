using Emprise.Domain.Core.Enums;
using Emprise.Infra.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;


namespace Emprise.Web.Areas.Admin.Pages
{

    [AdminAuthorize]
    public class BaseAdminPageModel : PageModel
    {

        protected readonly ILogger<BaseAdminPageModel> _logger;

        public BaseAdminPageModel(ILogger<BaseAdminPageModel> logger)
        {

        }



    }


}
