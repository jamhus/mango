using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Mango.Web.Utility;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _http;

        public BaseService(IHttpClientFactory http)
        {
            _http = http;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            HttpClient client = _http.CreateClient("MangoAPI");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            // token

            message.RequestUri = new Uri(requestDto.Url);

            if (requestDto.Data != null) message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");

            HttpResponseMessage apiResponse = null;

            switch (requestDto.ApiType)
            {

                case SD.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case SD.ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case SD.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            apiResponse = await client.SendAsync(message);

            try
            {
                switch (apiResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        return new ResponseDto { IsSuccess = false, DisplayMessage = "Resource not found" };
                    case System.Net.HttpStatusCode.Forbidden:
                        return new ResponseDto { IsSuccess = false, DisplayMessage = "Access denied" };
                    case System.Net.HttpStatusCode.Unauthorized:
                        return new ResponseDto { IsSuccess = false, DisplayMessage = "Unauthorized" };
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new ResponseDto { IsSuccess = false, DisplayMessage = "Internal server error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return responseDto;
                }
            }
            catch (Exception ex)
            {

                return new ResponseDto
                {
                    IsSuccess = false,
                    DisplayMessage = ex.Message
                };
            }

        }
    }
}
