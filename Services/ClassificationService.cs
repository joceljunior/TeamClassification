using Microsoft.EntityFrameworkCore;
using TeamClassification.Data;
using TeamClassification.DTOs;
using TeamClassification.Models;

namespace TeamClassification.Services
{
    public interface IClassificationService
    {
        Task<ClassificationDto> GetClassificationAsync();
        Task<Team> CreateTeamAsync(CreateTeamDto dto);
        Task<Participant> CreateParticipantAsync(CreateParticipantDto dto);
        Task UpdateTeamScoreAsync(int teamId, UpdateScoreDto dto);
        Task UpdateParticipantScoreAsync(int participantId, UpdateScoreDto dto);
        Task<List<ScoreHistoryDto>> GetTeamHistoryAsync(int teamId);
        Task<List<ScoreHistoryDto>> GetParticipantHistoryAsync(int participantId);
        Task DeleteTeamAsync(int teamId);
        Task DeleteParticipantAsync(int participantId);
        Task<TimeSpan> GetRemainingTimeAsync(int? teamId, int? participantId);
        Task<List<object>> GetParticipantsTimerInfoAsync();
        Task<Participant?> GetParticipantByIdAsync(int participantId);
        Task<List<object>> GetTeamsListAsync();
        Task<List<object>> GetParticipantsListAsync();
    }

    public class ClassificationService : IClassificationService
    {
        private readonly ApplicationDbContext _context;

        public ClassificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClassificationDto> GetClassificationAsync()
        {
            var teams = await _context.Teams.ToListAsync();
            var participants = await _context.Participants
                .Include(p => p.Team)
                .ToListAsync();

            // Calcular tempo restante para cada equipe e ordenar por tempo restante (maior primeiro)
            var teamData = new List<(Team team, TimeSpan remainingTime)>();
            foreach (var team in teams)
            {
                var remainingTime = await GetRemainingTimeAsync(team.Id, null);
                teamData.Add((team, remainingTime));
            }

            // Ordenar apenas por tempo restante - quem tem mais tempo fica na frente
            var sortedTeams = teamData
                .OrderByDescending(t => t.remainingTime)
                .ToList();

            var teamClassification = new List<TeamClassificationDto>();
            for (int i = 0; i < sortedTeams.Count; i++)
            {
                var (team, remainingTime) = sortedTeams[i];
                teamClassification.Add(new TeamClassificationDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    TotalScore = "00:00:00", // Não usar mais TotalScore
                    RemainingTime = FormatTimeSpan(remainingTime),
                    Position = i + 1,
                    IsActive = true // Sempre ativo
                });
            }

            // Calcular tempo restante para cada participante e ordenar por tempo restante (maior primeiro)
            var participantData = new List<(Participant participant, TimeSpan remainingTime)>();
            foreach (var participant in participants)
            {
                var remainingTime = await GetRemainingTimeAsync(null, participant.Id);
                participantData.Add((participant, remainingTime));
            }

            // Ordenar apenas por tempo restante - quem tem mais tempo fica na frente
            var sortedParticipants = participantData
                .OrderByDescending(p => p.remainingTime)
                .ToList();

            var participantClassification = new List<ParticipantClassificationDto>();
            for (int i = 0; i < sortedParticipants.Count; i++)
            {
                var (participant, remainingTime) = sortedParticipants[i];
                participantClassification.Add(new ParticipantClassificationDto
                {
                    Id = participant.Id,
                    Name = participant.Name,
                    TotalScore = "00:00:00", // Não usar mais TotalScore
                    RemainingTime = FormatTimeSpan(remainingTime),
                    TeamName = participant.Team.Name,
                    TeamId = participant.TeamId,
                    Position = i + 1,
                    IsActive = true // Sempre ativo
                });
            }

            return new ClassificationDto
            {
                Teams = teamClassification,
                Participants = participantClassification,
                LastUpdated = DateTime.Now
            };
        }

        public async Task<Team> CreateTeamAsync(CreateTeamDto dto)
        {
            var team = new Team
            {
                Name = dto.Name,
                TotalScore = TimeSpan.Zero,
                CreatedAt = DateTime.Now
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return team;
        }

        public async Task<Participant> CreateParticipantAsync(CreateParticipantDto dto)
        {
            if (!TryParseCustomTimeSpan(dto.InitialTime, out TimeSpan initialTime))
                throw new ArgumentException("Invalid time format. Use HH:MM:SS format.");

            var participant = new Participant
            {
                Name = dto.Name,
                TeamId = dto.TeamId,
                TotalScore = TimeSpan.Zero,
                InitialTime = initialTime,
                StartTime = DateTime.Now, // Sempre inicia automaticamente
                CreatedAt = DateTime.Now
            };

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            return participant;
        }

        public async Task UpdateTeamScoreAsync(int teamId, UpdateScoreDto dto)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null) throw new ArgumentException("Team not found");

            if (!TryParseCustomTimeSpan(dto.ScoreChange, out TimeSpan scoreChangeTime))
                throw new ArgumentException("Invalid time format. Use HH:MM:SS format.");

            if (!Enum.TryParse<ScoreActionType>(dto.ActionType, out ScoreActionType actionType))
                throw new ArgumentException("Invalid action type. Use: AddScore, RemoveScore, Penalty, or Bonus");

            var scoreChange = actionType == ScoreActionType.RemoveScore || actionType == ScoreActionType.Penalty
                ? -scoreChangeTime
                : scoreChangeTime;

            team.TotalScore += scoreChange;
            if (team.TotalScore < TimeSpan.Zero) team.TotalScore = TimeSpan.Zero;

            var history = new ScoreHistory
            {
                TeamId = teamId,
                ScoreChange = scoreChange,
                Description = dto.Description,
                ActionType = actionType,
                Timestamp = DateTime.Now
            };

            _context.ScoreHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateParticipantScoreAsync(int participantId, UpdateScoreDto dto)
        {
            var participant = await _context.Participants
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == participantId);
            
            if (participant == null) throw new ArgumentException("Participant not found");

            if (!TryParseCustomTimeSpan(dto.ScoreChange, out TimeSpan scoreChangeTime))
                throw new ArgumentException("Invalid time format. Use HH:MM:SS format.");

            if (!Enum.TryParse<ScoreActionType>(dto.ActionType, out ScoreActionType actionType))
                throw new ArgumentException("Invalid action type. Use: AddScore, RemoveScore, Penalty, or Bonus");

            var timeChange = actionType == ScoreActionType.RemoveScore || actionType == ScoreActionType.Penalty
                ? -scoreChangeTime
                : scoreChangeTime;

            // Modificar o InitialTime diretamente - isso afeta o RemainingTime
            participant.InitialTime += timeChange;
            if (participant.InitialTime < TimeSpan.Zero) participant.InitialTime = TimeSpan.Zero;

            var history = new ScoreHistory
            {
                TeamId = participant.TeamId,
                ParticipantId = participantId,
                ScoreChange = timeChange,
                Description = dto.Description,
                ActionType = actionType,
                Timestamp = DateTime.Now
            };

            _context.ScoreHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ScoreHistoryDto>> GetTeamHistoryAsync(int teamId)
        {
            var histories = await _context.ScoreHistories
                .Include(sh => sh.Team)
                .Include(sh => sh.Participant)
                .Where(sh => sh.TeamId == teamId)
                .OrderByDescending(sh => sh.Timestamp)
                .ToListAsync();

            return histories.Select(h => new ScoreHistoryDto
            {
                Id = h.Id,
                ScoreChange = FormatTimeSpan(h.ScoreChange),
                Description = h.Description,
                ActionType = h.ActionType.ToString(),
                Timestamp = h.Timestamp,
                TeamName = h.Team?.Name,
                ParticipantName = h.Participant?.Name
            }).ToList();
        }

        public async Task<List<ScoreHistoryDto>> GetParticipantHistoryAsync(int participantId)
        {
            var histories = await _context.ScoreHistories
                .Include(sh => sh.Team)
                .Include(sh => sh.Participant)
                .Where(sh => sh.ParticipantId == participantId)
                .OrderByDescending(sh => sh.Timestamp)
                .ToListAsync();

            return histories.Select(h => new ScoreHistoryDto
            {
                Id = h.Id,
                ScoreChange = FormatTimeSpan(h.ScoreChange),
                Description = h.Description,
                ActionType = h.ActionType.ToString(),
                Timestamp = h.Timestamp,
                TeamName = h.Team?.Name,
                ParticipantName = h.Participant?.Name
            }).ToList();
        }

        public async Task DeleteTeamAsync(int teamId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null) throw new ArgumentException("Team not found");

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteParticipantAsync(int participantId)
        {
            var participant = await _context.Participants
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == participantId);
            
            if (participant == null) throw new ArgumentException("Participant not found");

            _context.Participants.Remove(participant);

            // Recalculate team total score
            var teamParticipants = await _context.Participants
                .Where(p => p.TeamId == participant.TeamId)
                .ToListAsync();
            
            participant.Team.TotalScore = new TimeSpan(teamParticipants.Sum(p => p.TotalScore.Ticks));

            await _context.SaveChangesAsync();
        }


        public async Task<TimeSpan> GetRemainingTimeAsync(int? teamId, int? participantId)
        {
            if (participantId.HasValue)
            {
                var participant = await _context.Participants.FindAsync(participantId.Value);
                if (participant == null) return TimeSpan.Zero;

                if (!participant.StartTime.HasValue)
                    return participant.InitialTime;

                var elapsed = DateTime.Now - participant.StartTime.Value;
                var remaining = participant.InitialTime - elapsed;
                return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
            }

            if (teamId.HasValue)
            {
                // Para equipes, somar o tempo restante de todos os participantes
                var participants = await _context.Participants
                    .Where(p => p.TeamId == teamId.Value && p.StartTime.HasValue)
                    .ToListAsync();

                var totalRemainingTime = TimeSpan.Zero;

                foreach (var participant in participants)
                {
                    var elapsed = DateTime.Now - participant.StartTime.Value;
                    var remaining = participant.InitialTime - elapsed;
                    if (remaining > TimeSpan.Zero)
                    {
                        totalRemainingTime += remaining;
                    }
                }

                return totalRemainingTime;
            }

            return TimeSpan.Zero;
        }

        public async Task<List<object>> GetParticipantsTimerInfoAsync()
        {
            var participants = await _context.Participants
                .Include(p => p.Team)
                .Where(p => p.StartTime.HasValue)
                .ToListAsync();

            var timerInfo = new List<object>();

            foreach (var participant in participants)
            {
                var elapsed = DateTime.Now - participant.StartTime.Value;
                var remaining = participant.InitialTime - elapsed;
                var isExpired = remaining <= TimeSpan.Zero;

                timerInfo.Add(new
                {
                    participantId = participant.Id,
                    participantName = participant.Name,
                    teamName = participant.Team.Name,
                    initialTime = FormatTimeSpan(participant.InitialTime),
                    startTime = participant.StartTime.Value,
                    remainingTime = FormatTimeSpan(remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero),
                    isExpired = isExpired,
                    elapsedTime = FormatTimeSpan(elapsed)
                });
            }

            return timerInfo;
        }

        public async Task<Participant?> GetParticipantByIdAsync(int participantId)
        {
            return await _context.Participants.FindAsync(participantId);
        }

        public async Task<List<object>> GetTeamsListAsync()
        {
            var teams = await _context.Teams.ToListAsync();
            return teams.Select(team => new
            {
                id = team.Id,
                name = team.Name
            }).Cast<object>().ToList();
        }

        public async Task<List<object>> GetParticipantsListAsync()
        {
            var participants = await _context.Participants
                .Include(p => p.Team)
                .ToListAsync();
            
            return participants.Select(participant => new
            {
                id = participant.Id,
                name = participant.Name,
                teamName = participant.Team.Name,
                teamId = participant.TeamId
            }).Cast<object>().ToList();
        }

        private static bool TryParseCustomTimeSpan(string timeString, out TimeSpan result)
        {
            result = TimeSpan.Zero;
            
            if (string.IsNullOrEmpty(timeString))
                return false;
                
            var parts = timeString.Split(':');
            if (parts.Length != 3)
                return false;
                
            if (!int.TryParse(parts[0], out int hours) ||
                !int.TryParse(parts[1], out int minutes) ||
                !int.TryParse(parts[2], out int seconds))
                return false;
                
            // Validar limites
            if (hours < 0 || hours > 99 || minutes < 0 || minutes > 59 || seconds < 0 || seconds > 59)
                return false;
                
            result = new TimeSpan(hours, minutes, seconds);
            return true;
        }

        private static string FormatTimeSpan(TimeSpan timeSpan)
        {
            // Permitir até 99 horas para evitar valores muito grandes na exibição
            var hours = Math.Min(Math.Abs((int)timeSpan.TotalHours), 99);
            var minutes = Math.Abs(timeSpan.Minutes);
            var seconds = Math.Abs(timeSpan.Seconds);
            
            var sign = timeSpan < TimeSpan.Zero ? "-" : "";
            return $"{sign}{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
    }
}
