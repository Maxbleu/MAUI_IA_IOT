using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp_IA_IOT;

public class ModeloSettingsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
	public ModeloSettingsViewModel()
	{
		
	}

    #region INotifyPropertyChanged
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion
}