using Mango.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Mango.Web.Services.Interfaces
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public void ClearToken()
        {
            _contextAccessor.HttpContext.Response.Cookies.Delete(SD.TokenCookie);
        }

        public string GetToken()
        {
            bool hasToken = _contextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.TokenCookie, out var token);
            return hasToken is true ? token : null;
        }  

        public void SetToken(string token)
        {
            _contextAccessor.HttpContext.Response.Cookies.Append(SD.TokenCookie, token);
        }
    }
}
