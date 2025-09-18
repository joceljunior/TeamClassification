using System.ComponentModel.DataAnnotations;
using TeamClassification.Models;

namespace TeamClassification.DTOs
{
    public class CreateTeamDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    public class CreateParticipantDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public int TeamId { get; set; }
        
        [Required]
        public string InitialTime { get; set; } = "00:00:00";
    }

    public class UpdateScoreDto
    {
        public string ScoreChange { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
    }

    public class TeamClassificationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TotalScore { get; set; } = string.Empty;
        public string RemainingTime { get; set; } = string.Empty;
        public int Position { get; set; }
        public bool IsActive { get; set; }
    }

    public class ParticipantClassificationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TotalScore { get; set; } = string.Empty;
        public string RemainingTime { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public int Position { get; set; }
        public bool IsActive { get; set; }
    }

    public class ClassificationDto
    {
        public List<TeamClassificationDto> Teams { get; set; } = new();
        public List<ParticipantClassificationDto> Participants { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }

    public class ScoreHistoryDto
    {
        public int Id { get; set; }
        public string ScoreChange { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? TeamName { get; set; }
        public string? ParticipantName { get; set; }
    }

    public class StartTimerDto
    {
        public int? TeamId { get; set; }
        public int? ParticipantId { get; set; }
    }

    public class StopTimerDto
    {
        public int? TeamId { get; set; }
        public int? ParticipantId { get; set; }
    }
}
