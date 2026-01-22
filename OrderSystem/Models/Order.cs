namespace OrderSystem.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Created";

        public Product? Product { get; set; }
    }
}
