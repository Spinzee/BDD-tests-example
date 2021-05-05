namespace Products.ServiceWrapper.BroadbandProductsService
{
    using Newtonsoft.Json;

    public class BroadbandProductAddress
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("districtId")]
        public string DistrictId { get; set; }

        [JsonProperty("locality")]
        public string Locality { get; set; }

        [JsonProperty("postCode")]
        public string Postcode { get; set; }

        [JsonProperty("postTown")]
        public string PostTown { get; set; }

        [JsonProperty("premiseName")]
        public string PremiseName { get; set; }

        [JsonProperty("subPremises")]
        public string SubPremises { get; set; }

        [JsonProperty("thoroughfareName")]
        public string ThoroughfareName { get; set; }

        [JsonProperty("thoroughfareNumber")]
        public string ThoroughfareNumber { get; set; }

        [JsonProperty("parentUprn")]
        public string ParentUPRN { get; set; }

        [JsonProperty("uprn")]
        public string UPRN { get; set; }

        [JsonProperty("formatted")]
        public string Formatted { get; set; }
    }

    public class ProductsResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("lineSpeeds")]
        public LineSpeed[] LineSpeeds { get; set; }

        [JsonProperty("broadbandProducts")]
        public BroadbandProducts BroadbandProducts { get; set; }

        [JsonProperty("customerAddress")]
        public CustomerAddress[] CustomerAddress { get; set; }
    }

    public class BroadbandProducts
    {
        [JsonProperty("broadband")]
        public Broadband Broadband { get; set; }
    }

    public class Broadband
    {
        [JsonProperty("brand")]
        public Brand Brand { get; set; }
    }

    public class Brand
    {
        [JsonProperty("brandId")]
        public string BrandId { get; set; }

        [JsonProperty("tariffs")]
        public Tariff[] Tariffs { get; set; }

        [JsonProperty("options")]
        public Options Options { get; set; }
    }

    public class Options
    {
        [JsonProperty("callFeature")]
        public CallFeature CallFeature { get; set; }
    }

    public class CallFeature
    {
        [JsonProperty("callLineFeature")]
        public string CallLineFeature { get; set; }

        [JsonProperty("callLineFeatureCode")]
        public string CallLineFeatureCode { get; set; }
    }

    public class Tariff
    {
        [JsonProperty("tariffName")]
        public string TariffName { get; set; }

        [JsonProperty("tariffTechType")]
        public string TariffTechType { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("broadbandCode")]
        public string BroadbandCode { get; set; }

        [JsonProperty("talkCode")]
        public string TalkCode { get; set; }

        [JsonProperty("priceLines")]
        public PriceLine[] PriceLines { get; set; }

        [JsonProperty("contractDetails")]
        public ContractDetails ContractDetails { get; set; }

        [JsonProperty("estimatedSpeed")]
        public string EstimatedSpeed { get; set; }
    }

    public class ContractDetails
    {
        [JsonProperty("broadbandContractMonths")]
        public long BroadbandContractMonths { get; set; }

        [JsonProperty("talkContractMonths")]
        public long TalkContractMonths { get; set; }
    }

    public class PriceLine
    {
        [JsonProperty("featureDescription")]
        public string FeatureDescription { get; set; }

        [JsonProperty("featureCode")]
        public string FeatureCode { get; set; }

        [JsonProperty("rate")]
        public double Rate { get; set; }
    }

    public class CustomerAddress
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("districtId")]
        public string DistrictId { get; set; }

        [JsonProperty("locality")]
        public object Locality { get; set; }

        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        [JsonProperty("postTown")]
        public string PostTown { get; set; }

        [JsonProperty("premiseName")]
        public object PremiseName { get; set; }

        [JsonProperty("subPremises")]
        public object SubPremises { get; set; }

        [JsonProperty("thoroughfareName")]
        public string ThoroughfareName { get; set; }

        [JsonProperty("thoroughfareNumber")]
        public string ThoroughfareNumber { get; set; }

        [JsonProperty("parentUprn")]
        public object ParentUPRN { get; set; }

        [JsonProperty("uprn")]
        public object UPRN { get; set; }

        [JsonProperty("formatted")]
        public string Formatted { get; set; }
    }

    public class LineSpeed
    {
        [JsonProperty("maxSpeed")]
        public string MaxSpeed { get; set; }

        [JsonProperty("minSpeed")]
        public string MinSpeed { get; set; }

        [JsonProperty("maxDownload")]
        public string MaxDownload { get; set; }

        [JsonProperty("minDownload")]
        public string MinDownload { get; set; }

        [JsonProperty("maxUpload")]
        public string MaxUpload { get; set; }

        [JsonProperty("minUpload")]
        public string MinUpload { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
