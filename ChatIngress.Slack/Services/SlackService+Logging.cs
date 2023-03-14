using Dapper;
using SlackNet;
using System.Data;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChatIngress.Slack.Services
{
    public partial class SlackService
    {
        // USER

        public static Task<int> LogInsertUser(IDbConnection conn, string email, string firstName, bool restricted) =>
            conn.ExecuteAsync("Insert [ops].[SlackLog](Type, Blob) Values('InsertUser', @blob)", new
            {
                blob = new XElement("r", XAttribute("email", email), XAttribute("firstName", firstName), XAttribute("restricted", restricted)).ToString()
            });

        public static Task<int> LogBindUser(IDbConnection conn, string packId, string email) =>
            conn.ExecuteAsync("Insert [ops].[SlackLog](PackId, Type, Blob) Values(@packId, 'BindUser', @blob)", new
            {
                packId,
                blob = new XElement("r", XAttribute("email", email)).ToString()
            });

        public static Task<int> LogDeleteUser(IDbConnection conn, string packId, string email) =>
            conn.ExecuteAsync("Insert [ops].[SlackLog](PackId, Type, Blob) Values(@packId, 'DeleteUser', @blob)", new
            {
                packId = packId,
                blob = new XElement("r", XAttribute("email", email)).ToString()
            });

        public static Task<int> LogUpdateUserFullName(IDbConnection conn, string packId, string firstName, string lastName) =>
            conn.ExecuteAsync("Insert [ops].[SlackLog](PackId, Type, Blob) Values(@packId, 'UpdateUserFullName', @blob)", new
            {
                packId = packId,
                blob = new XElement("r", XAttribute("firstName", firstName), XAttribute("lastName", lastName)).ToString()
            });

        public static Task<int> LogUpdateUserEmail(IDbConnection conn, string packId, string email) =>
            conn.ExecuteAsync("Insert [ops].[SlackLog](PackId, Type, Blob) Values(@packId, 'UpdateUserEmail', @blob)", new
            {
                packId = packId,
                blob = new XElement("r", XAttribute("email", email)).ToString()
            });


        // CHANNEL

        public static Task<int> LogTypeChannel(IDbConnection conn, string channelId, string name, string type) =>
            conn.ExecuteAsync("Insert [ops].[SlackLog](PackId, Type, Blob) Values(@packId, 'TypeChannel', @blob)", new
            {
                packId = channelId,
                blob = new XElement("r", XAttribute("name", name), XAttribute("type", type)).ToString()
            });

        public static Task<int> LogRenameChannel(IDbConnection conn, string channelId, string lastName, string name) =>
            conn.ExecuteAsync("Insert [ops].[SlackLog](PackId, Type, Blob) Values(@packId, 'RenameChannel', @blob)", new
            {
                packId = channelId,
                blob = new XElement("r", XAttribute("lastName", lastName), XAttribute("name", name)).ToString()
            });

        public static Task<int> LogShiftChannel(IDbConnection conn, string channelId, string lastName, string name) =>
            conn.ExecuteAsync("Insert [ops].[SlackLog](PackId, Type, Blob) Values(@packId, 'ShiftChannel', @blob)", new
            {
                packId = channelId,
                blob = new XElement("r", XAttribute("lastName", lastName), XAttribute("name", name)).ToString()
            });

        public static Task<int> LogInsertChannel(IDbConnection conn, Conversation s) =>
            conn.ExecuteAsync("Insert [ops].[SlackLog](PackId, Type, Blob) Values(@packId, 'InsertChannel', @blob)", new
            {
                packId = s.Id,
                blob = new XElement("r", XAttribute("type", s.Creator), XAttribute("name", s.Name), XAttribute("purpose", s.Purpose?.Value), s.IsPrivate ? XAttribute("private", s.IsPrivate) : null).ToString()
            });

        public static Task<int> LogUpdateChannelStatus(IDbConnection conn, Conversation s) =>
            conn.ExecuteAsync("Insert [ops].[SlackLog](PackId, Type, Blob) Values(@packId, @type, @blob)", new
            {
                packId = s.Id,
                type = s.IsArchived ? "ArchiveChannel" : "UnArchiveChannel",
                blob = new XElement("r", XAttribute("name", s.Name), XAttribute("private", s.IsPrivate())).ToString()
            });
    }
}