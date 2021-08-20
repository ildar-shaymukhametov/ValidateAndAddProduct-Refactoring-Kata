using Xunit;
using ApprovalTests;
using ApprovalTests.Reporters;

namespace Validation
{
    [UseReporter(typeof(DiffReporter))]
    public class ValidationTest
    {
        [Fact]
        void Empty_product_name_produces_missing_name_response()
        {
            var productName = string.Empty;

            var actual = CreateSut().ValidateAndAdd(CreateData(name: productName));

            var expected = new Response(0, -2, "Missing Name");
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        void Empty_product_type_produces_missing_type_response()
        {
            var productType = string.Empty;

            var actual = CreateSut().ValidateAndAdd(CreateData(type: productType));

            var expected = new Response(0, -2, "Missing Type");
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        void Product_with_negative_weight_produces_weight_error_response()
        {
            var productWeight = -1;

            var actual = CreateSut().ValidateAndAdd(CreateData(weight: productWeight));

            var expected = new Response(0, -3, "Weight error");
            Assert.Equal(expected.ToString(), actual.ToString());
        }
        
        [Fact]
        void Lipstick_that_is_not_recyclable_and_has_suggested_price_greater_than_20_produces_failed_quality_check_response()
        {
            var recyclable = false;
            var suggestedPrice = 20 + 1;

            var actual = CreateSut().ValidateAndAdd(CreateLipstickData(recyclable: recyclable, suggestedPrice: suggestedPrice));

            var expected = CreateFailedQualityCheckResponse();
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        void Lipstick_whose_suggested_price_greater_than_20_and_weight_greater_then_zero_and_less_than_10_produces_failed_quality_check_response()
        {
            var suggestedPrice = 20 + 1;
            var weight = 5;

            var actual = CreateSut().ValidateAndAdd(CreateLipstickData(weight: weight, suggestedPrice: suggestedPrice));

            var expected = CreateFailedQualityCheckResponse();
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        void Lipstick_is_saved_to_db()
        {
            var type = "Lipstick";

            var db = new FakeDatabase();
            CreateSut(db).ValidateAndAdd(CreateData(type: type));
            var actual = db.Product.ToString();

            var expected = CreateLipstick().ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        void Lipstick_has_professional_range_if_suggested_price_greater_than_10()
        {
            var suggestedPrice = 10 + 1;

            var db = new FakeDatabase();
            CreateSut(db).ValidateAndAdd(CreateLipstickData(suggestedPrice: suggestedPrice));
            var actual = db.Product.Range;

            var expected = ProductRange.PROFESSIONAL;
            Assert.Equal(actual, expected);
        }

        [Fact]
        void Mascara_is_saved_to_db()
        {
            var type = "Mascara";

            var db = new FakeDatabase();
            CreateSut(db).ValidateAndAdd(CreateData(type: type));
            var actual = db.Product.ToString();

            var expected = CreateMascara().ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        void Mascara_has_professional_range_if_suggested_price_greater_than_15()
        {
            var suggestedPrice = 15 + 1;

            var db = new FakeDatabase();
            CreateSut(db).ValidateAndAdd(CreateMascaraData(suggestedPrice: suggestedPrice));
            var actual = db.Product.Range;

            var expected = ProductRange.PROFESSIONAL;
            Assert.Equal(expected, actual);
        }

        [Fact]
        void Mascara_has_queen_range_if_recyclable_and_suggested_price_greater_than_25()
        {
            var recyclable = true;
            var suggestedPrice = 25 + 1;

            var db = new FakeDatabase();
            CreateSut(db).ValidateAndAdd(CreateMascaraData(suggestedPrice: suggestedPrice, recyclable: recyclable));
            var actual = db.Product.Range;

            var expected = ProductRange.QUEEN;
            Assert.Equal(expected, actual);
        }

        [Fact]
        void Blusher_is_saved_to_db()
        {
            var type = "Blusher";

            var db = new FakeDatabase();
            CreateSut(db).ValidateAndAdd(CreateData(type: type));
            var actual = db.Product.ToString();

            var expected = CreateBlusher().ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        void Blusher_produces_error_response_if_weight_greater_than_10()
        {
            var weight = 10 + 1;

            var actual = CreateSut().ValidateAndAdd(CreateBlusherData(weight));

            var expected = new Response(0, -3, "Error - weight too high");
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        void Unknown_product_produces_unknown_product_type_response()
        {
            var productType = "Unknown";

            var actual = CreateSut().ValidateAndAdd(CreateData(type: productType));

            var expected = new Response(0, -1, "Unknown product type " + productType);
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        void Foundation_is_saved_to_db()
        {
            var type = "Foundation";

            var db = new FakeDatabase();
            CreateSut(db).ValidateAndAdd(CreateData(type: type));
            var actual = db.Product.ToString();

            var expected = CreateFoundation().ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        void Eyeshadow_is_saved_to_db()
        {
            var type = "Eyeshadow";

            var db = new FakeDatabase();
            CreateSut(db).ValidateAndAdd(CreateData(type: type));
            var actual = db.Product.ToString();

            var expected = CreateEyeshadow().ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        void Eyeshadow_has_professional_range_if_is_recyclable()
        {
            var recyclable = true;

            var db = new FakeDatabase();
            CreateSut(db).ValidateAndAdd(CreateEyeshadowData(recyclable: recyclable));
            var actual = db.Product.Range;

            var expected = ProductRange.PROFESSIONAL;
            Assert.Equal(expected, actual);
        }

        [Fact]
        void Eyeshadow_produces_successful_add_response()
        {
            var actual = CreateSut().ValidateAndAdd(CreateEyeshadowData());

            var expected = new Response(0, 0, "Product Successfully Added");
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        void Lipgloss_is_saved_to_db()
        {
            var type = "Lipgloss";
            var family = ProductFamily.LIPS;

            var db = new FakeDatabase();
            CreateSut(db).ValidateAndAdd(CreateData(type: type));
            var actual = db.Product.ToString();

            var expected = CreateProduct(type, family: family).ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        void Lipgloss_with_price_greater_than_10_produces_error()
        {
            var price = 10 + 1;

            var db = new FakeDatabase();
            var actual = CreateSut(db).ValidateAndAdd(CreateLipglossData(suggestedPrice: price));

            var expected = CreateFailedQualityCheckResponse();
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        void Lipgloss_produces_error_if_weight_greater_than_20()
        {
            var weight = 20 + 1;

            var actual = CreateSut().ValidateAndAdd(CreateLipglossData(weight));

            var expected = new Response(0, -3, "Error - weight too high");
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        void ValidateAndAdd() {
            // Arrange
            var productData = new ProductFormData("Sample product",
                "Lipstick", 5D, 10D, false);
            var db = new FakeDatabase();
            var sut = new ProductService(db);

            // Act
            var response = sut.ValidateAndAdd(productData);

            // Assert
            var productString = "";
            if (db.Product != null) {
                productString = db.Product.ToString();
            }
            var responseAndProduct = response + " " + productString;
            Approvals.Verify(responseAndProduct);

        }

        private static Product CreateProduct(string type = null, ProductFamily? family = null, ProductRange? range = null)
        {
            var result = new Product("Foo");
            result.Type = type;
            result.Family = family ?? default;
            result.Range = range ?? ProductRange.BUDGET;
            result.Weight = 1;
            return result;
        }

        private static Product CreateEyeshadow(ProductRange? range = null)
        {
            return CreateProduct(type: "Eyeshadow", family: ProductFamily.EYES, range: range);
        }

        private static Product CreateFoundation(ProductRange? range = null)
        {
            return CreateProduct(type: "Foundation", family: ProductFamily.SKIN, range: range);
        }

        private static Product CreateLipstick(ProductRange? range = null)
        {
            return CreateProduct(type: "Lipstick", family: ProductFamily.LIPS, range: range);
        }

        private static Product CreateMascara(ProductRange? range = null)
        {
            return CreateProduct(type: "Mascara", family: ProductFamily.LASHES, range: range);
        }

        private static Product CreateBlusher(ProductRange? range = null)
        {
            return CreateProduct(type: "Blusher", family: ProductFamily.SKIN, range: range);
        }

        private static ProductService CreateSut(FakeDatabase db = null)
        {
            return new ProductService(db ?? new FakeDatabase());
        }

        private static ProductFormData CreateData(string name = null, string type = null, double? weight = null, bool? recyclable = null, double? suggestedPrice = null)
        {
            return new ProductFormData(name ?? "Foo", type ?? "Bar", weight ?? 1, suggestedPrice ?? default, recyclable ?? default);
        }

        private static ProductFormData CreateEyeshadowData(bool? recyclable = null)
        {
            return CreateData(type: "Eyeshadow", recyclable: recyclable);
        }

        private static ProductFormData CreateLipstickData(double? weight = null, bool? recyclable = null, double? suggestedPrice = null)
        {
            return CreateData(type: "Lipstick", weight: weight, recyclable: recyclable, suggestedPrice: suggestedPrice);
        }

        private static ProductFormData CreateMascaraData(bool? recyclable = null, double? suggestedPrice = null)
        {
            return CreateData(type: "Mascara", recyclable: recyclable, suggestedPrice: suggestedPrice);
        }

        private static ProductFormData CreateBlusherData(double? weight = null)
        {
            return CreateData(type: "Blusher", weight: weight);
        }

        private static ProductFormData CreateLipglossData(double? weight = null, double? suggestedPrice = null)
        {
            return CreateData(type: "Lipgloss", weight: weight, suggestedPrice: suggestedPrice);
        }

        private static Response CreateFailedQualityCheckResponse()
        {
            return new Response(0, -1, "Error - failed quality check for Queen Range");
        }
    }
}
