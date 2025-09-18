using System.ComponentModel.DataAnnotations;

namespace TeamClassification.Models
{
    public class Participant
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public TimeSpan TotalScore { get; set; } = TimeSpan.Zero;
        
        public TimeSpan InitialTime { get; set; } = TimeSpan.Zero;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? StartTime { get; set; }
        
        // Foreign key
        public int TeamId { get; set; }
        
        // Navigation properties
        public virtual Team Team { get; set; } = null!;
        public virtual ICollection<ScoreHistory> ScoreHistories { get; set; } = new List<ScoreHistory>();
    }
}
