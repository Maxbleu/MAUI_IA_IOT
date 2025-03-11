using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp_IA_IOT;

public class NodeRedSettingsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
	public NodeRedSettingsViewModel()
	{
		
	}

    #region INotifyPropertyChanged
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion

}