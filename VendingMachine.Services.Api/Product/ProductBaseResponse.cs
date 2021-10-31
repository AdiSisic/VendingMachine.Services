namespace VendingMachine.Services.Api.Product
{
    public class ProductBaseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SellerId { get; set; }
        public int Amount { get; set; }
        public int Cost { get; set; }
    }
}
