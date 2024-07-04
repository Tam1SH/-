
namespace back.Controllers.Reports;
public class GetTransactionHistoryModel 
{
	public DateTime? FromDate { get; set; } 
	public DateTime? ToDate { get; set; }
	public string? Currency { get; set; }
	public long? AccountId { get; set; }
}