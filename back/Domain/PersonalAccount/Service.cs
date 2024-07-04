

using back.DataLayer.Model;
using back.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace back.Domain.PersonalAccountService;

public class AccountNotFoundException : Exception { }

public class CurrencyConversionException: Exception { }

public class InsufficientFundsException : Exception { }

public class CannotCreateMorePersonalAccountsException: Exception { }

public enum Currency { RUB, YE }

public class PersonalAccountService
{
	private readonly PersonalAccountRepository _personalAccountRepository;
	private readonly TransactionHistoryRepository _transactionHistoryRepository;
	private readonly ILogger _logger;
	public PersonalAccountService(
		PersonalAccountRepository personalAccountRepository,
		ILogger<PersonalAccountService> logger,
		TransactionHistoryRepository transactionHistoryRepository
	) 
	{
		_logger = logger;
		_personalAccountRepository = personalAccountRepository;
		_transactionHistoryRepository = transactionHistoryRepository;
	}

	public async Task<List<PersonalAccount>> GetPersonalsAccounts(long userId)
	{
		var accounts = await _personalAccountRepository.PersonalAccounts.Where(a => a.UserId == userId).ToListAsync();

		return accounts;
	}

	public async Task CreatePersonalAccount(long userId, Currency currency)
	{
		var accounts = _personalAccountRepository.PersonalAccounts.Where(a => a.UserId == userId);
		
		if (accounts.Count() >= 5) {
			throw new CannotCreateMorePersonalAccountsException();
		}

		var newAccount = new PersonalAccount
		{
			Amount = 0,
			Currency = currency.ToString(),
			UserId = userId,
		};

		_personalAccountRepository.PersonalAccounts.Add(newAccount);

		await _personalAccountRepository.SaveChangesAsync();

	}

	public async Task<decimal> ConvertCurrency(long accountId, Currency targetCurrency)
	{
		var account = await _personalAccountRepository.PersonalAccounts
			.SingleOrDefaultAsync(a => a.Id == accountId);

		if (account == null)
		{
			_logger.LogWarning("Account with ID {} not found.", accountId);
			throw new AccountNotFoundException();
		}

		if (account.Currency == targetCurrency.ToString())
		{
			return account.Amount; // Если валюта уже в нужной валюте, просто возвращаем сумму
		}

		var currencies = (await _personalAccountRepository.CurrencyExchanges.FirstAsync()).GetCurrencies()!;

		if (!currencies.TryGetValue(account.Currency, out var fromRate) || !currencies.TryGetValue(targetCurrency.ToString(), out var toRate))
		{
			_logger.LogError("Currency conversion rates not available for {} or {}.", account.Currency, targetCurrency);
			throw new CurrencyConversionException();
		}

		var rate = toRate / fromRate;
		var convertedAmount = account.Amount * rate;

		account.Amount = convertedAmount;
		account.Currency = targetCurrency.ToString();

		await _personalAccountRepository.SaveChangesAsync();

		return convertedAmount;
	}
	
	public async Task TransferFunds(long FromUserId, long fromAccountId, long ToUserId, long toAccountId, decimal amount)
	{
		try
		{
			var FromAccounts = _personalAccountRepository
				.PersonalAccounts
				.Where(a => a.UserId == FromUserId);
		
			var ToAccounts = _personalAccountRepository
				.PersonalAccounts
				.Where(a => a.UserId == ToUserId);

			var fromAccount = await FromAccounts.SingleAsync(a => a.Id == fromAccountId);
			var toAccount = await ToAccounts.SingleAsync(a => a.Id == toAccountId);
			

			if (fromAccount == null)
			{
				_logger.LogWarning("Account with ID {} not found.", fromAccountId);
				throw new AccountNotFoundException();
			}

			if (toAccount == null)
			{
				_logger.LogWarning("Account with ID {} not found.", toAccountId);
				throw new AccountNotFoundException();
			}

			if (fromAccount.Amount < amount)
			{
				_logger.LogWarning("Insufficient funds on account with ID {}.", fromAccountId);
				throw new InsufficientFundsException();
			}

			var currencies = (await _personalAccountRepository.CurrencyExchanges.FirstAsync()).GetCurrencies()!;

			if (fromAccount.Currency != toAccount.Currency)
			{
				if (!currencies.TryGetValue(fromAccount.Currency, out var fromRate) || !currencies.TryGetValue(toAccount.Currency, out var toRate))
				{
					_logger.LogError("Currency conversion rates not available for {} or {}.", fromAccount.Currency, toAccount.Currency);
					throw new CurrencyConversionException();
				}

				var rate = toRate / fromRate;
				var convertedAmount = amount * rate;

				fromAccount.Amount -= amount;
				toAccount.Amount += convertedAmount; 
			}
			else
			{
				fromAccount.Amount -= amount;
				toAccount.Amount += amount;
			}

			
			_transactionHistoryRepository.TransactionHistories.Add(
				new TransactionHistory
				{
					FromAccountId = fromAccountId,
					ToAccountId = toAccountId,
					FromUserId = FromUserId,
					ToUserId = ToUserId,
					Amount = amount,
					Currency = fromAccount.Currency,
					TransactionDate = DateTime.UtcNow
				}
			);

			await _personalAccountRepository.SaveChangesAsync();

			_logger.LogInformation("Transferred {} from account {} to account {}.", amount, fromAccountId, toAccountId);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred during the transfer.");
			throw;
		}
	}
}