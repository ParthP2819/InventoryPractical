namespace InventoryPractical.Models
{
    public class Inventory
    {
        public string ProductCode { get; set; }
        public int EventType { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }
    }
}
