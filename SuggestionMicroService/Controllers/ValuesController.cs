using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SuggestionMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetApi()
        {
            return Ok();
        }
    }
}
