using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Middlewares
{
    public class UploadMiddleware
    {
        private readonly RequestDelegate _next;

        public UploadMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var files = context.Request.Form.Files;
            if (files == null || files.Count == 0)
            {
                await context.Response.WriteAsync("");
            }
            var file = files[0];

            string today = DateTime.Now.ToString("yyyyMMdd");
            string fileName = Guid.NewGuid().ToString();

            var path = Path.Combine("/upload/", today, $"{fileName}{file.FileName}");

            Directory.CreateDirectory(path);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
