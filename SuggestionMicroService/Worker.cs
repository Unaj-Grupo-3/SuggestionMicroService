using Application.Interfaces;

namespace SuggestionMicroService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IUserApiServices _userApiServices;
        private readonly IMatchApiServices _matchApiServices;
        private readonly IPreferenceApiServices _preferenceApiServices;


        public Worker(ILogger<Worker> logger, IUserApiServices user, IPreferenceApiServices preference, IMatchApiServices match)
        {
            _logger = logger;
            _userApiServices = user;
            _preferenceApiServices = preference;
            _matchApiServices = match;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}