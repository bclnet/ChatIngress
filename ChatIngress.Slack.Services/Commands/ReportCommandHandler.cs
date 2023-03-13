using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ChatIngress.Slack.Commands
{
    public class ReportCommandHandler : IRequestHandler<ReportCommand, Stream>
    {
        public async Task<Stream> Handle(ReportCommand s, CancellationToken cancellationToken) =>
             await s.Service.ReportAsync(s.Db, s.From ?? DateTime.Today.AddDays(-DateTime.Today.DayOfYear), s.Till ?? DateTime.Today.AddDays(1), cancellationToken);
    }
}
