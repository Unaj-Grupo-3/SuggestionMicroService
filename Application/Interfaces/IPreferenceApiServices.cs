﻿
using System.Text.Json;

namespace Application.Interfaces
{
    public interface IPreferenceApiServices
    {
        Task<JsonDocument> GetAllPreference();

        Task<JsonDocument> GetPreferencesByList(List<int> preferenceIds);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
