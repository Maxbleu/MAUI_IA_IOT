using CommunityToolkit.Maui.Alerts;
using MauiApp_IA_IOT.Models;
using MauiApp_IA_IOT.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace MauiApp_IA_IOT.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private static readonly string LLM_URL = "http://192.168.1.130:1234/v1/chat/completions";
        private string HOSTNAME = "http://192.168.1.130:1880/encender?command=";

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
        }

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
            await SendInstructionToLLMAsync(this.NewMessageText);
        }
        private async Task SendInstructionToLLMAsync(string instruction)
        {
            var requestData = new
            {
                model = "llama-3.2-3b-instruct",
                messages = new[] {
                    new { role = "user", content = instruction }
                },
                tools = new[] {
                    new {
                        type = "function",
                        function = new {
                            name = "turn_on_light_red",
                            description = "Turn on the light. If the user instructs 'enciende las luces rojas', this function should be called.",
                            parameters = new {
                                type = "object",
                                properties = new { }
                            }
                        }
                    },
                    new {
                        type = "function",
                        function = new {
                            name = "turn_off_light_red",
                            description = "Turn on the light. If the user instructs 'apaga las luces rojas', this function should be called.",
                            parameters = new {
                                type = "object",
                                properties = new { }
                            }
                        }
                    },
                    new {
                        type = "function",
                        function = new {
                            name = "turn_on_light_blue",
                            description = "Turn on the light. If the user instructs 'enciende las luces azules', this function should be called.",
                            parameters = new {
                                type = "object",
                                properties = new { }
                            }
                        }
                    },
                    new {
                        type = "function",
                        function = new {
                            name = "turn_off_light_blue",
                            description = "Turn on the light. If the user instructs 'apaga las luces azules', this function should be called.",
                            parameters = new {
                                type = "object",
                                properties = new { }
                            }
                        }
                    },
                    new {
                        type = "function",
                        function = new {
                            name = "turn_on_light_green",
                            description = "Turn on the light. If the user instructs 'enciende las luces verdes', this function should be called.",
                            parameters = new {
                                type = "object",
                                properties = new { }
                            }
                        }
                    },
                    new {
                        type = "function",
                        function = new {
                            name = "turn_off_light_green",
                            description = "Turn on the light. If the user instructs 'apaga las luces verdes', this function should be called.",
                            parameters = new {
                                type = "object",
                                properties = new { }
                            }
                        }
                    }
                }
            };
            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(LLM_URL, content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject responseJson = JObject.Parse(responseBody);

                    JToken toolCalls = responseJson["choices"]?[0]?["message"]?["tool_calls"];
                    if (toolCalls != null && toolCalls.HasValues)
                    {
                        var toolCall = toolCalls.First;
                        string functionName = toolCall["function"]?["name"]?.ToString();

                        string color = null;
                        if(functionName.Contains("turn_on"))
                        {
                            if (functionName == "turn_on_light_red")
                            {
                                this.ColorRojo = Brush.Red;
                                await SendCommandToNodeRedAsync("encender");
                                color = "rojas";
                            }
                            else if (functionName == "turn_on_light_blue")
                            {
                                this.ColorAzul = Brush.Blue;
                                //await SendCommandToNodeRedAsync("encender_luces_azules");
                                color = "azules";
                            }
                            else if (functionName == "turn_on_light_green")
                            {
                                this.ColorVerde = Brush.Green;
                                //await SendCommandToNodeRedAsync("encender_luces_verdes"
                                color = "verdes";
                            }
                        }
                        else
                        {

                            if (functionName == "turn_off_light_red")
                            {
                                this.ColorRojo = Brush.DarkRed;
                                await SendCommandToNodeRedAsync("apagar");
                                color = "rojas";
                            }
                            else if (functionName == "turn_off_light_blue")
                            {
                                this.ColorAzul = Brush.DarkBlue;
                                //await SendCommandToNodeRedAsync("apagar_luces_azules");
                                color = "azules";
                            }
                            else if (functionName == "turn_off_light_green")
                            {
                                this.ColorVerde = Brush.DarkGreen;
                                //await SendCommandToNodeRedAsync("apagar_luces_verdes");
                                color = "verdes";
                            }
                        }

                        this.Messages.Add(
                            new Message
                            {
                                Text = functionName.Contains("turn_on") ? $"Las luces {color} han sido encendidas": $"Las luces {color} han sido apagadas",
                                IsCurrentUser = false
                            }
                        );
                    }
                    else
                    {
                        string contentMessage = responseJson["choices"]?[0]?["message"]?["content"]?.ToString();
                        ThingsUtils.SendSnakbarMessage($"Respuesta del modelo: {contentMessage}");
                    }
                }
                catch (Exception ex)
                {
                    ThingsUtils.SendSnakbarMessage("Error comunicándose con LM Studio: " + ex.Message);
                }
            }
            this.NewMessageText = string.Empty;
        }
        private async Task SendCommandToNodeRedAsync(string command)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = HOSTNAME + command;
                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        await Snackbar.Make("Error al enviar la orden a Node-RED: " + response.ReasonPhrase).Show();
                    }
                }
                catch (Exception ex)
                {
                    await Snackbar.Make("Error al comunicarse con Node-RED: " + ex.Message).Show();
                }
            }
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
