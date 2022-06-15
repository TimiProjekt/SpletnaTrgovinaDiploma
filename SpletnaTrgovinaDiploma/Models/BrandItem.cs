namespace SpletnaTrgovinaDiploma.Models
{
    public class BrandItem
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }
    }
}
