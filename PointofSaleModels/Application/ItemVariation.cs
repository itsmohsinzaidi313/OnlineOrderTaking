namespace PointofSaleModels.Application
{
    public class ItemVariation
    {
        public int Id { get; set; }
        public ItemSize Size { get; set; }
        public ItemFlavour Flavour { get; set; }
        public double Price { get; set; }
        public List<ItemChoice> ItemChoices { get; set; } = [];
    }
}
