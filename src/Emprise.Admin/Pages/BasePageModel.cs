using Emprise.Domain.Core.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;


namespace Emprise.Admin.Pages
{
    public class BasePageModel : PageModel
    {

        protected readonly ILogger<BasePageModel> _logger;

        public BasePageModel(ILogger<BasePageModel> logger)
        {

        }



    }

    public class OperatorLog
    {
        public OperatorLogType Type { set; get; }

        public string Content { set; get; }

        public string Name { set; get; }
    }
}
