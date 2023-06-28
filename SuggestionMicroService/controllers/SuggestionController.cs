using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SuggestionMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestionController : ControllerBase
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
                IList<Suggestion> response = await _suggestionServices.GetAll();
                return new JsonResult(new { Count = response.Count, Response = response }) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new JsonResult(new { ex.Message }) { StatusCode = 500 };
            }
        }

        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetSuggestionMe()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int userId = _tokenServices.GetUserId(identity);

                //IList<SuggestionResponse> response = await _suggestionServices.GetSuggestionsByUserId(userId);
                SuggestionResponse response = new();
                response = await _suggestionServices.GetSuggestionsByUserId(userId);

                return new JsonResult(response) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new JsonResult(new { ex.Message }) { StatusCode = 500 };
            }
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteByUserAndSuggested(SuggestionRequest request)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int userId = _tokenServices.GetUserId(identity);

                bool response = await _suggestionServices.DeleteWorkerSuggByUserIdAndUserSuggested(userId, request.UserId);
                return new JsonResult(new { Message = "Se ha eliminado la sugerencia", Response = response }) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new JsonResult(new { ex.Message }) { StatusCode = 500 };
            }
        }
    }
}
