using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserMatchApiServices
    {
        Task<List<UserMatch>> GetAllMatches();

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
