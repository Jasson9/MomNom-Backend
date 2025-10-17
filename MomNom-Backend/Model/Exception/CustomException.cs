using Microsoft.AspNetCore.Mvc;
using MomNom_Backend.Model.Response;

namespace MomNom_Backend.Model.Exception
{
    public class BaseException<T> : System.Exception
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public BaseException() { }

        public BaseException(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public BaseResponse<T> toResponseOutput(T data)
        {
            return new BaseResponse<T>(ErrorCode,ErrorMessage,data);
        }

        public BaseResponse<T> toResponseOutput()
        {
            return new BaseResponse<T>(ErrorCode,ErrorMessage);
        }
    }

    public class InternalServerErrorException<T> : BaseException<T>
    {

        public InternalServerErrorException(): base(500, "Internal Server Error") { }
        public InternalServerErrorException(string errorMessage): base(500, errorMessage) { }
    }

    public class BadRequestException<T> : BaseException<T>
    {
        public BadRequestException(string errorMessage) : base(400, errorMessage) { }
    }

    public class UnauthorizedException<T> : BaseException<T>
    {
        public UnauthorizedException(string errorMessage) : base(401, errorMessage) { }
    }
}
