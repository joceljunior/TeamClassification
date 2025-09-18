using System.ComponentModel.DataAnnotations;

namespace TeamClassification.Models
{
    public class Team
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public TimeSpan TotalScore { get; set; } = TimeSpan.Zero;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
        public virtual ICollection<ScoreHistory> ScoreHistories { get; set; } = new List<ScoreHistory>();
    }
}
