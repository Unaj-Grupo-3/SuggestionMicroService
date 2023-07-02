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
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    DateTimeOffset init = DateTimeOffset.Now;
                    _logger.LogInformation("Worker running at: {time}", init);
                    //await _suggestionWorkerServices.DeleteSuggestionsAll();
                    // Calcula nuevas sugerencias cuando la cantidad de sugerencias calculada baja del numero informado por parametros:
                    var countUsers = await _suggestionWorkerServices.CountSuggestionsUsers(5);
                    foreach (var user in countUsers)
                    {
                        await _suggestionWorkerServices.GenerateSuggestionXUser(user);
                    }

                    // Calcula sugerencias para los usuarios nuevos:
                    var usersNew = await _suggestionWorkerServices.UsersNew();
                    foreach (var user in usersNew)
                    {
                        await _suggestionWorkerServices.GenerateSuggestionXUser(user);
                    }
                    DateTimeOffset end = DateTimeOffset.Now;
                    _logger.LogInformation("Worker ending at: {time}", end);

                    Console.WriteLine("************************************************************************************************************************");
                    Console.WriteLine(init - end);
                    Console.WriteLine("************************************************************************************************************************");
                    // Ejecuta cada 2 minutos.
                    await Task.Delay(60000 * 1, stoppingToken);
                }
            }
            catch(Exception e)
            {
                _logger.LogInformation($"Error en el Worker: {e}");
            }
            
        }
    }
}