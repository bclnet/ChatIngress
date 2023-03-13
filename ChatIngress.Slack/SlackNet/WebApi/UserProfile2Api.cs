//using System.Threading;
//using System.Threading.Tasks;
//using Args = System.Collections.Generic.Dictionary<string, object>;

//namespace SlackNet.WebApi
//{
//    /// <summary>
//    /// IUserProfile2Api
//    /// </summary>
//    public interface IUserProfile2Api
//    {
//        Task SetUserProfileEmail(string userId, string email, CancellationToken? cancellationToken = null);
//        Task SetUserProfileFullName(string userId, string firstName, string lastName, CancellationToken? cancellationToken = null);
//        Task SetUserProfileUserName(string userId, string userName, CancellationToken? cancellationToken = null);
//    }

//    /// <summary>
//    /// UserProfile2Api
//    /// </summary>
//    public class UserProfile2Api : IUserProfile2Api
//    {
//        readonly ISlackApiClient _client;
//        public UserProfile2Api(ISlackApiClient client) => _client = client;

//        public Task SetUserProfileEmail(string userId, string email, CancellationToken? cancellationToken = null) =>
//            _client.Post("users.profile.set", new Args
//            {
//                { "user", userId },
//                { "profile", $"{{\"email\":\"{email}\"}}" },
//                { "set_active", "true" }
//            }, cancellationToken);

//        public Task SetUserProfileFullName(string userId, string firstName, string lastName, CancellationToken? cancellationToken = null) =>
//            _client.Post("users.profile.set", new Args
//            {
//                { "user", userId },
//                { "profile", "{\"first_name\":\"" + firstName + "\",\"last_name\": \"" + lastName +"\"}" },
//                { "set_active", "true" }
//            }, cancellationToken);

//        public Task SetUserProfileUserName(string userId, string userName, CancellationToken? cancellationToken = null) =>
//             _client.Post("users.profile.set", new Args
//             {
//                { "user", userId },
//                { "profile", "{\"username\":\"" + userName + "\"}" },
//                { "set_active", "true" }
//             }, cancellationToken);
//    }
//}
