using back.Domain.PersonalAccountService;

namespace back.Controllers.PersonalAccountManager;

public class CreateAccountModel
{
    public Currency Currency { get; set; }
}

public class TransferFundsModel
{
	public long FromAccountId { get; set; }
	public long ToUserId { get; set; }
    public long ToAccountId { get; set; }
    public decimal Amount { get; set; }
}

public class ConvertCurrencyModel
{
    public long AccountId { get; set; }
    public Currency TargetCurrency { get; set; }
}