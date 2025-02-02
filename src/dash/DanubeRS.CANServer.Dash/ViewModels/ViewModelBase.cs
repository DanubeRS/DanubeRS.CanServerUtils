using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DanubeRS.CANServer.Dash.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    // This event is implemented by "INotifyPropertyChanged" and is all we need to inform 
    // our View about changes.
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}