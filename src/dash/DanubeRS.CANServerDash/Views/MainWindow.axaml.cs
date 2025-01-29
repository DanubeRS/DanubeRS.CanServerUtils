using Avalonia.Controls;
using Avalonia.Platform;

namespace DanubeRS.CANServerDash.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        WindowState = WindowState.FullScreen;
        InitializeComponent();
    }
}