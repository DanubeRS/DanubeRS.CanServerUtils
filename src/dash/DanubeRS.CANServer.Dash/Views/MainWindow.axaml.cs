using Avalonia.Controls;
using Avalonia.Platform;

namespace DanubeRS.CANServer.Dash.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        WindowState = WindowState.FullScreen;
        InitializeComponent();
    }
}