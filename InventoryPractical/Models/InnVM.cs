namespace InventoryPractical.Models
{
    public class InnVM
    {
        public string Date { get; set; }
        public string ProductCode { get; set; }
        public double EventType { get; set; }
        public double Total_Purchase_Quantity { get; set; }
        public double Total_Purchase_Amount { get; set; }
        public string Total_Sale_Quantity { get; set; }
        public string Total_Sale_Amount { get; set; }
        public string Profit_Loss { get; set; }
        public double Opening_Quantity { get; set; }
        public double Closing_Quantity { get; set; }
        public double Purchase_Price { get; set; }
    }
}
