namespace back.DataLayer.Model
{
    public class Users
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}
