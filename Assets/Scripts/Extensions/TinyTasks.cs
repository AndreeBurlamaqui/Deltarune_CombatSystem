using System;
using System.Threading.Tasks;

namespace TinyCacto.Utils
{
    public static class TinyTasks
    {
        /// <summary>
        /// Blocks while condition is true or timeout occurs.
        /// <para>To use along async method.</para>
        /// </summary>
        /// <param name="condition">The condition that will perpetuate the block.</param>
        /// <param name="frequency">The frequency at which the condition will be check, in milliseconds.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static async Task WaitWhile(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                throw new TimeoutException();
        }

        /// <summary>
        /// Blocks until condition is true or timeout occurs.
        /// <para>To use along async method.</para>
        /// </summary>
        /// <param name="condition">The break condition.</param>
        /// <param name="frequency">The frequency at which the condition will be checked.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <returns></returns>
        public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask,
                    Task.Delay(timeout)))
                throw new TimeoutException();
        }

        /// <summary>
        /// Wait until condition is true or timeout occurs and then call an action.
        /// </summary>
        /// <param name="condition">The break condition. Needs to be a method, otherwise it'll not update the check. </param>
        /// <param name="toCall">Delegate to call after the condition is fulfilled.</param>
        /// <param name="frequency">The frequency in milisseconds at which the condition will be checked.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <returns></returns>
        public static async void WaitUntilThenCall(Func<bool> condition, Action toCall, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask,
                    Task.Delay(timeout)))
                throw new TimeoutException();

            toCall?.Invoke();
        }

        /// <summary>
        /// Wait a set milliseconds and then call an action.
        /// </summary>
        public static async void WaitDelayThenCall(int milliseconds, Action invoke)
        {
            await Task.Delay(milliseconds);

            invoke?.Invoke();
        }
    }
}
