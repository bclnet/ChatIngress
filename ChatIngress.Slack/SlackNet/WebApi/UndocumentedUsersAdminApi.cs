using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace ChatIngress.Slack.SlackNet.WebApi
{
    /// <summary>
    /// IUndocumentedUsersAdminApi
    /// </summary>
    public interface IUndocumentedUsersAdminApi
    {
        /// <summary>
        /// Invites a user to the workspace.
        /// </summary>
        /// <param name="email">The email address of the person to invite.</param>
        /// <param name="firstName">First name of the user.</param>
        /// <param name="restricted">Is this user a multi-channel guest user? (default: false).</param>
        /// <param name="channels">A list of channel_ids for this user to join. At least one channel is required.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task Invite(string email, string firstName, bool restricted, IEnumerable<string> channels = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Set an existing guest, regular user, or admin user to inactive.
        /// </summary>
        /// <param name="userId">The ID of the user to make inactive.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task SetInactive(string userId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Remove a user from the workspace.
        /// </summary>
        /// <param name="userId">The ID of the user to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task Remove(string userId, CancellationToken? cancellationToken = null);
    }

    /// <summary>
    /// UndocumentedUsersAdminApi
    /// </summary>
    public class UndocumentedUsersAdminApi : IUndocumentedUsersAdminApi
    {
        readonly ISlackApiClient _client;
        public UndocumentedUsersAdminApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Invites a user to the workspace.
        /// </summary>
        /// <param name="email">The email address of the person to invite.</param>
        /// <param name="firstName">First name of the user.</param>
        /// <param name="restricted">Is this user a multi-channel guest user? (default: false).</param>
        /// <param name="channels">A list of channel_ids for this user to join. At least one channel is required.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task Invite(string email, string firstName, bool restricted, IEnumerable<string> channels = null, CancellationToken? cancellationToken = null) =>
            _client.Get("users.admin.invite", new Args
            {
                { "email", email },
                { "first_name", firstName },
                { "channels", channels != null ? string.Join(",", channels) : null },
                { "restricted", restricted ? "1" : null }
            }, cancellationToken);

        /// <summary>
        /// Set an existing guest, regular user, or admin user to inactive.
        /// </summary>
        /// <param name="userId">The ID of the user to make inactive.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task SetInactive(string userId, CancellationToken? cancellationToken = null) =>
            _client.Get("users.admin.setInactive", new Args
            {
                { "user", userId },
                { "set_active", "true" }
            }, cancellationToken);

        /// <summary>
        /// Remove a user from the workspace.
        /// </summary>
        /// <param name="userId">The ID of the user to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task Remove(string userId, CancellationToken? cancellationToken = null) =>
            _client.Get("users.admin.delete", new Args
            {
                { "userId", userId }
            }, cancellationToken);
    }
}
