using Microsoft.Azure.Cosmos;
using my_customers_cosmos_db_C_.Models;
using static my_customers_cosmos_db_C_.Models.CreateCustomerResponseModel;

namespace my_customers_cosmos_db_C_.Services
{

    public class CustomerService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public CustomerService(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer("Supermarkets", "StoreId"); // Substitua "storeId" pelo nome da sua partição se necessário
        }

        public async Task<CreateCustomerResponseModel> CreateCustomer(CreateCustomerRequestModel model)
        {
            if (model != null && model.StoreId != 001)
                throw new InvalidOperationException("Invalid StoreId");

            var listAddress = new List<CreateCustomerAddressResponseModel>();

            foreach (var address in model.CustomerAddresses)
            {
                var customerAddress = new CreateCustomerAddressResponseModel
                {
                    StoreId = address.StoreId,
                    Street = address.Street,
                    Neighborhood = address.Neighborhood,
                    HouseNumber = address.HouseNumber,
                };

                listAddress.Add(customerAddress);
            }

            var customer = new CreateCustomerResponseModel()
            {
                Id = Guid.NewGuid().ToString(),
                StoreId = model.StoreId,
                Document = model.Document,
                DocumentType = model.DocumentType,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedDate = model.CreatedDate,
                CustomerAddresses = listAddress
            };

            // Salvar o cliente no Cosmos DB
            var partitionKey = new PartitionKey(model.StoreId.ToString()); // Se "storeId" for sua partição

            await _container.CreateItemAsync(customer, partitionKey);

            return customer;
        }

    }
}
