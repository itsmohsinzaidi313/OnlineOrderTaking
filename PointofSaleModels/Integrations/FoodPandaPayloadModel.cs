using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.Linq;
using PointofSaleModels.Integrations;
using PointofSaleModels.Integrations.Common;

namespace PointofSaleModels.Integrations
{

    public partial class FoodPandaPayloadModel : PayloadModel
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("shortCode")]
        [JsonConverter(typeof(ParseStringToLongConverter))]
        public long ShortCode { get; set; }

        [JsonPropertyName("preOrder")]
        public bool PreOrder { get; set; }

        [JsonPropertyName("expiryDate")]
        public DateTimeOffset ExpiryDate { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("localInfo")]
        public LocalInfo? LocalInfo { get; set; }


        [JsonPropertyName("platformRestaurant")]
        public PlatformRestaurant? PlatformRestaurant { get; set; }


        [JsonPropertyName("customer")]
        public Customer? Customer { get; set; }


        [JsonPropertyName("payment")]
        public Payment? Payment { get; set; }


        [JsonPropertyName("expeditionType")]
        public string? ExpeditionType { get; set; }


        [JsonPropertyName("products")]
        public Product[]? Products { get; set; }


        [JsonPropertyName("corporateTaxId")]
        public string? CorporateTaxId { get; set; }


        [JsonPropertyName("comments")]
        public Comments? Comments { get; set; }


        [JsonPropertyName("vouchers")]
        public object[]? Vouchers { get; set; }


        [JsonPropertyName("discounts")]
        public Discount[]? Discounts { get; set; }


        [JsonPropertyName("price")]
        public Price? Price { get; set; }


        [JsonPropertyName("webOrder")]
        public bool WebOrder { get; set; }

        [JsonPropertyName("mobileOrder")]
        public bool MobileOrder { get; set; }

        [JsonPropertyName("corporateOrder")]
        public bool CorporateOrder { get; set; }

        [JsonPropertyName("integrationInfo")]
        public IntegrationInfo? IntegrationInfo { get; set; }


        [JsonPropertyName("test")]
        public bool Test { get; set; }

        [JsonPropertyName("delivery")]
        public Delivery? Delivery { get; set; }


        [JsonPropertyName("callbackUrls")]
        public CallbackUrls? CallbackUrls { get; set; }


        public PayloadModel Payload
        {
            get
            {
                var obj = Delivery?.Address;
                var address = obj?.Street + obj?.Number;
                var instructions = obj?.DeliveryInstructions;
                return new PayloadModel
                {
                    OrderId = Code,
                    OrderStatus = ExpeditionType,
                    OrderType = ExpeditionType,
                    OSPId = string.Empty,
                    PaymentStatus = Payment.Status,

                    Customer = new PayloadCustomerModel
                    {
                        Id = Customer.Id,
                        Address = address,
                        PhoneNumber = Customer.MobilePhone,
                        FirstName = Customer.FirstName,
                        LastName = Customer.LastName,
                        Landmark = string.Empty,
                        Instructions = instructions ?? string.Empty,
                        AlternateNumber = string.Empty,

                    },
                    Products = [.. (from Product x in Products
                                select new PayloadProductModel
                                {
                                    Comments = x.Comment,
                                    DiscountedPrice = x.DiscountAmount,
                                    Name = x.Name,
                                    POSCode = x.RemoteCode,
                                    Price = x.UnitPrice.ToString(),
                                    Quantity = x.Quantity.ToString(),
                                })],
                    PaymentDetail = new()
                    {
                        DeliveryCharges = Price.DeliveryFee.ToString(),
                        DiscountAmount = Price.DiscountAmountTotal.ToString(),
                        GrandTotal = Price.GrandTotal,
                        TaxAmount = Price.ServiceTaxValue.ToString(),
                        TaxPercentage = Price.ServiceTax.ToString(),
                    }
                };
            }
        }
    }

    public partial class CallbackUrls
    {
        [JsonPropertyName("orderAcceptedUrl")]
        public Uri? OrderAcceptedUrl { get; set; }


        [JsonPropertyName("orderRejectedUrl")]
        public Uri? OrderRejectedUrl { get; set; }


        [JsonPropertyName("orderPickedUpUrl")]
        public Uri? OrderPickedUpUrl { get; set; }


        [JsonPropertyName("orderPreparedUrl")]
        public Uri? OrderPreparedUrl { get; set; }

    }

    public partial class Comments
    {
        [JsonPropertyName("customerComment")]
        public string? CustomerComment { get; set; }


        [JsonPropertyName("vendorComment")]
        public string? VendorComment { get; set; }

    }

    public partial class Customer
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }


        [JsonPropertyName("code")]
        public string? Code { get; set; }


        [JsonPropertyName("mobilePhone")]
        public string? MobilePhone { get; set; }


        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }


        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }


        [JsonPropertyName("email")]
        public string? Email { get; set; }


        [JsonPropertyName("mobilePhoneCountryCode")]
        public string? MobilePhoneCountryCode { get; set; }


        [JsonPropertyName("flags")]
        public object[]? Flags { get; set; }

    }

    public partial class Delivery
    {
        [JsonPropertyName("expressDelivery")]
        public bool ExpressDelivery { get; set; }

        [JsonPropertyName("expectedDeliveryTime")]
        public DateTime ExpectedDeliveryTime { get; set; }

        [JsonPropertyName("riderPickupTime")]
        public DateTime? RiderPickupTime { get; set; }

        [JsonPropertyName("address")]
        public Address? Address { get; set; }

    }

    public partial class Address
    {
        [JsonPropertyName("line1")]
        public string? Line1 { get; set; }


        [JsonPropertyName("line2")]
        public string? Line2 { get; set; }


        [JsonPropertyName("line3")]
        public string? Line3 { get; set; }


        [JsonPropertyName("line4")]
        public string? Line4 { get; set; }


        [JsonPropertyName("line5")]
        public string? Line5 { get; set; }


        [JsonPropertyName("street")]
        public string? Street { get; set; }


        [JsonPropertyName("number")]
        public string? Number { get; set; }


        [JsonPropertyName("room")]
        public string? Room { get; set; }


        [JsonPropertyName("flatNumber")]
        public string? FlatNumber { get; set; }


        [JsonPropertyName("building")]
        public string? Building { get; set; }


        [JsonPropertyName("intercom")]
        public string? Intercom { get; set; }


        [JsonPropertyName("entrance")]
        public string? Entrance { get; set; }


        [JsonPropertyName("structure")]
        public string? Structure { get; set; }


        [JsonPropertyName("floor")]
        public string? Floor { get; set; }

        [JsonPropertyName("district")]
        public string? District { get; set; }


        [JsonPropertyName("other")]
        public string? Other { get; set; }


        [JsonPropertyName("city")]
        public string? City { get; set; }


        [JsonPropertyName("postcode")]
        public string? Postcode { get; set; }


        [JsonPropertyName("company")]
        public string? Company { get; set; }


        [JsonPropertyName("deliveryMainArea")]
        public string? DeliveryMainArea { get; set; }


        [JsonPropertyName("deliveryMainAreaPostcode")]
        public string? DeliveryMainAreaPostcode { get; set; }


        [JsonPropertyName("deliveryArea")]
        public string? DeliveryArea { get; set; }


        [JsonPropertyName("deliveryAreaPostcode")]
        public string? DeliveryAreaPostcode { get; set; }


        [JsonPropertyName("deliveryInstructions")]
        public string? DeliveryInstructions { get; set; }


        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }

    public partial class IntegrationInfo
    {
    }

    public partial class LocalInfo
    {
        [JsonPropertyName("platform")]
        public string? Platform { get; set; }


        [JsonPropertyName("platformKey")]
        public string? PlatformKey { get; set; }


        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }


        [JsonPropertyName("currencySymbol")]
        public string? CurrencySymbol { get; set; }


        [JsonPropertyName("currencySymbolPosition")]
        public string? CurrencySymbolPosition { get; set; }


        [JsonPropertyName("currencySymbolSpaces")]
        public string? CurrencySymbolSpaces { get; set; }


        [JsonPropertyName("decimalSeparator")]
        public string? DecimalSeparator { get; set; }


        [JsonPropertyName("decimalDigits")]
        public string? DecimalDigits { get; set; }


        [JsonPropertyName("thousandsSeparator")]
        public string? ThousandsSeparator { get; set; }


        [JsonPropertyName("website")]
        public string? Website { get; set; }


        [JsonPropertyName("email")]
        public string? Email { get; set; }


        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

    }

    public partial class Payment
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }


        [JsonPropertyName("remoteCode")]
        public string? RemoteCode { get; set; }


        [JsonPropertyName("status")]
        public string? Status { get; set; }


        [JsonPropertyName("requiredMoneyChange")]
        public string? RequiredMoneyChange { get; set; }


        [JsonPropertyName("vatName")]
        public string? VatName { get; set; }


        [JsonPropertyName("vatId")]
        public string? VatId { get; set; }

    }

    public partial class PlatformRestaurant
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

    }

    public partial class Price
    {
        [JsonPropertyName("minimumDeliveryValue")]
        public string? MinimumDeliveryValue { get; set; }


        [JsonPropertyName("comission")]
        public string? Comission { get; set; }


        [JsonPropertyName("deliveryFee")]
        public string? DeliveryFee { get; set; }


        [JsonPropertyName("deliveryFees")]
        public DeliveryFee[]? DeliveryFees { get; set; }


        [JsonPropertyName("containerCharge")]
        public string? ContainerCharge { get; set; }


        [JsonPropertyName("deliveryFeeDiscount")]
        public string? DeliveryFeeDiscount { get; set; }


        [JsonPropertyName("serviceFeePercent")]
        public string? ServiceFeePercent { get; set; }


        [JsonPropertyName("serviceFeeTotal")]
        public string? ServiceFeeTotal { get; set; }


        [JsonPropertyName("serviceTax")]
        public decimal ServiceTax { get; set; }

        [JsonPropertyName("serviceTaxValue")]
        public decimal ServiceTaxValue { get; set; }

        [JsonPropertyName("subTotal")]
        [JsonConverter(typeof(ParseStringToDecimalConverter))]
        public decimal SubTotal { get; set; }

        [JsonPropertyName("totalNet")]
        [JsonConverter(typeof(ParseStringToDecimalConverter))]
        public decimal TotalNet { get; set; }

        [JsonPropertyName("vatVisible")]
        public bool VatVisible { get; set; }

        [JsonPropertyName("vatPercent")]
        public string? VatPercent { get; set; }


        [JsonPropertyName("vatTotal")]
        [JsonConverter(typeof(ParseStringToDecimalConverter))]
        public decimal VatTotal { get; set; }

        [JsonPropertyName("grandTotal")]
        public string? GrandTotal { get; set; }


        [JsonPropertyName("discountAmountTotal")]
        [JsonConverter(typeof(ParseStringToDecimalConverter))]
        public decimal DiscountAmountTotal { get; set; }

        [JsonPropertyName("differenceToMinimumDeliveryValue")]
        public string? DifferenceToMinimumDeliveryValue { get; set; }


        [JsonPropertyName("payRestaurant")]
        [JsonConverter(typeof(ParseStringToDecimalConverter))]
        public decimal PayRestaurant { get; set; }

        [JsonPropertyName("collectFromCustomer")]
        public string? CollectFromCustomer { get; set; }


        [JsonPropertyName("riderTip")]
        [JsonConverter(typeof(ParseStringToDecimalConverter))]
        public decimal RiderTip { get; set; }
    }

    public partial class DeliveryFee
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }


        [JsonPropertyName("value")]
        public double Value { get; set; }
    }

    public partial class Product
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(ParseStringToLongConverter))]
        public long Id { get; set; }

        [JsonPropertyName("remoteCode")]
        public string? RemoteCode { get; set; }


        [JsonPropertyName("name")]
        public string? Name { get; set; }


        [JsonPropertyName("description")]
        public string? Description { get; set; }


        [JsonPropertyName("comment")]
        public string? Comment { get; set; }


        [JsonPropertyName("categoryName")]
        public string? CategoryName { get; set; }


        [JsonPropertyName("variation")]
        public Variation? Variation { get; set; }


        [JsonPropertyName("unitPrice")]
        [JsonConverter(typeof(ParseStringToDecimalConverter))]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("paidPrice")]
        [JsonConverter(typeof(ParseStringToDecimalConverter))]
        public decimal PaidPrice { get; set; }

        [JsonPropertyName("discountAmount")]
        public string? DiscountAmount { get; set; }


        [JsonPropertyName("quantity")]
        [JsonConverter(typeof(ParseStringToLongConverter))]
        public long Quantity { get; set; }

        [JsonPropertyName("halfHalf")]
        public bool HalfHalf { get; set; }

        [JsonPropertyName("vatPercentage")]
        public string? VatPercentage { get; set; }


        [JsonPropertyName("selectedChoices")]
        public object[]? SelectedChoices { get; set; }


        [JsonPropertyName("selectedToppings")]
        public SelectedTopping[]? SelectedToppings { get; set; }


        [JsonPropertyName("discounts")]
        public Discount[]? Discounts { get; set; }

    }

    public partial class Variation
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

    }

    public partial class Discount
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }


        [JsonPropertyName("amount")]
        public string? Amount { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("type")]
        public string? Type { get; set; }


        [JsonPropertyName("sponsorships")]
        public Sponsorship[]? Sponsorships { get; set; }

    }

    public partial class SelectedTopping
    {
        [JsonPropertyName("children")]
        public object[]? Children { get; set; }


        [JsonPropertyName("name")]
        public string? Name { get; set; }


        [JsonPropertyName("price")]
        public string? Price { get; set; }


        [JsonPropertyName("quantity")]
        public long Quantity { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }


        [JsonPropertyName("remoteCode")]
        public string? RemoteCode { get; set; }


        [JsonPropertyName("type")]
        public string? Type { get; set; }


        [JsonPropertyName("discounts")]
        public Discount[]? Discounts { get; set; }

    }

    public partial class Sponsorship
    {
        [JsonPropertyName("sponsor")]
        public string? Sponsor { get; set; }


        [JsonPropertyName("amount")]
        public string? Amount { get; set; }

    }

    public partial class FoodPandaPayloadModel
    {
        public static FoodPandaPayloadModel? FromJson(string json) => JsonSerializer.Deserialize<FoodPandaPayloadModel>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this FoodPandaPayloadModel self) => JsonSerializer.Serialize(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General)
        {
            Converters =
            {
                new DateOnlyConverter(),
                new TimeOnlyConverter(),
                IsoDateTimeOffsetConverter.Singleton
            },
        };
    }

    internal class ParseStringToLongConverter : JsonConverter<long>
    {
        public override bool CanConvert(Type t) => t == typeof(long);

        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.ToString(), options);
            return;
        }

        public static readonly ParseStringToLongConverter Singleton = new ParseStringToLongConverter();
    }

    internal class ParseStringToDecimalConverter : JsonConverter<decimal>
    {
        public override bool CanConvert(Type t) => t == typeof(decimal);

        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            decimal l;
            if (decimal.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type decimal");
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.ToString(), options);
            return;
        }

        public static readonly ParseStringToLongConverter Singleton = new ParseStringToLongConverter();
    }

    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private readonly string serializationFormat;
        public DateOnlyConverter() : this(null) { }

        public DateOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
        }

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.ToString(serializationFormat));
    }

    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        private readonly string serializationFormat;

        public TimeOnlyConverter() : this(null) { }

        public TimeOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
        }

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return TimeOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.ToString(serializationFormat));
    }

    internal class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
        private string? _dateTimeFormat;
        private CultureInfo? _culture;

        public DateTimeStyles DateTimeStyles
        {
            get => _dateTimeStyles;
            set => _dateTimeStyles = value;
        }

        public string? DateTimeFormat
        {
            get => _dateTimeFormat ?? string.Empty;
            set => _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value;
        }

        public CultureInfo Culture
        {
            get => _culture ?? CultureInfo.CurrentCulture;
            set => _culture = value;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            string text;


            if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                    || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
            {
                value = value.ToUniversalTime();
            }

            text = value.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);

            writer.WriteStringValue(text);
        }

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? dateText = reader.GetString();

            if (string.IsNullOrEmpty(dateText) == false)
            {
                if (!string.IsNullOrEmpty(_dateTimeFormat))
                {
                    return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
                }
                else
                {
                    return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
                }
            }
            else
            {
                return default(DateTimeOffset);
            }
        }


        public static readonly IsoDateTimeOffsetConverter Singleton = new IsoDateTimeOffsetConverter();
    }
}
