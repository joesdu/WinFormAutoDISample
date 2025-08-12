using EasilyNET.Core.Misc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WinFormAutoDISample.Common;
using WinFormAutoDISample.Properties;

namespace WinFormAutoDISample;

internal sealed class App : ApplicationContext
{
    private static App? _current;
    private static ILogger<App>? _logger;

    private App(ref IHost host)
    {
        Host = host;
        Application.ApplicationExit += OnApplicationExit;
        Application.ThreadException += OnThreadException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    private static App Current => _current ?? throw new InvalidOperationException("应用程序尚未初始化");

    private IHost Host { get; }

    /// <summary>
    /// Services
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public static IServiceProvider Services => CurrentAppHost.Services;

    private static IHost CurrentAppHost => Current.Host ?? throw new InvalidOperationException("无法获取AppHost，当前Application实例不是App类型。");

    public static void Initialize(ref IHost host)
    {
        _current = new(ref host);
        _logger = host.Services.GetRequiredService<ILogger<App>>();
    }

    private async void OnApplicationExit(object? sender, EventArgs e)
    {
        try
        {
            WinApis._mutex.ReleaseMutex();
            await Host.StopAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "OnApplicationExit");
        }
    }

    private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
        HandleException(e.Exception);
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        HandleException(e.ExceptionObject as Exception);
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        HandleException(e.Exception);
        e.SetObserved();
    }

    private static void HandleException(Exception? ex)
    {
        if (ex is null) return;
        var errorMessage = AppResource.Program_HandleHostInitializationAsync_应用程序启动时发生错误X.Format(ex.Message);
        MessageBox.Show(errorMessage, AppResource.Program_HandleHostInitializationAsync_错误, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}