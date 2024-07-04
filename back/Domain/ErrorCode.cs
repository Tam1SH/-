
namespace back.Domain
{
	public class ErrorCode
	{
		public long Code { get; set; }
		public string Message { get; set; }

		public ErrorCode(long code, string message) 
		{
			Code = code;
			Message = message;
		}
	}
}