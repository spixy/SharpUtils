using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SharpUtils
{
    public static class Extensions
    {
        /// <summary>
        /// Determines whether an element is in the array
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        public static bool IsNetCore()
        {
            return System.Type.GetType(typeof(System.Console).FullName) == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this Task task)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget<T>(this Task<T> task)
        {
        }

        /// <summary>
        /// Make sure object is never garbage collected
        /// </summary>
        public static void LiveForever<T>(this T obj)
        {
            async Task<T> RunForever()
            {
                await Task.Delay(-1);
                return obj;
            }
            Task.Factory.StartNew(RunForever, TaskCreationOptions.LongRunning);
        }
    }
}
