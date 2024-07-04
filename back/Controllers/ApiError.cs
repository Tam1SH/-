using back.Domain;

namespace back.Controllers
{
	
    public class ApiError
    {
        public long Code { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
    }

    public class ApiErrorFactory<T>
    {
        public ApiErrorFactory() { }

        public ApiError Create(ErrorCode errorCode)
        {
            return new ApiError { Code = errorCode.Code, Message = errorCode.Message, Type = typeof(T).Name };
        }
    }
}
