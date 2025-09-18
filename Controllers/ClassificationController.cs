using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TeamClassification.DTOs;
using TeamClassification.Hubs;
using TeamClassification.Models;
using TeamClassification.Services;

namespace TeamClassification.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassificationController : ControllerBase
    {
        private readonly IClassificationService _classificationService;
        private readonly IHubContext<ClassificationHub> _hubContext;

        public ClassificationController(IClassificationService classificationService, IHubContext<ClassificationHub> hubContext)
        {
            _classificationService = classificationService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<ClassificationDto>> GetClassification()
        {
            var classification = await _classificationService.GetClassificationAsync();
            return Ok(classification);
        }

        [HttpPost("teams")]
        public async Task<ActionResult<Team>> CreateTeam([FromBody] CreateTeamDto dto)
        {
            try
            {
                var team = await _classificationService.CreateTeamAsync(dto);
                
                // Notify all clients about the update
                var classification = await _classificationService.GetClassificationAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveClassification", classification);
                
                return CreatedAtAction(nameof(GetClassification), new { id = team.Id }, team);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("participants")]
        public async Task<ActionResult<Participant>> CreateParticipant([FromBody] CreateParticipantDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest($"Model validation errors: {string.Join(", ", errors)}");
                }

                var participant = await _classificationService.CreateParticipantAsync(dto);
                
                // Notify all clients about the update
                var classification = await _classificationService.GetClassificationAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveClassification", classification);
                
                return CreatedAtAction(nameof(GetClassification), new { id = participant.Id }, participant);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("teams/{teamId}/score")]
        public async Task<IActionResult> UpdateTeamScore(int teamId, [FromBody] UpdateScoreDto dto)
        {
            try
            {
                await _classificationService.UpdateTeamScoreAsync(teamId, dto);
                
                // Notify all clients about the update
                var classification = await _classificationService.GetClassificationAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveClassification", classification);
                
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("participants/{participantId}/score")]
        public async Task<IActionResult> UpdateParticipantScore(int participantId, [FromBody] UpdateScoreDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest($"Model validation errors: {string.Join(", ", errors)}");
                }

                await _classificationService.UpdateParticipantScoreAsync(participantId, dto);
                
                // Notify all clients about the update
                var classification = await _classificationService.GetClassificationAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveClassification", classification);
                
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("teams/{teamId}/history")]
        public async Task<ActionResult<List<ScoreHistoryDto>>> GetTeamHistory(int teamId)
        {
            try
            {
                var history = await _classificationService.GetTeamHistoryAsync(teamId);
                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("participants/{participantId}/history")]
        public async Task<ActionResult<List<ScoreHistoryDto>>> GetParticipantHistory(int participantId)
        {
            try
            {
                var history = await _classificationService.GetParticipantHistoryAsync(participantId);
                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("teams/{teamId}")]
        public async Task<IActionResult> DeleteTeam(int teamId)
        {
            try
            {
                await _classificationService.DeleteTeamAsync(teamId);
                
                // Notify all clients about the update
                var classification = await _classificationService.GetClassificationAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveClassification", classification);
                
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("participants/{participantId}")]
        public async Task<IActionResult> DeleteParticipant(int participantId)
        {
            try
            {
                await _classificationService.DeleteParticipantAsync(participantId);
                
                // Notify all clients about the update
                var classification = await _classificationService.GetClassificationAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveClassification", classification);
                
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("simple-test")]
        public IActionResult SimpleTest([FromBody] object data)
        {
            return Ok(new { 
                received = data,
                type = data?.GetType().Name
            });
        }

        [HttpGet("timer/remaining")]
        public async Task<ActionResult<object>> GetRemainingTime([FromQuery] int? teamId, [FromQuery] int? participantId)
        {
            try
            {
                var remainingTime = await _classificationService.GetRemainingTimeAsync(teamId, participantId);
                return Ok(new { 
                    remainingTime = $"{(int)remainingTime.TotalHours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}",
                    teamId = teamId,
                    participantId = participantId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("participants/timer-info")]
        public async Task<ActionResult<List<object>>> GetParticipantsTimerInfo()
        {
            try
            {
                var timerInfo = await _classificationService.GetParticipantsTimerInfoAsync();
                return Ok(timerInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("debug/timer-info")]
        public async Task<ActionResult<object>> GetDebugTimerInfo([FromQuery] int participantId)
        {
            try
            {
                var participant = await _classificationService.GetParticipantByIdAsync(participantId);
                if (participant == null)
                    return NotFound("Participant not found");

                var remainingTime = await _classificationService.GetRemainingTimeAsync(null, participantId);
                
                return Ok(new
                {
                    participantId = participant.Id,
                    participantName = participant.Name,
                    initialTime = participant.InitialTime.ToString(@"hh\:mm\:ss"),
                    initialTimeTicks = participant.InitialTime.Ticks,
                    startTime = participant.StartTime,
                    remainingTime = remainingTime.ToString(@"hh\:mm\:ss"),
                    remainingTimeTicks = remainingTime.Ticks,
                    elapsed = participant.StartTime.HasValue ? (DateTime.Now - participant.StartTime.Value).ToString(@"hh\:mm\:ss") : "N/A"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("teams/list")]
        public async Task<ActionResult<List<object>>> GetTeamsList()
        {
            try
            {
                var teams = await _classificationService.GetTeamsListAsync();
                return Ok(teams);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("participants/list")]
        public async Task<ActionResult<List<object>>> GetParticipantsList()
        {
            try
            {
                var participants = await _classificationService.GetParticipantsListAsync();
                return Ok(participants);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
