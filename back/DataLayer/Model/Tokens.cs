using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back.DataLayer.Model 
{
	public class Tokens
    {
        [Key]
        public string RefreshToken { get; set; }
        public long UserId { get; set; }
        public DateTime ExpiresIn { get; set; }
        public DateTime LastActivity { get; set; }

    }
}
