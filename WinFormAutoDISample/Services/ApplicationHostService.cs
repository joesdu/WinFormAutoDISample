using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WinFormAutoDISample.Services.Contracts;

namespace WinFormAutoDISample.Services;

/// <summary>
/// Managed host of the application.
/// </summary>
internal sealed class ApplicationHostService(IServiceProvider sp) : IHostedService
{
    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await HandleActivationAsync();
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Creates main window during activation.
    /// </summary>
    private Task HandleActivationAsync()
    {
        if (Application.OpenForms.OfType<MainForm>().Any())
        {
            return Task.CompletedTask;
        }
        var mainWindow = sp.GetRequiredService<IWindow>();
        mainWindow.Loaded += OnMainWindowLoaded;
        mainWindow.Show();
        return Task.CompletedTask;
    }

    private static void OnMainWindowLoaded(object? sender, EventArgs e)
    {
        if (sender is not MainForm mainWindow)
        {
            return;
        }
        // Initialize the main window
    }
}