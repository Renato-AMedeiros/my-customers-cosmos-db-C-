﻿using Microsoft.Azure.Cosmos;
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

            var partitionKey = new PartitionKey(model.StoreId);

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

        public async Task<UpdateCustomerResponseModel> UpdateCustomer(string customerId, UpdateCustomerRequestModel model)
        {
            var customer = await GetCustomerById(customerId, model.StoreId);

            var listAddress = new List<UpdateCustomerAddressResponseModel>();

            if (model.CustomerAddresses != null)
            {
                foreach (var address in model.CustomerAddresses)
                {
                    var customerAddress = new UpdateCustomerAddressResponseModel
                    {
                        StoreId = model.StoreId,
                        Street = address.Street,
                        Neighborhood = address.Neighborhood,
                        HouseNumber = address.HouseNumber,
                    };

                    listAddress.Add(customerAddress);
                }
            }

            var updateCustomer = new UpdateCustomerResponseModel
            {
                Id = customerId,
                StoreId = model.StoreId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Document = model.Document,
                DocumentType = model.DocumentType,
                CustomerAddresses = listAddress
            };

            var partitionKey = new PartitionKey(model.StoreId);

            try
            {
                var response = await _container.ReplaceItemAsync<UpdateCustomerResponseModel>(updateCustomer, customerId, partitionKey);
                return response;
            }
            catch (Exception ex)
            {

                throw new BadRequestException($"Error updating in CosmosDb {ex}");
            }
        }

        public async Task<GetCustomerByIdResponseModel> GetCustomerById(string customerId, string partitionKey)
        {
            var sendPartitionKey = new PartitionKey(partitionKey);

            try
            {
                ItemResponse<GetCustomerByIdResponseModel> customer = await _container.ReadItemAsync<GetCustomerByIdResponseModel>(customerId, sendPartitionKey);

                return customer;
            }
            catch (Exception ex)
            {
                throw new NotFoundException($"customer not exists {ex}");
            }
        }

        public async Task<List<GetCustomerListResponseModel>> GetCustomerList()
        {
            var sqlQuery = "SELECT * from c";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);

            FeedIterator<GetCustomerListResponseModel> queryResultSetIterator = _container.GetItemQueryIterator<GetCustomerListResponseModel>(queryDefinition);

            List<GetCustomerListResponseModel> customers = new List<GetCustomerListResponseModel>();

            try
            {
                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<GetCustomerListResponseModel> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (GetCustomerListResponseModel employee in currentResultSet)
                    {
                        customers.Add(employee);
                    }
                }
            }
            catch (Exception ex )
            {

                throw new BadRequestException($"Error get customers in CosmosDb {ex}");
            }

            return customers;
        }

        public async Task DeleteCustomerById(string customerId, string partitionKey)
        {
            var sendPartitionKey = new PartitionKey(partitionKey);

            try
            {
                var customer = await _container.DeleteItemAsync<GetCustomerByIdResponseModel>(customerId, sendPartitionKey);
            }
            catch (Exception ex)
            {
                throw new NotFoundException($"customer not exists {ex}");
            }
        }
    }
}

