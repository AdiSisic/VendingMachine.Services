using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Services.Api.Product.Request
{
    public class ManipulateProductRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public int Cost { get; set; }
    }
}
