
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using my_customers_cosmos_db_C_.Exceptions;
using my_customers_cosmos_db_C_.Models;
using static my_customers_cosmos_db_C_.Models.CreateCustomerResponseModel;

namespace my_customers_cosmos_db_C_.Services
{

    public class CustomerService
    {
        private readonly Container _container;

        public CustomerService(CosmosClient client)
        {
            _container = client.GetContainer("Customers", "supermarkets");
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
            catch (Exception ex)
            {
                throw new BadRequestException($"Error saving CosmosDb {ex}");

            }
        }

        public async Task <GetCustomerByIdResponseModel> GetCustomerById (string customerId, string partitionKey)
        {
            var sendPartitionKey = new PartitionKey(partitionKey);

            try
            {
                var customer = await _container.ReadItemAsync<GetCustomerByIdResponseModel>(customerId, sendPartitionKey);

                return customer;
            }
            catch (Exception ex)
            {
                throw new NotFoundException($"customer not exists {ex}");           
            }
        }
    }
}

