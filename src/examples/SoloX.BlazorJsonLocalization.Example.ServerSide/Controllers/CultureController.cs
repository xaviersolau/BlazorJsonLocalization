using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace SoloX.BlazorJsonLocalization.Example.ServerSide.Controllers
{
    /// <summary>
    /// This controller is dedicated to setup the current culture in a Browser cookie.
    /// See https://docs.microsoft.com/fr-fr/aspnet/core/blazor/globalization-localization?view=aspnetcore-5.0#provide-ui-to-choose-the-culture
    /// for more informations.
    /// </summary>
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        public IActionResult SetCulture(string culture, string redirectUri)
        {
            if (culture != null)
            {
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(
                        new RequestCulture(culture)));
            }

            return LocalRedirect(redirectUri);
        }
    }
}
