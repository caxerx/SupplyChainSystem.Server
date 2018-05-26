namespace SupplyChainSystem.Server.Models
{
    public class VirtualIdMap
    {

        public string VirtualItemId { get; set; }
        public VirtualItem VirtualItem { get; set; }

        public string SupplierItemId { get; set; }
        public Item Item { get; set; }
    }
}