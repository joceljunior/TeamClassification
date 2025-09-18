using Microsoft.AspNetCore.SignalR;
using TeamClassification.Hubs;
using TeamClassification.Data;
using Microsoft.EntityFrameworkCore;

namespace TeamClassification.Services
{
    public class TimerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TimerBackgroundService> _logger;

        public TimerBackgroundService(IServiceProvider serviceProvider, ILogger<TimerBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TimerBackgroundService iniciado");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<ClassificationHub>>();

                    // Obter todos os participantes ativos
                    var participants = await context.Participants
                        .Where(p => p.StartTime.HasValue && p.InitialTime > TimeSpan.Zero)
                        .ToListAsync();

                    _logger.LogDebug($"Encontrados {participants.Count} participantes ativos");

                    var expiredParticipants = new List<int>();

                    foreach (var participant in participants)
                    {
                        var elapsed = DateTime.Now - participant.StartTime.Value;
                        var remaining = participant.InitialTime - elapsed;

                        if (remaining <= TimeSpan.Zero)
                        {
                            expiredParticipants.Add(participant.Id);
                        }
                    }

                    // Notificar clientes sobre participantes que expiraram
                    if (expiredParticipants.Any())
                    {
                        _logger.LogInformation($"Participantes expirados: {string.Join(", ", expiredParticipants)}");
                        await hubContext.Clients.All.SendAsync("ParticipantsExpired", expiredParticipants);
                    }

                    // Enviar atualização geral do tempo para todos os clientes
                    await hubContext.Clients.All.SendAsync("TimeUpdate", DateTime.Now);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no serviço de timer em background");
                }

                // Aguardar 1 segundo antes da próxima atualização
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
