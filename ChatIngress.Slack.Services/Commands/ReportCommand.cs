using ChatIngress.Slack.Services;
using MediatR;
using System;
using System.Data;
using System.IO;

namespace ChatIngress.Slack.Commands
{
    public class ReportCommand : IRequest<Stream>
    {
        public ISlackService Service { get; set; }
        public IDbConnection Db { get; set; }
        public DateTime? From { get; set; }
        public DateTime? Till { get; set; }
    }
}
