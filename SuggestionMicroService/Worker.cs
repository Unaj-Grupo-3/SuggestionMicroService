using Application.Interfaces;
using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace SuggestionMicroService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISuggestionWorkerServices _suggestionWorkerServices;

        public Worker(ILogger<Worker> logger, ISuggestionWorkerServices suggestionWorkerServices)
        {
            _logger = logger;
            _suggestionWorkerServices = suggestionWorkerServices;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _suggestionWorkerServices.GenerateSuggestionAll();
                // Ejecuta cada 30 minutos.
                await Task.Delay(60000 * 30, stoppingToken);
            }
        }
    }
}