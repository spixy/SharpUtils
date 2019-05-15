using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SharpUtils
{
    /// <summary>
    /// Scoped operation
    /// </summary>
    public class ScopedAction : IDisposable
    {
        private readonly Action _undoAction;

        private static Action CreateUndoAction<TResult>(object obj, MemberInfo memberInfo, TResult newValue)
        {
            TResult oldValue;

            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    oldValue = (TResult)fieldInfo.GetValue(obj);
                    fieldInfo.SetValue(obj, newValue);
                    return () => { fieldInfo.SetValue(obj, oldValue); };

                case PropertyInfo propertyInfo:
                    oldValue = (TResult)propertyInfo.GetMethod.Invoke(obj, Array.Empty<object>());
                    propertyInfo.SetMethod.Invoke(obj, new object[] { newValue });
                    return () => { propertyInfo.SetMethod.Invoke(obj, new object[] { oldValue }); };

                default:
                    throw new Exception("Only property and field expressions are supported");
            }
        }

        /// <summary>
        /// Create Undo for static object
        /// </summary>
        public static ScopedAction Create<TResult>(Expression<Func<TResult>> memberFunc, TResult newValue)
        {
            MemberInfo memberInfo = (memberFunc.Body as MemberExpression)?.Member;
            Action undoAction = CreateUndoAction(null, memberInfo, newValue);
            return new ScopedAction(undoAction);
        }

        /// <summary>
        /// Create Undo for instance object
        /// </summary>
        public static ScopedAction Create<T, TResult>(T obj, Expression<Func<T, TResult>> memberFunc, TResult newValue)
        {
            MemberInfo memberInfo = (memberFunc.Body as MemberExpression)?.Member;
            Action undoAction = CreateUndoAction(obj, memberInfo, newValue);
            return new ScopedAction(undoAction);
        }
        
        public ScopedAction(Action undoAction)
        {
            _undoAction = undoAction ?? throw new ArgumentNullException(nameof(undoAction));
        }

        public void Dispose()
        {
            _undoAction();
        }
    }
}
