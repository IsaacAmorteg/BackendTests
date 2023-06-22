using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Task9.Models.Responses.Base
{
    public class CommonResponse<T>
    {
        public HttpStatusCode Status;

        public string Content;

        public T Body;

        public bool IsSuccess { get; internal set; }
        public string ErrorMessage { get; internal set; }
    }

}
