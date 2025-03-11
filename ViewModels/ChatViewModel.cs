using CommunityToolkit.Maui.Alerts;
using MauiApp_IA_IOT.Models;
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
        private string HOSTNAME = "http://192.168.1.133/";
        public event PropertyChangedEventHandler? PropertyChanged;
        private Brush _color = Brush.LightGray;
        private string _newMessageText;

        public ObservableCollection<Message> Messages { get; set; }
        public ICommand SendMessageCommand { get; set; }

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
        public Brush Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

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
                model = "llama-3.2-1b-instruct",
                messages = new[] {
                    new { role = "user", content = instruction }
                },
                tools = new[] {
                    new {
                        type = "function",
                        function = new {
                            name = "turn_on_light",
                            description = "Turn on the light. If the user instructs 'enciende las luces', this function should be called.",
                            parameters = new {
                                type = "object",
                                properties = new { }
                            }
                        }
                    },
                    new {
                        type = "function",
                        function = new {
                            name = "turn_off_light",
                            description = "Turn off the light. If the user instructs 'apaga las luces', this function should be called.",
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

                        Message message = new Message
                        {
                            Text = toolCall["function"]?["description"]?.ToString(),
                            IsCurrentUser = false
                        };

                        if (message.Text == "turn_on_light")
                        {
                            this.Color = Brush.Yellow;
                            await SendCommandToNodeRedAsync("encender");
                            this.Messages.Add(message);
                        }
                        else if (message.Text == "turn_off_light")
                        {
                            this.Color = Brush.Gray;
                            await SendCommandToNodeRedAsync("apagar");
                            this.Messages.Add(message);
                        }
                        else
                        {
                            await Snackbar.Make($"Función desconocida: {message.Text}").Show();
                        }
                    }
                    else
                    {
                        string contentMessage = responseJson["choices"]?[0]?["message"]?["content"]?.ToString();
                        await Snackbar.Make($"Respuesta del modelo: {contentMessage}").Show();
                    }
                }
                catch (Exception ex)
                {
                    await Snackbar.Make("Error comunicándose con LM Studio: " + ex.Message).Show();
                }

                this.NewMessageText = string.Empty;
            }
        }
        private async Task SendCommandToNodeRedAsync(string color)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(HOSTNAME + color);
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
