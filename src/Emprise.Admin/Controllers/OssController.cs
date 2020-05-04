using Emprise.Admin.Helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Controllers
{
    [Route("oss")]
    public class OssController
    {
        private readonly IOssHelper _ossHelper;
        public OssController(IOssHelper ossHelper)
        {
            _ossHelper = ossHelper;
        }


        [Route("token")]
        [HttpPost]
        public async Task<IActionResult> GetToken(int id)
        {
            var tokenModel = await _ossHelper.GetToken();
            return new JsonResult(tokenModel);
        }
    }
}
