namespace Validation
{
    internal class ProductFormData
    {
        public string Name { get; }
        public string Type { get; }
        public double Weight { get; }
        public double SuggestedPrice { get; }
        public bool PackagingRecyclable { get; }

        public ProductFormData(string name, string type, double weight, double suggestedPrice, bool packagingRecyclable)
        {
            Name = name;
            Type = type;
            Weight = weight;
            SuggestedPrice = suggestedPrice;
            PackagingRecyclable = packagingRecyclable;
        }
    }

    internal class EnrichedProductFormData : ProductFormData
    {
        public EnrichedProductFormData(ProductFormData data) : base(data.Name, data.Type, data.Weight, data.SuggestedPrice, data.PackagingRecyclable)
        {
        }

        public ProductRange Range { get; set; }
    }
}