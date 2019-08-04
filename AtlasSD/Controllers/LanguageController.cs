using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace AtlasSD.Controllers
{
    public class LanguageController : Controller
    {
        public ActionResult Change(string LanguageAbbrevation)
        {
            if (LanguageAbbrevation != null)
            {
                //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(LanguageAbbrevation);
                //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(LanguageAbbrevation);

                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(LanguageAbbrevation)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );

            }

            //HttpContext.Session.SetString("Language", LanguageAbbrevation);

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}