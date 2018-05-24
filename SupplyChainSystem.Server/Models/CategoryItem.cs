namespace SupplyChainSystem.Server.Models
{
    public class CategoryItem
    {
        public string VirtualItemId { get; set; }
        public VirtualItem VirtualItem { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}