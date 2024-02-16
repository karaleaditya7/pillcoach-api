using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OntrackDb.Dto
{
    public class Response<T>
    {
        public Boolean Success { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AccessToken { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<T> DataList { get; set; }

        
        public static Response<Exception> createErrorResponse(Exception ex)
        {
            Response<Exception> response;
            response = new Response<Exception>();
            response.Data = ex;
            response.Success = false;
            response.Message = "Somethings went Wrong";
            return response;
        }
    }
}
