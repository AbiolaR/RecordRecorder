using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Record.Recorder.Core
{
    public static class ExpressionHelpers
    {
        /// <summary>
        /// Compiles an expression and gets the functions return value
        /// </summary>
        /// <typeparam name="T">The tyoe of return value</typeparam>
        /// <param name="lambdaExpression">The expression to compile</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(this Expression<Func<T>> lambdaExpression)
        {
            return lambdaExpression.Compile().Invoke();
        }

        /// <summary>
        /// Sets the the underlying properties value to the given value from an expression that contains the property
        /// </summary>
        /// <typeparam name="T">The type of value to set</typeparam>
        /// <param name="expression">The expression</param>
        /// <param name="value">The value to set the property to</param>
        public static void SetPropertyValue<T>(this Expression<Func<T>> expression, T value)
        {
            // Converts from () => some.Property to some.Property
            var lambdaExpression = (expression as LambdaExpression).Body as MemberExpression;

            var propertyInfo = (PropertyInfo)lambdaExpression.Member;
            var target = Expression.Lambda(lambdaExpression.Expression).Compile().DynamicInvoke();

            propertyInfo.SetValue(target, value);
        }
    }
}
