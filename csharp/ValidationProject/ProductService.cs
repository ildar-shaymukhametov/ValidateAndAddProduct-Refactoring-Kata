using System;
using System.Collections.Generic;
using System.Linq;
using Validator = System.Func<Validation.EnrichedProductFormData, Validation.Response>;

namespace Validation
{
    internal class ProductService
    {
        private readonly List<Func<EnrichedProductFormData, Response>> _validators;
        private readonly List<Func<ProductFormData, ProductRange?>> _rangeCalculators;
        private readonly IDatabaseAccess _db;

        public ProductService(IDatabaseAccess db)
        {
            _db = db;
            _validators = new List<Func<EnrichedProductFormData, Response>>()
            {
                new Validator(x => x.Name == "" ? new Response(0, -2, "Missing Name") : null),
                new Validator(x => x.Type == "" ? new Response(0, -2, "Missing Type") : null),
                new Validator(x => "Lipstick" == x.Type && x.SuggestedPrice > 20 && x.Weight > 0 && x.Weight < 10 ? new Response(0, -1, "Error - failed quality check for Queen Range") : null),
                new Validator(x => x.Weight < 0 ? new Response(0, -3, "Weight error") : null),
                new Validator(x => "Blusher" == (x.Type) && x.Weight > 10 ? new Response(0, -3, "Error - weight too high") : null),
                new Validator(x => "Unknown" == (x.Type) ? new Response(0, -1, "Unknown product type " + x.Type) : null),
                new Validator(x => !x.PackagingRecyclable && x.Range == ProductRange.QUEEN ? new Response(0, -1, "Error - failed quality check for Queen Range") : null),
                new Validator(x => "Lipgloss" == (x.Type) && x.Weight > 20 ? new Response(0, -3, "Error - weight too high") : null),
            };
            _rangeCalculators = new List<Func<ProductFormData, ProductRange?>>
            {
                new Func<ProductFormData, ProductRange?>(x => nameof(Eyeshadow) == (x.Type) && x.Name.Contains("Queen") ? ProductRange.QUEEN : null),
                new Func<ProductFormData, ProductRange?>(x => nameof(Foundation) == (x.Type) && x.SuggestedPrice > 10 ? ProductRange.PROFESSIONAL : null),
                new Func<ProductFormData, ProductRange?>(x => nameof(Lipstick) == (x.Type) && x.SuggestedPrice > 10 ? ProductRange.PROFESSIONAL : null),
                new Func<ProductFormData, ProductRange?>(x => nameof(Lipstick) == (x.Type) && x.SuggestedPrice > 20 ? ProductRange.QUEEN : null),
                new Func<ProductFormData, ProductRange?>(x => nameof(Mascara) == (x.Type) && x.SuggestedPrice > 15 && !x.PackagingRecyclable ? ProductRange.PROFESSIONAL : null),
                new Func<ProductFormData, ProductRange?>(x => nameof(Mascara) == (x.Type) && x.SuggestedPrice > 25 && x.PackagingRecyclable ? ProductRange.QUEEN : null),
                new Func<ProductFormData, ProductRange?>(x => x.PackagingRecyclable ? ProductRange.PROFESSIONAL : null),
                new Func<ProductFormData, ProductRange?>(x => nameof(Lipgloss) == (x.Type) && x.SuggestedPrice > 10 ? ProductRange.QUEEN : null),
            };
        }

        private Response Validate(EnrichedProductFormData data)
        {
            return _validators.Aggregate(default(Response), (acc, v) => acc ?? v(data));
        }

        private ProductRange CalculateRange(ProductFormData productData)
        {
            var result = _rangeCalculators.Aggregate(default(ProductRange?), (acc, v) => acc ?? v(productData));
            return result ?? ProductRange.BUDGET;
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

        private EnrichedProductFormData EnrichData(ProductFormData productData)
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