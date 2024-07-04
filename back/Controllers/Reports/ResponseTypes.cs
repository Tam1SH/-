

namespace back.Controllers.Reports;

public class TransactionHistoryResponse
{
	public long FromAccountId { get; set; }
	public long ToAccountId { get; set; }
	public long SenderUserId { get; set; }
	public long ReceiverUserId { get; set; }
	public decimal Amount { get; set; }
	public string Currency { get; set; }
	public DateTime TransactionDate { get; set; }
}
