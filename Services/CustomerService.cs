using Microsoft.Azure.Cosmos;
using my_customers_cosmos_db_C_.Models;
using Newtonsoft.Json;
using static my_customers_cosmos_db_C_.Models.CreateCustomerResponseModel;

namespace my_customers_cosmos_db_C_.Services
{

    public class CustomerService
    {
        private readonly Container _container;

        public CustomerService(CosmosClient client)
        {
            _container = client.GetContainer("Customers", "Supermarkets");
        }

        public async Task<CreateCustomerResponseModel> CreateCustomer(CreateCustomerRequestModel model)
        {
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

            var customer = new CreateCustomerResponseModel
            {
                Id = Guid.NewGuid().ToString(),
                StoreId = model.StoreId,
                Document = model.Document,
                DocumentType = model.DocumentType,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedDate = DateTime.UtcNow,
                CustomerAddresses = listAddress
            };

            var partitionKey = new PartitionKey(model.StoreId); // Definindo a chave de partição

            try
            {
                await _container.CreateItemAsync(customer, partitionKey);
                return customer;
            }
            catch (CosmosException ex)
            {
                // Tratar exceção
                Console.WriteLine($"Error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
