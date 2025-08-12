namespace WinFormAutoDISample.Services.Contracts;

public interface IWindow
{
    event EventHandler Loaded;

    void Show();
}