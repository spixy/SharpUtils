using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
            return Type.GetType(typeof(System.Console).FullName) == null;
        }

        /// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task{TResult}"></see> completes.</summary>
        /// <param name="task">A task.</param>
        /// <param name="continuationFunction">A function to run when the <see cref="Task{TResult}"></see> completes. When run, the delegate will be passed the completed task as an argument.</param>
        /// <typeparam name="TResult">The type of the result produced by this <see cref="Task{TResult}"></see>.</typeparam>
        /// <typeparam name="TNewResult">The type of the result produced by the continuation.</typeparam>
        /// <returns>A new continuation <see cref="Task{TResult}"></see>.</returns>
        /// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}"></see> has been disposed.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="continuationFunction">continuationFunction</paramref> argument is null.</exception>
        public static Task<TNewResult> ContinueWith<TResult, TNewResult>(this Task<TResult> task, Func<TResult, TNewResult> continuationFunction)
        {
            return task.ContinueWith(t => continuationFunction(t.Result));
        }

        /// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task{TResult}"></see> completes.</summary>
        /// <param name="task">A task.</param>
        /// <param name="continuationFunction">A function to run when the <see cref="Task{TResult}"></see> completes. When run, the delegate will be passed the completed task as an argument.</param>
        /// <typeparam name="TResult">The type of the result produced by this <see cref="Task{TResult}"></see>.</typeparam>
        /// <returns>A new continuation <see cref="Task{TResult}"></see>.</returns>
        /// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}"></see> has been disposed.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="continuationFunction">continuationFunction</paramref> argument is null.</exception>
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<TResult> continuationFunction)
        {
            return task.ContinueWith(t => continuationFunction(t.Result));
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
                await Task.Delay(-1).ConfigureAwait(false);
                return obj;
            }
            Task.Factory.StartNew(RunForever, TaskCreationOptions.LongRunning);
        }
    }
}
