using EasilyNET.AutoDependencyInjection.Core.Attributes;
using EasilyNET.Core.Misc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WinFormAutoDISample.Listeners;
using WinFormAutoDISample.Messenger;
using WinFormAutoDISample.Properties;
using WinFormAutoDISample.Services.Contracts;

// ReSharper disable AsyncVoidMethod
// ReSharper disable ClassNeverInstantiated.Global

namespace WinFormAutoDISample;

/// <summary>
/// 主窗体
/// </summary>
[DependencyInjection(ServiceLifetime.Singleton, AsType = typeof(IWindow))]
public partial class MainForm : Form, IWindow
{
    private readonly ILogger<MainForm> _logger;
    private readonly IMessenger _messenger;
    private readonly PerformanceListener _performanceListener;

    /// <inheritdoc />
    public event EventHandler? Loaded;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MainForm(ILogger<MainForm> logger, IMessenger messenger, PerformanceListener performanceListener)
    {
        InitializeComponent();
        _logger = logger;
        _messenger = messenger;
        _performanceListener = performanceListener;
        FormClosed += MainForm_FormClosed;
    }

    private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        _performanceListener.CpuUsageUpdated -= OnCpuUsageUpdated;
        _performanceListener.MemoryUsageUpdated -= MemoryUsageUpdated;
        _messenger.Unregister<MainForm>(this); // 注销当前窗体的消息接收
        _logger.LogInformation("MainForm closed.");
        // 释放资源
        _performanceListener.Dispose();
        Application.Exit();
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        Loaded?.Invoke(this, e);
        _performanceListener.CpuUsageUpdated += OnCpuUsageUpdated;
        _performanceListener.MemoryUsageUpdated += MemoryUsageUpdated;
    }

    private void MemoryUsageUpdated(object? sender, Tuple<double, string?> e)
    {
        try
        {
            Invoke(() =>
            {
                if (IsDisposed)
                    return;
                tsslRam.Text = $@"{e.Item1} {e.Item2}";
            });
        }
        catch (TaskCanceledException)
        {
            // Ignore
        }
    }

    private void OnCpuUsageUpdated(object? sender, Tuple<double, string?> e)
    {
        try
        {
            Invoke(() =>
            {
                if (IsDisposed)
                    return;
                tsslCpu.Text = $@"{e.Item1} {e.Item2}";
            });
        }
        catch (TaskCanceledException)
        {
            // Ignore
        }
    }

    private void button1_Click(object sender, EventArgs e)
    {
        // 获取当前运行时的版本信息
        var version = Environment.Version;
        Invoke(() =>
        {
            if (IsDisposed)
                return;
            label1.Text = AppResource.MainForm_Hello_Click.Format(version);
        });
    }
}