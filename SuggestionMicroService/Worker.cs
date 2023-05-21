using Application.Interfaces;
using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace SuggestionMicroService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IUserApiServices _userApiServices;
        private readonly IMatchApiServices _matchApiServices;
        private readonly IPreferenceApiServices _preferenceApiServices;
        private readonly ISuggestionWorkerServices _suggestionWorkerServices;

        public Worker
        (
            ILogger<Worker> logger, 
            IUserApiServices user, 
            IPreferenceApiServices preference, 
            IMatchApiServices match, 
            ISuggestionWorkerServices suggestionWorkerServices
            )
        {
            _logger = logger;
            _userApiServices = user;
            _preferenceApiServices = preference;
            _matchApiServices = match;
            _suggestionWorkerServices = suggestionWorkerServices;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}