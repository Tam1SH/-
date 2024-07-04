namespace back.DataLayer.Model;

public class TransactionHistory
{
	public long Id { get; set; }
	public long FromAccountId { get; set; }
	public long ToAccountId { get; set; }
	public long FromUserId { get; set; }
	public long ToUserId { get; set; }
	public decimal Amount { get; set; }
	public string Currency { get; set; }
	public DateTime TransactionDate { get; set; }
}
