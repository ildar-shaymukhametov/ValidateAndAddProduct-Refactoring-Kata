namespace Validation
{
    internal class ProductService
    {
        private readonly IDatabaseAccess _db;

        public ProductService(IDatabaseAccess db)
        {
            _db = db;
        }

        private static Response Validate(ProductFormData productData, ProductRange range)
        {
            var result = new Response(0, 0, null);
            if ("" == (productData.Name))
            {
                result = new Response(0, -2, "Missing Name");
            }

            if ("" == (productData.Type))
            {
                result = new Response(0, -2, "Missing Type");
            }

            if ("Lipstick" == productData.Type && productData.SuggestedPrice > 20 && productData.Weight > 0 && productData.Weight < 10)
            {
                result = new Response(0, -1, "Error - failed quality check for Queen Range");
            }

            if (productData.Weight < 0)
            {
                result = new Response(0, -3, "Weight error");
            }

            if ("Blusher" == (productData.Type) && productData.Weight > 10)
            {
                result = new Response(0, -3, "Error - weight too high");
            }

            if ("Unknown" == (productData.Type))
            {
                result = new Response(0, -1, "Unknown product type " + productData.Type);
            }

            if (!productData.PackagingRecyclable && range == ProductRange.QUEEN)
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

            if ("Eyeshadow" == (productData.Type) && productData.Name.Contains("Queen"))
            {
                result = (ProductRange.QUEEN);
            }
            else if ("Foundation" == (productData.Type) && productData.SuggestedPrice > 10)
            {
                result = (ProductRange.PROFESSIONAL);
            }
            else if ("Lipstick" == (productData.Type))
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
            else if ("Mascara" == (productData.Type))
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

        public Response ValidateAndAdd(ProductFormData data)
        {
            var range = CalculateRange(data);
            var response = Validate(data, range);
            if (response.StatusCode != 0)
            {
                return response;
            }
            
            return new Response(_db.storeProduct(CreateProduct(data, range)), 0, "Product Successfully Added");
        }

        private static Product CreateProduct(ProductFormData data, ProductRange range)
        {
            var result = new Product(data.Name);
            result.Range = range;
            result.Weight = (data.Weight);

            if ("Eyeshadow" == (data.Type) || "Mascara" == (data.Type))
            {
                result.Type = (data.Type);
                result.Family = (ProductFamily.EYES);
            }

            if ("Lipstick" == (data.Type))
            {
                result.Type = (data.Type);
                result.Family = (ProductFamily.LIPS);
            }

            if ("Mascara" == (data.Type))
            {
                result.Family = (ProductFamily.LASHES);
            }

            if ("Blusher" == (data.Type) || "Foundation" == (data.Type))
            {
                result.Type = (data.Type);
                result.Family = (ProductFamily.SKIN);
            }

            return result;
        }
    }
}