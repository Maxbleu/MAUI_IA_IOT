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
            this._settingsModeloViewModel.LoadModelsAsync();
            this._settingsModeloViewModel.LoadNodeRedAsync();
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
            if(e.PropertyName == "FunctionName" && !String.IsNullOrEmpty(this._settingsModeloViewModel.FunctionName))
            {
                string color = null;
                string functionName = this._settingsModeloViewModel.FunctionName;
                if (functionName.Contains("turn_on"))
                {
                    if (functionName == "turn_on_light_red")
                    {
                        this.ColorRojo = Brush.Red;
                        color = "rojas";
                        await this._settingsModeloViewModel.SendCommandToNodeRedAsync(true, color);
                    }
                    else if (functionName == "turn_on_light_blue")
                    {
                        this.ColorAzul = Brush.Blue;
                        color = "azules";
                        await this._settingsModeloViewModel.SendCommandToNodeRedAsync(true, color);
                    }
                    else if (functionName == "turn_on_light_green")
                    {
                        this.ColorVerde = Brush.Green;
                        color = "verdes";
                        await this._settingsModeloViewModel.SendCommandToNodeRedAsync(true, color);
                    }
                }
                else
                {
                    if (functionName == "turn_off_light_red")
                    {
                        this.ColorRojo = Brush.DarkRed;
                        color = "rojas";
                        await this._settingsModeloViewModel.SendCommandToNodeRedAsync(false, color);
                    }
                    else if (functionName == "turn_off_light_blue")
                    {
                        this.ColorAzul = Brush.DarkBlue;
                        color = "azules";
                        await this._settingsModeloViewModel.SendCommandToNodeRedAsync(false, color);
                    }
                    else if (functionName == "turn_off_light_green")
                    {
                        this.ColorVerde = Brush.DarkGreen;
                        color = "verdes";
                        await this._settingsModeloViewModel.SendCommandToNodeRedAsync(false, color);
                    }
                }

                this.Messages.Add(
                    new Message
                    {
                        Text = functionName.Contains("turn_on") ? $"Las luces {color} han sido encendidas" : $"Las luces {color} han sido apagadas",
                        IsCurrentUser = false
                    }
                );

                this._settingsModeloViewModel.FunctionName = string.Empty;
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
