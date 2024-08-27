namespace Shopy_App.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string CartId { get; set; } // Can be user ID or unique session key
    }
}
