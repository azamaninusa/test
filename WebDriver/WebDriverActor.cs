using OpenQA.Selenium;
using Serilog;
using System.Threading.Channels;

namespace VaxCare.Core.WebDriver
{
    public interface IWebDriverActor : IAsyncDisposable
    {
        IWebDriver Driver { get; }
        ILogger? _logger { get; }
        Task ExecuteAsync(Func<IWebDriver, Task> action);
        Task<T> ExecuteAsync<T>(Func<IWebDriver, Task<T>> func);
        Task SetLogger(ILogger logger);
    }

    public class WebDriverActor : IWebDriverActor
    {
        private readonly Channel<Func<IWebDriver, Task>> _commandChannel;
        private readonly IWebDriver _driver;
        public IWebDriver Driver => _driver;
        private ILogger? _loggerBacking;
        public ILogger? _logger => _loggerBacking;
        private readonly Task _processingTask;
        private readonly CancellationTokenSource _cts;

        public WebDriverActor(IWebDriver driver, ILogger? logger = null)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _loggerBacking = logger;
            _commandChannel = Channel.CreateUnbounded<Func<IWebDriver, Task>>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });
            _cts = new CancellationTokenSource();
            _processingTask = ProcessCommandsAsync(_cts.Token);
        }

        public async Task SetLogger(ILogger logger)
        {
            await Task.Run(() =>
            {
                ArgumentNullException.ThrowIfNull(logger);
                _loggerBacking = logger;
            });
        }

        public async Task ExecuteAsync(Func<IWebDriver, Task> action)
        {
            var tcs = new TaskCompletionSource();
            await _commandChannel.Writer.WriteAsync(async driver =>
            {
                try
                {
                    await action(driver);
                    tcs.SetResult();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            await tcs.Task;
        }

        public async Task<T> ExecuteAsync<T>(Func<IWebDriver, Task<T>> func)
        {
            var tcs = new TaskCompletionSource<T>();
            await _commandChannel.Writer.WriteAsync(async driver =>
            {
                try
                {
                    var result = await func(driver);
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }

        private async Task ProcessCommandsAsync(CancellationToken ctx)
        {
            try
            {
                await foreach (var command in _commandChannel.Reader.ReadAllAsync(ctx))
                {
                    await command(_driver);
                }
            }
            catch (OperationCanceledException)
            {
                _logger!.Information("WebDriver actor processing stopped");
                throw;
            }
            catch (Exception ex)
            {
                _logger!.Error(ex, "Error in WebDriver actor processing loop");
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            _commandChannel.Writer.Complete();

            try
            {
                await _processingTask;
                _driver?.Quit();
            }
            catch (OperationCanceledException)
            {
                // Expected here
            }
            catch (Exception ex)
            {
                _logger!.Error(ex, "Error disposing of the WebDriver Actor");
            }

            _driver?.Dispose();
            _cts.Dispose();
        }
    }
}