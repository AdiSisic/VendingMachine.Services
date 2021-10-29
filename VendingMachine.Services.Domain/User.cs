using System.Collections.Generic;

namespace VendingMachine.Services.Domain
{
    public class User
    {
        public User()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Deposit { get; set; }
        public int RoleId { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
