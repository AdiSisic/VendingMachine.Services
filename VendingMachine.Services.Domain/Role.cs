using System.Collections.Generic;

namespace VendingMachine.Services.Domain
{
    public class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
