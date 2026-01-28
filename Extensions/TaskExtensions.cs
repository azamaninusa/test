using System.Linq.Expressions;
using System.Reflection;

namespace VaxCare.Core.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Chains a Task<T> to a function that returns Task<U>.
        /// In other words, it gets the result of Task<T> and uses that result 
        /// to perform a Task<U>
        /// </summary>
        /// <returns>Task<U></returns>
        public static async Task<U> Then<T, U>(this Task<T> task, Func<T, Task<U>> next)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                return await next(result).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Wrap exception with method name context from the lambda
                var methodName = GetMethodNameFromLambda(next);
                if (!string.IsNullOrEmpty(methodName))
                {
                    throw new Exception($"Failed in chain step: {methodName}", ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Chains a Task<T> to a function that returns a Task
        /// </summary>
        /// <returns>Task</returns>
        public static async Task Then<T>(this Task<T> task, Func<T, Task> next)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                await next(result).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Wrap exception with method name context from the lambda
                var methodName = GetMethodNameFromLambda(next);
                if (!string.IsNullOrEmpty(methodName))
                {
                    throw new Exception($"Failed in chain step: {methodName}", ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Chains a Task to a function that returns Task
        /// </summary>
        /// <returns>Task</returns>
        public static async Task Then(this Task task, Func<Task> next)
        {
            try
            {
                await task.ConfigureAwait(false);
                await next().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Wrap exception with method name context from the lambda
                var methodName = GetMethodNameFromLambda(next);
                if (!string.IsNullOrEmpty(methodName))
                {
                    throw new Exception($"Failed in chain step: {methodName}", ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Ends the chain of Task<T> and returns the result of that task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public static async Task Then<T>(this Task<T> task, Action<T> next)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                next(result);
            }
            catch (Exception ex)
            {
                // Wrap exception with method name context from the lambda
                var methodName = GetMethodNameFromLambda(next);
                if (!string.IsNullOrEmpty(methodName))
                {
                    throw new Exception($"Failed in chain step: {methodName}", ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Extracts method name from lambda expression for better error reporting
        /// </summary>
        private static string GetMethodNameFromLambda(Delegate lambda)
        {
            try
            {
                if (lambda?.Method != null)
                {
                    var method = lambda.Method;
                    // Check if it's a compiler-generated method (lambda)
                    if (method.DeclaringType != null && method.DeclaringType.Name.Contains("<>c"))
                    {
                        // Try to extract from the method name or target
                        var methodName = method.Name;
                        
                        // Look for Async methods in the declaring type
                        if (methodName.Contains("b__"))
                        {
                            // This is a compiler-generated lambda, try to find the actual method
                            // by looking at the target if it's a method call
                            return ExtractMethodNameFromExpression(lambda);
                        }
                    }
                    else
                    {
                        return $"{method.DeclaringType?.Name}.{method.Name}";
                    }
                }
            }
            catch
            {
                // Ignore errors in reflection
            }
            return string.Empty;
        }

        /// <summary>
        /// Tries to extract method name from expression tree
        /// </summary>
        private static string ExtractMethodNameFromExpression(Delegate lambda)
        {
            try
            {
                // For lambda expressions like: page => page.VerifyPatientIsNotListedAsync()
                // We can't easily extract this at runtime without expression trees
                // So we'll rely on stack trace parsing instead
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
