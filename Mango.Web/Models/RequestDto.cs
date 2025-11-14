using static Mango.Web.Utility.SD;

namespace Mango.Web.Models
{
    public class RequestDto
    {
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
        public ApiType ApiType { get; set; } = ApiType.GET;
    }
}
