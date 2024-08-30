using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateCustomers([FromBody] CreateCustomerRequestModel model )
        {
            var response = _customerService.CreateCustomer(model);

            return Ok(response);
        }
    }
}
