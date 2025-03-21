using MauiApp_IA_IOT.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiApp_IA_IOT.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private SettingsViewModel _settingsModeloViewModel;

        public event PropertyChangedEventHandler? PropertyChanged;
        private Brush _colorRojo = Brush.DarkRed;
        private Brush _colorAzul = Brush.DarkBlue;
        private Brush _colorVerde = Brush.DarkGreen;
        private string _newMessageText;

        public ObservableCollection<Message> Messages { get; set; }

        public string NewMessageText
        {
            get => _newMessageText;
            set
            {
                if (_newMessageText != value)
                {
                    _newMessageText = value;
                    OnPropertyChanged();
                }
            }
        }
        public Brush ColorVerde
        {
            get => this._colorVerde;
            set
            {
                if (this._colorVerde != value)
                {
                    this._colorVerde = value;
                    OnPropertyChanged();
                }
            }
        }
        public Brush ColorRojo
        {
            get => this._colorRojo;
            set
            {
                if (this._colorRojo != value)
                {
                    this._colorRojo = value;
                    OnPropertyChanged();
                }
            }
        }
        public Brush ColorAzul
        {
            get => this._colorAzul;
            set
            {
                if (this._colorAzul != value)
                {
                    this._colorAzul = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SendMessageCommand { get; set; }

        public ChatViewModel()
        {
            this.SendMessageCommand = new Command(SendMessage);
            this.Messages = new ObservableCollection<Message>();

            this._settingsModeloViewModel = IPlatformApplication.Current.Services.GetService<SettingsViewModel>();
            this._settingsModeloViewModel.PropertyChanged += SettingsModeloViewModel_PropertyChanged;
        }

        //  EVENTOS SUBCRIPTOS
        /// <summary>
        /// Este método se encarga de responder ante la modificación de
        /// valores de la propiedad FunctionName
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SettingsModeloViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsRequestNodeRedSucessful" && this._settingsModeloViewModel.IsRequestNodeRedSucessful)
            {
                if (this._settingsModeloViewModel.FunctionName.Contains("turn_on"))
                {
                    switch (this._settingsModeloViewModel.FunctionName)
                    {
                        case "turn_on_light_red":
                            this.ColorRojo = Brush.Red;
                            break;
                        case "turn_on_light_blue":
                            this.ColorAzul = Brush.Blue;
                            break;
                        case "turn_on_light_green":
                            this.ColorVerde = Brush.Green;
                            break;
                    }
                }
                else if (this._settingsModeloViewModel.FunctionName.Contains("turn_off"))
                {
                    switch (this._settingsModeloViewModel.FunctionName)
                    {
                        case "turn_off_light_red":
                            this.ColorRojo = Brush.DarkRed;
                            break;
                        case "turn_off_light_blue":
                            this.ColorAzul = Brush.DarkBlue;
                            break;
                        case "turn_off_light_green":
                            this.ColorVerde = Brush.DarkGreen;
                            break;
                    }
                }

                this.Messages.Add(
                    new Message
                    {
                        Text = this._settingsModeloViewModel.FunctionName.Contains("turn_on") ? $"Las luces {this._settingsModeloViewModel.ColorSeleccionado} han sido encendidas" : $"Las luces {this._settingsModeloViewModel.ColorSeleccionado} han sido apagadas",
                        IsCurrentUser = false
                    }
                );

                this.NewMessageText = string.Empty;
                this._settingsModeloViewModel.IsRequestNodeRedSucessful = false;
            }
        }

        /// <summary>
        /// Este método se encarga de enviar
        /// el mensaje escrito por el usuario
        /// al modelo para que infiera que tool
        /// ejecutar.
        /// </summary>
        private async void SendMessage()
        {
            if(String.IsNullOrWhiteSpace(this.NewMessageText)) return;
            this.Messages.Add
            (
                new Message
                {
                    Text = this.NewMessageText,
                    IsCurrentUser = true
                }
            );
            await this._settingsModeloViewModel.SendMessageToLLMAsync(this.NewMessageText);
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
