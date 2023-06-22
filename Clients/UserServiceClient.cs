using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task9.Extensions;
using Task9.Models.Responses.Base;
using Task9.Models.Requests;

namespace Task9.Clients
{
    public class UserServiceClient
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _baseUrl = "https://userservice-uat.azurewebsites.net";

        public async Task<CommonResponse<Int32>> RegisterNewUser(RegisterNewUserRequest request)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Register/RegisterNewUser"),
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
            return await response.ToCommonResponse<Int32>();
        }

        public async Task<CommonResponse<object>> DeleteUser(Int32 userId)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_baseUrl}/Register/DeleteUser?userId={userId}")
            };

            HttpResponseMessage response = await _client.DeleteAsync(httpRequestMessage.RequestUri);
            return await response.ToCommonResponse<object>();
        }

        public async Task<CommonResponse<bool>> SetUserStatus(int userId, bool newStatus)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_baseUrl}/UserManagement/SetUserStatus?userId={userId}&newStatus={newStatus}")
            };

            HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);

            var commonResponse = new CommonResponse<bool>
            {
                Status = response.StatusCode,
                Content = await response.Content.ReadAsStringAsync(),
                IsSuccess = response.IsSuccessStatusCode
            };

            if (!commonResponse.IsSuccess)
            {
                commonResponse.ErrorMessage = "Failed to set user status";
            }

            return commonResponse;
        }


        public async Task<CommonResponse<bool>> GetUserStatus(int userId, string noElementsMessage)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_baseUrl}/UserManagement/GetUserStatus?userId={userId}")
            };

            HttpResponseMessage response = await _client.GetAsync(httpRequestMessage.RequestUri);

            var commonResponse = new CommonResponse<bool>
            {
                Status = response.StatusCode,
                Content = await response.Content.ReadAsStringAsync(),
                IsSuccess = response.IsSuccessStatusCode
            };

            if (commonResponse.IsSuccess)
            {
                if (commonResponse.Content == noElementsMessage)
                {
                    commonResponse.IsSuccess = false;
                    commonResponse.ErrorMessage = "User status not found";
                    commonResponse.Body = false;
                }
                else
                {
                    commonResponse.Body = bool.Parse(commonResponse.Content);
                }
            }

            return commonResponse;
        }


    }
}
