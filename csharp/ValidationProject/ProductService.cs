using System;
using System.Collections.Generic;
using System.Linq;
using Validator = System.Func<Validation.EnrichedProductFormData, Validation.Response>;

namespace Validation
{
    internal class ProductService
    {
        private static List<Func<EnrichedProductFormData, Response>> _validators;
        private readonly IDatabaseAccess _db;

        static ProductService()
        {
            _validators = new List<Func<EnrichedProductFormData, Response>>()
            {
                new Validator(x => x.Name == "" ? new Response(0, -2, "Missing Name") : null),
                new Validator(x => x.Type == "" ? new Response(0, -2, "Missing Type") : null),
                new Validator(x => "Lipstick" == x.Type && x.SuggestedPrice > 20 && x.Weight > 0 && x.Weight < 10 ? new Response(0, -1, "Error - failed quality check for Queen Range") : null),
                new Validator(x => x.Weight < 0 ? new Response(0, -3, "Weight error") : null),
                new Validator(x => "Blusher" == (x.Type) && x.Weight > 10 ? new Response(0, -3, "Error - weight too high") : null),
                new Validator(x => "Unknown" == (x.Type) ? new Response(0, -1, "Unknown product type " + x.Type) : null),
                new Validator(x => !x.PackagingRecyclable && x.Range == ProductRange.QUEEN ? new Response(0, -1, "Error - failed quality check for Queen Range") : null),
            };
        }

        public ProductService(IDatabaseAccess db)
        {
            _db = db;
        }

        private static Response Validate(EnrichedProductFormData data)
        {
            return _validators.Aggregate(default(Response), (acc, v) =>
            {
                acc = acc?.StatusCode < 0 ? acc : v(data);
                return acc;
            });
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
            if (response != null && response.StatusCode != 0)
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