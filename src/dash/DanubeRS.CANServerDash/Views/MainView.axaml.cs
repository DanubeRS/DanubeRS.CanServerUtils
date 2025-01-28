using Avalonia.Controls;
using Avalonia.Interactivity;

namespace DanubeRS.CANServerDash.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        // See https://github.com/AvaloniaUI/Avalonia/issues/13598#issuecomment-1809748252
        var insetsManager = TopLevel.GetTopLevel(this)?.InsetsManager;

        if (insetsManager != null)
        {
            insetsManager.DisplayEdgeToEdge = true;
            insetsManager.IsSystemBarVisible = false;
        }
    }
}