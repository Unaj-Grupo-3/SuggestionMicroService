using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class PreferenceAPIServices
    {
        private readonly string _url;
        private readonly HttpClient _httpClient;
        private readonly string _key;

        public PreferenceAPIServices(HttpClient httpClient)
        {
            _url = ;
            _httpClient = httpClient;
            _key = "";
        }
    }
}
