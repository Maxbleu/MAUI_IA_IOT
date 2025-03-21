using MauiApp_IA_IOT.ViewModels;

namespace MauiApp_IA_IOT.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel settingsModeloViewModel)
	{
		InitializeComponent();
		this.BindingContext = settingsModeloViewModel;
    }
}