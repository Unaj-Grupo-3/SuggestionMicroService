using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace SuggestionMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestionController
    {
        private readonly ISuggestionServices _suggestionServices;
        private readonly ITokenServices _tokenServices;

        public SuggestionController(ISuggestionServices suggestionServices, ITokenServices tokenServices)
        {
            _suggestionServices = suggestionServices;
            _tokenServices = tokenServices;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                IList<SuggestionResponse> response = await _suggestionServices.GetAll();
                return new JsonResult(new { Count = response.Count, Response = response }) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new JsonResult(new { ex.Message }) { StatusCode = 500 };
            }
        }


    }
}
