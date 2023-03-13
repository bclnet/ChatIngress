using MediatR;
using System;

namespace ChatIngress.Slack.Commands
{
    public class ChannelStatusCommand : IRequest<string>
    {
        public Nameable<(string id, string email)> Requester { get; set; }
        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
        public bool Archive { get; set; }
    }
}
