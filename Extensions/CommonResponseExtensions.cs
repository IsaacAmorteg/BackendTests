using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task9.Models.Responses.Base;

namespace Task9.Extensions
{
    public static class CommonResponseExtensions
    {
        public static async Task<CommonResponse<T>> ToCommonResponse<T>(this HttpResponseMessage message)
        {
            string responseBody = await message.Content.ReadAsStringAsync();

            var commonResponse = new CommonResponse<T>
            {
                Status = message.StatusCode,
                Content = responseBody
            };

            try
            {
                commonResponse.Body = JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (JsonReaderException exception)
            {
            }

            return commonResponse;
        }
    }
}