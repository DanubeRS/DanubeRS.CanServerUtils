using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;

namespace DanubeRS.CANServerDash.ViewModels;

public partial class MainViewModel : ReactiveObject
{
    private decimal _Slider = 0;
    public decimal Slider
    {
        get => _Slider;
        set
        {
            this.RaiseAndSetIfChanged(ref _Slider, value);
        }
    }
}
