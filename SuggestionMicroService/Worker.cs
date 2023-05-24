using Application.Interfaces;
using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace SuggestionMicroService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IScopedProcessingService _processingService;
        private readonly ISuggestionWorkerServices _suggestionWorkerServices;

        public Worker
        (
            ILogger<Worker> logger, 
            ISuggestionWorkerServices suggestionWorkerServices,
            IScopedProcessingService processingService
            )
        {
            _logger = logger;
            _suggestionWorkerServices = suggestionWorkerServices;
            _processingService = processingService;


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _suggestionWorkerServices.GenerateSuggestionAll();
                //_processingService.DoWorkAsync(stoppingToken);
                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}