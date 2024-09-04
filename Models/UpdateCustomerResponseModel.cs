using Newtonsoft.Json;

namespace my_customers_cosmos_db_C_.Models
{
    public class UpdateCustomerResponseModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("document")]
        public long Document { get; set; }

        [JsonProperty("documentType")]
        public string DocumentType { get; set; }

        [JsonProperty("customerAddresses")]
        public List<UpdateCustomerAddressResponseModel>? CustomerAddresses { get; set; }
    }

    public class UpdateCustomerAddressResponseModel
    {
        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [JsonProperty("street")]
        public string? Street { get; set; }

        [JsonProperty("neighborhood")]
        public string? Neighborhood { get; set; }

        [JsonProperty("houseNumber")]
        public int? HouseNumber { get; set; }
    }
}
