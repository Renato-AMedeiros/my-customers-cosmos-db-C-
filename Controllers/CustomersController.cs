using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using my_customers_cosmos_db_C_.Models;
using my_customers_cosmos_db_C_.Services;

namespace my_customers_cosmos_db_C_.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {

        private readonly CustomerService _customerService;

        public CustomersController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequestModel model)
        {
            var response = await _customerService.CreateCustomer(model);

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromQuery] string customerId, [FromBody] UpdateCustomerRequestModel model)
        {
            var response = await _customerService.UpdateCustomer(customerId, model);
      
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerById([FromQuery] string customerId, [FromQuery] string partitionKey)
        {
            var response = await _customerService.GetCustomerById(customerId, partitionKey);

            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCustomerById([FromQuery] string customerId, [FromQuery] string partitionKey)
        {
             await _customerService.DeleteCustomerById(customerId, partitionKey);

            return NoContent();
        }

    }
}
