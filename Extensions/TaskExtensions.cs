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
            var result = await task.ConfigureAwait(false);
            return await next(result).ConfigureAwait(false);
        }

        /// <summary>
        /// Chains a Task<T> to a function that returns a Task
        /// </summary>
        /// <returns>Task</returns>
        public static async Task Then<T>(this Task<T> task, Func<T, Task> next)
        {
            var result = await task.ConfigureAwait(false);
            await next(result).ConfigureAwait(false);
        }

        /// <summary>
        /// Chains a Task to a function that returns Task
        /// </summary>
        /// <returns>Task</returns>
        public static async Task Then(this Task task, Func<Task> next)
        {
            await task.ConfigureAwait(false);
            await next().ConfigureAwait(false);
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
            var result = await task.ConfigureAwait(false);
            next(result);
        }
    }
}
