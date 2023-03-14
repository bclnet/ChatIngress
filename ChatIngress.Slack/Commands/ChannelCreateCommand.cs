using MediatR;
using System;

namespace ChatIngress.Slack.Commands
{
    public class ChannelCreateCommand : IRequest<string>
    {
        public ChannelCreateCommand NextCommand { get; set; }
        public Nameable<(string id, string email)> Requester { get; set; }
        public bool Private { get; set; }
        public string ChannelType { get; set; }
        public Nameable<object> AccountId { get; set; }
        public object AccountId2 { get; set; }
        public string ChannelName { get; set; }
        public string Purpose { get; set; }
    }
}
