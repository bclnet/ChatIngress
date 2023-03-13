using ChatIngress.Slack.SlackNet.WebApi;
using Microsoft.Extensions.Caching.Memory;
using SlackNet.Blocks;
using SlackNet.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackNet
{
    /// <summary>
    /// SlackNetExtensions
    /// </summary>
    public static class SlackNetExtensions
    {
        static MemoryCache _cache = new(new MemoryCacheOptions());

        public static IAdminUsersApi AdminUsers(this ISlackApiClient source) => new AdminUsersApi(source);
        public static IUndocumentedApi Undocumented(this ISlackApiClient source) => new UndocumentedApi(source);
        public static IUndocumentedUsersAdminApi UndocumentedUsersAdmin(this ISlackApiClient source) => new UndocumentedUsersAdminApi(source);

        /// <summary>
        /// Users the profile2.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        //public static IUserProfile2Api UserProfile2(this SlackApiClient source) => new UserProfile2Api(source);

        /// <summary>
        /// Determines whether this instance is private.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   <c>true</c> if the specified source is private; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrivate(this Conversation source) => source.Id?.StartsWith("G") ?? false;

        /// <summary>
        /// Converts to sectionblock.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static SectionBlock ToSectionBlock(this Markdown source) => new SectionBlock { Text = source };

        /// <summary>
        /// Cacheds the users.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <returns></returns>
        public static (IDictionary<string, User> byId, IDictionary<string, User> byName) CachedUsers(this IUsersApi source, TimeSpan? timeSpan = null)
        {
            if (!_cache.TryGetValue("users", out (IDictionary<string, User> byId, IDictionary<string, User> byName) users))
            {
                var list = source.List().PagedMetadata(x => x.Members, x => x.ResponseMetadata, cursor => source.List(cursor: cursor))
                    .GetAwaiter().GetResult().ToList();
                users = (list.ToDictionary(x => x.Id), list.ToDictionary(x => x.Name));
                _cache.Set("users", users, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(timeSpan ?? TimeSpan.FromSeconds(60 * 5)));
            }
            return users;
        }

        /// <summary>
        /// Tries the name of the get user by.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static bool TryGetUserByName(this IUsersApi source, string name, out User user)
        {
            var (_, byName) = source.CachedUsers();
            return byName.TryGetValue(name.Substring(1), out user);
        }

        /// <summary>
        /// Tries the name of the get user by.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="id">The id.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static bool TryGetUserById(this IUsersApi source, string id, out User user)
        {
            var (byId, _) = source.CachedUsers();
            return byId.TryGetValue(id, out user);
        }

        public static bool TryGetRequester(this IUsersApi source, User user, out Nameable<(string id, string email)> requester, bool alwaysTrue = false)
        {
            var (byId, _) = source.CachedUsers();
            if (byId.TryGetValue(user.Id, out var foundUser))
            {
                requester = new Nameable<(string id, string email)>((foundUser.Id, foundUser.Profile.Email), foundUser.Profile.RealName);
                return true;
            }
            if (!alwaysTrue)
            {
                requester = default;
                return false;
            }
            requester = new Nameable<(string id, string email)>((user.Id, null), $"@{user.Name}");
            return true;
        }

        /// <summary>
        /// Pageds the metadata asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The task.</param>
        /// <param name="element">The element.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="next">The next.</param>
        /// <param name="predicate">Optional match predicate. false continues.</param>
        /// <returns></returns>
        public static async Task<IList<TElement>> PagedMetadata<T, TElement>(this Task<T> source, Func<T, IList<TElement>> element, Func<T, ResponseMetadata> metadata, Func<string, Task<T>> next, Predicate<List<TElement>> predicate = null)
        {
            var list = new List<TElement>();
            var set = await source.ConfigureAwait(false);
            var nextCursor = metadata(set).NextCursor;
            while (!string.IsNullOrEmpty(nextCursor) && (predicate == null || !predicate(list)))
            {
                list.AddRange(element(set));
                set = await next(nextCursor).ConfigureAwait(false);
                nextCursor = metadata(set).NextCursor;
            }
            list.AddRange(element(set));
            return list;
        }
    }
}
