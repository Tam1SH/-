

namespace back.DataLayer.Model;

public class PersonalAccount 
{
	public long Id { get; set; }
	public long UserId { get; set; }
	public decimal Amount { get; set; }
	public string Currency { get; set; }
}