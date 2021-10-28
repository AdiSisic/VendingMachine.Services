namespace VendingMachine.Services.Application.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SellerId { get; set; }
        public int Amount { get; set; }
        public int Cost { get; set; }
    }
}
