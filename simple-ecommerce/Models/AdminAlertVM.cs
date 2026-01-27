namespace simple_ecommerce.Models
{
    public class AdminAlertVM
    {
        public int NewOrderCount { get; set; }
        public List<OrderAlertItem> Orders { get; set; }
    }

    public class OrderAlertItem
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
    }

}
