using System.ComponentModel.DataAnnotations;

namespace TeamClassification.Models
{
    public class ScoreHistory
    {
        public int Id { get; set; }
        
        public TimeSpan ScoreChange { get; set; }
        
        public string Description { get; set; } = string.Empty;
        
        public ScoreActionType ActionType { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Foreign keys (nullable for team-only or participant-only actions)
        public int? TeamId { get; set; }
        public int? ParticipantId { get; set; }
        
        // Navigation properties
        public virtual Team? Team { get; set; }
        public virtual Participant? Participant { get; set; }
    }
    
    public enum ScoreActionType
    {
        AddScore,
        RemoveScore,
        Penalty,
        Bonus
    }
}
