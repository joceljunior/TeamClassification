using Microsoft.AspNetCore.SignalR;
using TeamClassification.DTOs;
using TeamClassification.Services;

namespace TeamClassification.Hubs
{
    public class ClassificationHub : Hub
    {
        private readonly IClassificationService _classificationService;

        public ClassificationHub(IClassificationService classificationService)
        {
            _classificationService = classificationService;
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task GetCurrentClassification()
        {
            var classification = await _classificationService.GetClassificationAsync();
            await Clients.Caller.SendAsync("ReceiveClassification", classification);
        }
    }
}
