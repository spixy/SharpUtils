using System;
using System.Linq.Expressions;
using System.Security.Claims;

namespace SharpUtils
{
    public static class Extensions
    {
        /// <summary>
        /// Determines whether an element is in the array
        /// </summary>
        public static bool Contains<T>(this T[] array, T item) => Array.IndexOf(array, item) != -1;

        /// <summary>
        /// Returns current user ID
        /// </summary>
        public static Guid GetId(this ClaimsPrincipal user) => Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid guid) ? guid : Guid.Empty;

        /// <summary>
        /// Creates Undo action for this object
        /// </summary>
        public static ScopedAction CreateUndo<T, TResult>(this T obj, Expression<Func<T, TResult>> propertyFunc, TResult newValue)
        {
            return ScopedAction.Create(obj, propertyFunc, newValue);
        }

        /// <summary>
        /// Returns true if running on .NET Core, false otherwise
        /// </summary>
        public static bool IsNetCore() => System.Type.GetType(typeof(System.Console).FullName) == null;
    }
}
