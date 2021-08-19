using System;

namespace Validation
{
    internal class ProductService
    {
        private readonly IDatabaseAccess _db;

        public ProductService(IDatabaseAccess db)
        {
            _db = db;
        }

        private static Response Validate(EnrichedProductFormData data)
        {
            var result = new Response(0, 0, null);
            if ("" == (data.Name))
            {
                result = new Response(0, -2, "Missing Name");
            }

            if ("" == (data.Type))
            {
                result = new Response(0, -2, "Missing Type");
            }

            if ("Lipstick" == data.Type && data.SuggestedPrice > 20 && data.Weight > 0 && data.Weight < 10)
            {
                result = new Response(0, -1, "Error - failed quality check for Queen Range");
            }

            if (data.Weight < 0)
            {
                result = new Response(0, -3, "Weight error");
            }

            if ("Blusher" == (data.Type) && data.Weight > 10)
            {
                result = new Response(0, -3, "Error - weight too high");
            }

            if ("Unknown" == (data.Type))
            {
                result = new Response(0, -1, "Unknown product type " + data.Type);
            }

            if (!data.PackagingRecyclable && data.Range == ProductRange.QUEEN)
            {
                result = new Response(0, -1, "Error - failed quality check for Queen Range");
            }

            return result;
        }

        private static ProductRange CalculateRange(ProductFormData productData)
        {
            var result = ProductRange.BUDGET;
            if (productData.PackagingRecyclable)
            {
                result = (ProductRange.PROFESSIONAL);
            }

            if (nameof(Eyeshadow) == (productData.Type) && productData.Name.Contains("Queen"))
            {
                result = (ProductRange.QUEEN);
            }
            else if (nameof(Foundation) == (productData.Type) && productData.SuggestedPrice > 10)
            {
                result = (ProductRange.PROFESSIONAL);
            }
            else if (nameof(Lipstick) == (productData.Type))
            {
                if (productData.SuggestedPrice > 10)
                {
                    result = (ProductRange.PROFESSIONAL);
                }

                if (productData.SuggestedPrice > 20)
                {
                    result = (ProductRange.QUEEN);
                }
            }
            else if (nameof(Mascara) == (productData.Type))
            {
                if (productData.SuggestedPrice > 15)
                {
                    result = (ProductRange.PROFESSIONAL);
                }

                if (productData.SuggestedPrice > 25 && productData.PackagingRecyclable)
                {
                    result = (ProductRange.QUEEN);
                }
            }

            return result;
        }

        public Response ValidateAndAdd(ProductFormData productData)
        {
            var data = EnrichData(productData);
            var response = Validate(data);
            if (response.StatusCode != 0)
            {
                return response;
            }

            return new Response(_db.storeProduct(CreateProduct(data)), 0, "Product Successfully Added");
        }

        private static EnrichedProductFormData EnrichData(ProductFormData productData)
        {
            var result = new EnrichedProductFormData(productData);
            result.Range = CalculateRange(productData);
            return result;
        }

        private static Product CreateProduct(EnrichedProductFormData data)
        {
            var type = Type.GetType($"{typeof(Product).Namespace}.{data.Type}");
            var result = (Product)Activator.CreateInstance(type, new object[] { data.Name });

            result.Range = data.Range;
            result.Weight = data.Weight;

            return result;
        }
    }
}