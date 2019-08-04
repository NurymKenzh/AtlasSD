using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AtlasSD.Models
{
    public class Middleware
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        private readonly RequestDelegate next;

        public Middleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            this.BeginInvoke(context);
            await this.next.Invoke(context);
            this.EndInvoke(context);
        }

        private void BeginInvoke(HttpContext context)
        {
            // Do custom work before controller execution
            //string Language = _session.GetString("Language");
            //if (!string.IsNullOrEmpty(Language))
            //{
            //    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Language);
            //    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(Language);
            //}
            //else
            //{
            //    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru");
            //    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru");
            //}
        }

        private void EndInvoke(HttpContext context)
        {
            // Do custom work after controller execution
        }
    }
}
