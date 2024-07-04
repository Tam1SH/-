
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace back.DataLayer.Model;

public class CurrencyExchange 
{
    public long Id { get; set; } 
	
    [Required]
    public string CurrenciesJson { get; set; }

    public Dictionary<string, decimal>? GetCurrencies()
    {
        return string.IsNullOrEmpty(CurrenciesJson) 
            ? new Dictionary<string, decimal>() 
            : JsonSerializer.Deserialize<Dictionary<string, decimal>>(CurrenciesJson);
    }

    public void SetCurrencies(Dictionary<string, decimal> currencies)
    {
        CurrenciesJson = JsonSerializer.Serialize(currencies);
    }
}


