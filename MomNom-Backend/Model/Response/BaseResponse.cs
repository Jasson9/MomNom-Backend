using Microsoft.AspNetCore.Mvc;

namespace MomNom_Backend.Model.Response
{
    public class BaseResponse<T>
    {
        public int statusCode { get; set; }
        public string statusMessage { get; set; }
        public T? data { get; set; }

        public BaseResponse(int statusCode, string statusMessage, T data) { 
            this.statusCode = statusCode;
            this.statusMessage = statusMessage; 
            this.data = data;
        }

        public BaseResponse(int statusCode, string statusMessage)
        {
            this.statusCode = statusCode;
            this.statusMessage = statusMessage;
        }

        public BaseResponse(T data) : base()
        {
            this.statusCode = 200;
            this.statusMessage = "OK";
            this.data = data;
        }
    }

    public class BaseResponse
    {

        public int statusCode { get; set; }
        public string statusMessage { get; set; }
        public object? data { get; set; }
    }
}
