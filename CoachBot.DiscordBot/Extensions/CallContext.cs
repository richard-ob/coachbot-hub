using System.Collections.Concurrent;
using System.Threading;

namespace CoachBot.Extensions
{
    /// <summary>
    /// Provides a way to set contextual data that flows with the call and
    /// async context of a test or invocation.
    /// </summary>
    public static class CallContext
    {
        private static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="callContextDataType">The data type with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData(CallContextDataType callContextDataType, object data) =>
            state.GetOrAdd(callContextDataType.ToString(), _ => new AsyncLocal<object>()).Value = data;

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="callContextDataType">The data type of the item in the call context.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static object GetData(CallContextDataType callContextDataType) =>
            state.TryGetValue(callContextDataType.ToString(), out AsyncLocal<object> data) ? data.Value : null;
    }

    public enum CallContextDataType
    {
        DiscordUser
    }
}