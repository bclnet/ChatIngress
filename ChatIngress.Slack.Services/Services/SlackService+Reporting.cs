using Dapper;
using ExcelTrans.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChatIngress.Slack.Services
{
    public partial class SlackService
    {
        enum LogType
        {
            // users
            [Display(Name = "Invited")] InsertUser,
            [Display(Name = "Activated")] BindUser,
            [Display(Name = "Inactivated")] Inactivate,
            [Display(Name = "Disabled")] DeleteUser,
            [Display(Name = "Updated: Full Name")] UpdateUserFullName,
            [Display(Name = "Updated: Email")] UpdateUserEmail,
            // channels
            [Display(Name = "Updated: Type")] TypeChannel,
            [Display(Name = "Updated: Rename")] RenameChannel,
            [Display(Name = "Added")] InsertChannel,
            [Display(Name = "Updated: Account")] ShiftChannel,
            [Display(Name = "Archived")] ArchiveChannel,
            [Display(Name = "Un-archived")] UnArchiveChannel,
        }

        class PersonResult_
        {
            public string PackId { get; set; }
            public string FullName { get; set; }
        }

        class ReportResult_
        {
            public DateTime CreatedOn { get; set; }
            public string PackId { get; set; }
            public string Type { get; set; }
            public XElement Blob { get; set; }
        }

        public async Task<Stream> ReportAsync(IDbConnection conn, DateTime from, DateTime till, CancellationToken? cancellationToken = null)
        {
            var peopleById = (await conn.QueryAsync<PersonResult_>("Select PackId, FullName From [crm].[Person]", null, commandType: CommandType.Text)).ToDictionary(x => x.PackId);
            var set = (await conn.QueryAsync<ReportResult_>("Select CreatedOn, PackId, Type, Blob From [ops].[SlackLog] Where CreatedOn >= @from And CreatedOn < @till", new { from, till }, commandType: CommandType.Text))
                .Select(x => new
                {
                    x.CreatedOn,
                    Type = (LogType)Enum.Parse(typeof(LogType), x.Type),
                    x.PackId,
                    x.Blob,
                }).OrderBy(x => x.Type).ThenBy(x => x.CreatedOn).ToList();
            var people = set.Where(x => x.Type >= LogType.InsertUser && x.Type <= LogType.UpdateUserEmail)
                .Select(x => new
                {
                    x.CreatedOn,
                    x.Type,
                    x.PackId,
                    Person = x.PackId != null && peopleById.TryGetValue(x.PackId, out var person) ? person : null,
                    x.Blob,
                }).OrderBy(x => x.Type).ThenBy(x => x.CreatedOn).ToList();
            foreach (var group in people.Where(x => x.Type == LogType.InsertUser || x.Type == LogType.BindUser || x.Type == LogType.DeleteUser).GroupBy(x => x.PackId))
                foreach (var i in group.OrderByDescending(x => x.CreatedOn).Skip(1))
                    people.Remove(i);
            var channels = set.Where(x => x.Type >= LogType.TypeChannel && x.Type <= LogType.UnArchiveChannel)
                .Select(x => new
                {
                    x.CreatedOn,
                    x.Type,
                    x.Blob,
                })
                .OrderBy(x => x.Type).ThenBy(x => x.CreatedOn).ToList();
            
            // WRITE
            var s = new MemoryStream();
            var w = new StreamWriter(s);
            var ctx = new CsvWriterOptions();
            ctx.Fields.Add(new CsvWriterField("FromName") { DisplayName = "From Name" });

            // PEOPLE
            w.WriteLine("People invited/activated/inactivated/disabled/updated");
            CsvWriter.Write(w, people.Select(x => new
            {
                Method = EnumHelper<LogType>.GetDisplayValue(x.Type),
                Person = x.Type == LogType.InsertUser ? $"{x.Blob.Attribute("firstName").Value} - {x.Blob.Attribute("email").Value}"
                   : x.Type == LogType.DeleteUser ? x.Blob.Attribute("email").Value
                   : x.Person != null ? x.Person.FullName : $"Unknown: {x.PackId}",
                Date = x.CreatedOn,
            }), ctx);
            w.WriteLine();

            // CHANNELS
            w.WriteLine("Channels added/updated/removed/archived/requested");
            CsvWriter.Write(w, channels.Select(x => new
            {
                Method = EnumHelper<LogType>.GetDisplayValue(x.Type),
                Name = x.Blob.Attribute("name").Value,
                FromName = x.Blob.Attribute("lastName") != null ? x.Blob.Attribute("lastName").Value : string.Empty,
                Channel = x.Type == LogType.InsertChannel ? x.Blob.Attribute("type").Value : string.Empty,
                Date = x.CreatedOn,
            }));
            w.WriteLine();

            s.Position = 0;
            return s;
        }
    }
}