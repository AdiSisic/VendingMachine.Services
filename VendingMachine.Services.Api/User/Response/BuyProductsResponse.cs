using System.Collections.Generic;

namespace VendingMachine.Services.Api.User.Response
{
    public class BuyProductsResponse
    {
        public int Spent { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public int MoneyLeft { get; set; }
        public List<Change> Change { get; set; }
    }

    public class Change
    {
        public int Amount { get; set; }
        public int Coin { get; set; }
    }
}
