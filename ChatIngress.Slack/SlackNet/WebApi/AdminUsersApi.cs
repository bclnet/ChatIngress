using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet;
using SlackNet.WebApi;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace ChatIngress.Slack.SlackNet.WebApi
{
    /// <summary>
    /// IAdminUsersApi
    /// </summary>
    public interface IAdminUsersApi
    {
        /// <summary>
        /// Add an Enterprise user to a workspace.
        /// </summary>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="userId">The ID of the user to add to the workspace.</param>
        /// <param name="channels">Values of channel IDs to add user in the new workspace.</param>
        /// <param name="isRestricted">True if user should be added to the workspace as a guest.</param>
        /// <param name="isUltraRestricted">True if user should be added to the workspace as a single-channel guest.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.assign" />
        Task Assign(string teamId, string userId,
            IEnumerable<string> channels = null, bool isRestricted = false, bool isUltraRestricted = false,
            CancellationToken? cancellationToken = null);

        /// <summary>
        /// Invite a user to a workspace.
        /// </summary>
        /// <param name="channels">A list of channel_ids for this user to join. At least one channel is required.</param>
        /// <param name="email">The email address of the person to invite.</param>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="customMessage">An optional message to send to the user in the invite email.</param>
        /// <param name="guestExpirationTs">Timestamp when guest account should be disabled. Only include this timestamp if you are inviting a guest user and you want their account to expire on a certain date.</param>
        /// <param name="isRestricted">Is this user a multi-channel guest user? (default: false).</param>
        /// <param name="isUltraRestricted">Is this user a single channel guest user? (default: false).</param>
        /// <param name="realName">Full name of the user.</param>
        /// <param name="resend">Allow this invite to be resent in the future if a user has not signed up yet. (default: false).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.invite" />
        Task Invite(IEnumerable<string> channels, string email, string teamId,
            string customMessage = null, string guestExpirationTs = null, bool isRestricted = false, bool isUltraRestricted = false, string realName = null, bool resend = false,
            CancellationToken? cancellationToken = null);

        /// <summary>
        /// List users on a workspace.
        /// </summary>
        /// <param name="cursor">Set cursor to next_cursor returned by the previous call to list items in the next page.</param>
        /// <param name="limit">Limit for how many users to be retrieved per page.</param>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.list" />
        Task<UserListResponse> List(string cursor = null, int limit = 100, string teamId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Remove a user from a workspace.
        /// </summary>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="userId">The ID of the user to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.remove" />
        Task Remove(string teamId, string userId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Set an existing guest, regular user, or owner to be an admin user.
        /// </summary>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="userId">The ID of the user to designate as an admin.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.setAdmin" />
        Task SetAdmin(string teamId, string userId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Set an expiration for a guest user.
        /// </summary>
        /// <param name="expirationTs">Timestamp when guest account should be disabled.</param>
        /// <param name="userId">The ID of the user to set an expiration for.</param>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.setExpiration" />
        Task SetExpiration(string expirationTs, string userId, string teamId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Set an existing guest, regular user, or admin user to be a workspace owner.
        /// </summary>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="userId">Id of the user to promote to owner.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.setOwner" />
        Task SetOwner(string teamId, string userId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Set an existing guest user, admin user, or owner to be a regular user.
        /// </summary>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="userId">The ID of the user to designate as a regular user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.setRegular" />
        Task SetRegular(string teamId, string userId, CancellationToken? cancellationToken = null);
    }

    /// <summary>
    /// AdminUsersApi
    /// </summary>
    /// <seealso cref="IAdminUsersApi" />
    public class AdminUsersApi : IAdminUsersApi
    {
        readonly ISlackApiClient _client;
        public AdminUsersApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Add an Enterprise user to a workspace.
        /// </summary>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="userId">The ID of the user to add to the workspace.</param>
        /// <param name="channels">Values of channel IDs to add user in the new workspace.</param>
        /// <param name="isRestricted">True if user should be added to the workspace as a guest.</param>
        /// <param name="isUltraRestricted">True if user should be added to the workspace as a single-channel guest.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.assign" />
        public Task Assign(string teamId, string userId,
            IEnumerable<string> channels = null, bool isRestricted = false, bool isUltraRestricted = false,
            CancellationToken? cancellationToken = null) =>
            _client.Post("admin.users.assign", new Args
            {
                { "team_id", teamId },
                { "user_id", userId },
                { "channel_ids", string.Join(",", channels) },
                { "is_restricted", isRestricted ? isRestricted : null },
                { "is_ultra_restricted", isUltraRestricted ? isUltraRestricted : null },
            }, cancellationToken);

        /// <summary>
        /// Invite a user to a workspace.
        /// </summary>
        /// <param name="channels">A list of channel_ids for this user to join. At least one channel is required.</param>
        /// <param name="email">The email address of the person to invite.</param>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="customMessage">An optional message to send to the user in the invite email.</param>
        /// <param name="guestExpirationTs">Timestamp when guest account should be disabled. Only include this timestamp if you are inviting a guest user and you want their account to expire on a certain date.</param>
        /// <param name="isRestricted">Is this user a multi-channel guest user? (default: false).</param>
        /// <param name="isUltraRestricted">Is this user a single channel guest user? (default: false).</param>
        /// <param name="realName">Full name of the user.</param>
        /// <param name="resend">Allow this invite to be resent in the future if a user has not signed up yet. (default: false).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.invite" />
        public Task Invite(IEnumerable<string> channels, string email, string teamId,
            string customMessage = null, string guestExpirationTs = null, bool isRestricted = false, bool isUltraRestricted = false, string realName = null, bool resend = false,
            CancellationToken? cancellationToken = null) =>
            _client.Post("admin.users.invite", new Args
            {
                { "channel_ids", string.Join(",", channels) },
                { "email", email },
                { "team_id", teamId },
                { "custom_message", customMessage },
                { "guest_expiration_ts", guestExpirationTs },
                { "is_restricted", isRestricted ? isRestricted : null },
                { "is_ultra_restricted", isUltraRestricted ? isUltraRestricted : null },
                { "real_name", realName },
                { "resend", resend ? resend : null }
            }, cancellationToken);

        /// <summary>
        /// List users on a workspace.
        /// </summary>
        /// <param name="cursor">Set cursor to next_cursor returned by the previous call to list items in the next page.</param>
        /// <param name="limit">Limit for how many users to be retrieved per page.</param>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.list" />
        public Task<UserListResponse> List(string cursor = null, int limit = 100, string teamId = null, CancellationToken? cancellationToken = null) =>
            _client.Post<UserListResponse>("admin.users.list", new Args
            {
                { "cursor", cursor },
                { "limit", limit },
                { "team_id", teamId }
            }, cancellationToken);

        /// <summary>
        /// Remove a user from a workspace.
        /// </summary>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="userId">The ID of the user to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.remove" />
        public Task Remove(string teamId, string userId, CancellationToken? cancellationToken = null) =>
            _client.Post("admin.users.remove", new Args
            {
                { "team_id", teamId },
                { "user_id", userId }
            }, cancellationToken);

        /// <summary>
        /// Set an existing guest, regular user, or owner to be an admin user.
        /// </summary>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="userId">The ID of the user to designate as an admin.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.setAdmin" />
        public Task SetAdmin(string teamId, string userId, CancellationToken? cancellationToken = null) =>
            _client.Get("admin.users.setAdmin", new Args
            {
                { "team_id", teamId },
                { "user_id", userId }
            }, cancellationToken);

        /// <summary>
        /// Set an expiration for a guest user.
        /// </summary>
        /// <param name="expirationTs">Timestamp when guest account should be disabled.</param>
        /// <param name="userId">The ID of the user to set an expiration for.</param>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.setExpiration" />
        public Task SetExpiration(string expirationTs, string userId, string teamId = null, CancellationToken? cancellationToken = null) =>
            _client.Get("admin.users.setExpiration", new Args
            {
                { "expiration_ts", expirationTs },
                { "user_id", userId },
                { "team_id", teamId }
            }, cancellationToken);

        /// <summary>
        /// Set an existing guest, regular user, or admin user to be a workspace owner.
        /// </summary>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="userId">Id of the user to promote to owner.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.setOwner" />
        public Task SetOwner(string teamId, string userId, CancellationToken? cancellationToken = null) =>
            _client.Get("admin.users.setOwner", new Args
            {
                { "team_id", teamId },
                { "user_id", userId }
            }, cancellationToken);

        /// <summary>
        /// Set an existing guest user, admin user, or owner to be a regular user.
        /// </summary>
        /// <param name="teamId">The ID (T1234) of the workspace.</param>
        /// <param name="userId">The ID of the user to designate as a regular user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <seealso cref="https://api.slack.com/methods/admin.users.setRegular" />
        public Task SetRegular(string teamId, string userId, CancellationToken? cancellationToken = null) =>
            _client.Get("admin.users.setRegular", new Args
            {
                { "team_id", teamId },
                { "user_id", userId }
            }, cancellationToken);
    }
}
