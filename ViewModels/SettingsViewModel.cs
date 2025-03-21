using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using MauiApp_IA_IOT.Models;
using MauiApp_IA_IOT.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MauiApp_IA_IOT.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    //  CAMPOS NODERED
    private Brush _colorNodeRed = Brush.Red;
    private string _protocolNodeRed = "http";
    private string _hostNodeRed = "localhost";
    private string _portNodeRed = "1880";
    private string _colorSeleccionado;

    private bool _isEnabledButtonNodeRed = false;
    private bool _isNodeRedServiceRunning = false;
    private bool _isRequestNodeRedSucessful = false;

    //  CAMPOS MODELOS
    private Brush _colorModelos = Brush.Red;
    private string _protocolModelos = "http";
    private string _hostModelos = "localhost";
    private string _portModelos = "1234";

    private string _functionName;
    private string _modeloSeleccionado;

    private bool _isEnabledButtonModelos = false;
    private bool _isModeloServiceRunning = false;

    //  PROPIEDADES MODELOS
    public Brush ColorModelos
    {
        get => this._colorModelos;
        set
        {
            if (this._colorModelos != value)
            {
                this._colorModelos = value;
                OnPropertyChanged();
            }
        }
    }
    public string PortModelos
    {
        get => this._portModelos;
        set
        {
            if (this._portModelos != value)
            {
                this._portModelos = value;
                OnPropertyChanged();
                this.IsEnabledButtonModelos = true;
            }
        }
    }
    public string FunctionName
    {
        get => this._functionName;

        set
        {
            if (this._functionName != value)
            {
                this._functionName = value;
                OnPropertyChanged();
            }
        }
    }
    public string ProtocolModelos
    {
        get => this._protocolModelos;
        set
        {
            if (this._protocolModelos != value)
            {
                this._protocolModelos = value;
                OnPropertyChanged();
                this.IsEnabledButtonModelos = true;
            }
        }
    }
    public string HostNameModelos
    {
        get => this._hostModelos;
        set
        {
            if (this._hostModelos != value)
            {
                this._hostModelos = value;
                OnPropertyChanged();
                this.IsEnabledButtonModelos = true;
            }
        }
    }
    public bool IsEnabledButtonModelos
    {
        get => _isEnabledButtonModelos;
        set
        {
            _isEnabledButtonModelos = value;
            OnPropertyChanged();
        }
    }
    public string ModeloSeleccionado
    {
        get => _modeloSeleccionado;
        set
        {
            if (_modeloSeleccionado != value)
            {
                _modeloSeleccionado = value;
                OnPropertyChanged();
            }
        }
    }
    public bool IsModeloServiceRunning
    {
        get => _isModeloServiceRunning;
        set
        {
            if (_isModeloServiceRunning != value)
            {
                _isModeloServiceRunning = value;
                OnPropertyChanged();
            }

            this.ColorModelos = this.IsModeloServiceRunning ? Brush.Green : Brush.Red;
        }
    }
    public bool IsRequestNodeRedSucessful
    {
        get => _isRequestNodeRedSucessful;
        set
        {
            if (_isRequestNodeRedSucessful != value)
            {
                _isRequestNodeRedSucessful = value;
                OnPropertyChanged();
            }
        }
    }
    public ObservableCollection<string> Models { get; set; }

    //  PROPIEDADES NODERED
    public string ColorSeleccionado
    {
        get => _colorSeleccionado;
        set
        {
            if (_colorSeleccionado != value)
            {
                _colorSeleccionado = value;
                OnPropertyChanged();
            }
        }
    }
    public Brush ColorNodeRed
    {
        get => this._colorNodeRed;
        set
        {
            if (this._colorNodeRed != value)
            {
                this._colorNodeRed = value;
                OnPropertyChanged();
            }
        }
    }
    public string ProtocolNodeRed
    {
        get => this._protocolNodeRed;
        set
        {
            if (this._protocolNodeRed != value)
            {
                this._protocolNodeRed = value;
                OnPropertyChanged();
                this.IsEnabledButtonNodeRed = true;
            }
        }
    }
    public string HostNameNodeRed
    {
        get => this._hostNodeRed;
        set
        {
            if (this._hostNodeRed != value)
            {
                this._hostNodeRed = value;
                OnPropertyChanged();
                this.IsEnabledButtonNodeRed = true;
            }
        }
    }
    public string PortNodeRed
    {
        get => this._portNodeRed;
        set
        {
            if (this._portNodeRed != value)
            {
                this._portNodeRed = value;
                OnPropertyChanged();
                this.IsEnabledButtonNodeRed = true;
            }
        }
    }
    public bool IsEnabledButtonNodeRed
    {
        get => _isEnabledButtonNodeRed;
        set
        {
            _isEnabledButtonNodeRed = value;
            OnPropertyChanged();
        }
    }
    public bool IsNodeRedServiceRunning
    {
        get => _isNodeRedServiceRunning;
        set
        {
            if (_isNodeRedServiceRunning != value)
            {
                _isNodeRedServiceRunning = value;
                OnPropertyChanged();
            }
            this.ColorNodeRed = this.IsNodeRedServiceRunning ? Brush.Green : Brush.Red;
        }
    }

    public ICommand ReloadModelsCommand { get; }
    public ICommand ReloadNodeRedCommand { get; }

    public SettingsViewModel()
	{
        this.Models = new ObservableCollection<string>();
        this.ReloadModelsCommand = new Command(ReloadModels);
        this.ReloadNodeRedCommand = new Command(ReloadNodeRed);

        this.LoadModelsAsync();
        this.LoadNodeRedAsync();
    }

    /// <summary>
    /// Este método se encarga de recargar
    /// la conexión con Node-Red
    /// </summary>
    /// <param name="obj"></param>
    private async void ReloadNodeRed(object obj)
    {
        await LoadNodeRedAsync();
    }
    /// <summary>
    /// Este método se encarga de enviar una orden
    /// que ha sido seleccionada procesada
    /// por el modelo al servicio de NodeRed
    /// </summary>
    /// <param name="isEncenderCommand"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public async Task SendCommandToNodeRedAsync(bool isEncenderCommand, string color)
    {
        using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
        {
            string path = isEncenderCommand ? "/encender" : "/apagar";
            try
            {
                string url = ThingsUtils.GetUrl(this.ProtocolNodeRed, this.HostNameNodeRed, this.PortNodeRed, path + "?color=" + color);
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    ThingsUtils.SendSnakbarMessage("Error al enviar la orden a Node-RED: " + response.ReasonPhrase);
                }
                else
                {
                    this.IsRequestNodeRedSucessful = true; 
                }
            }
            catch (Exception ex)
            {
                ThingsUtils.SendSnakbarMessage("Error al comunicarse con Node-RED: " + ex.Message);
            }
        }
    }
    /// <summary>
    /// Este método se encarga de cargar la conexión
    /// con NodeRed
    /// </summary>
    /// <returns></returns>
    public async Task LoadNodeRedAsync()
    {
        using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
        {
            try
            {
                string url = ThingsUtils.GetUrl(this.ProtocolNodeRed, this.HostNameNodeRed, this.PortNodeRed, "");
                var response = await client.GetAsync(url);
                if(response.IsSuccessStatusCode)
                {
                    NotifyResponseRequestNodeRed("Se conectado con NodeRed", false, true);
                }
            }
            catch (Exception ex)
            {
                NotifyResponseRequestNodeRed("Error comunicándose con NodeRed: " + ex.Message, true, false);
            }
        }
    }
    /// <summary>
    /// Este método se encarga de enviar un snakbar 
    /// con un mensaje indicando el estado del modelo posteriormente
    /// habilito o desabilito el botón e indico que el modelo esta
    /// corriendo de manera normal para NodeRed
    /// </summary>
    /// <param name="mensajeSnakBar"></param>
    /// <param name="isEnabledButton"></param>
    /// <param name="isNodeRedServiceRunning"></param>
    private void NotifyResponseRequestNodeRed(string mensajeSnakBar, bool isEnabledButton, bool isNodeRedServiceRunning)
    {
        ThingsUtils.SendSnakbarMessage(mensajeSnakBar);
        this.IsEnabledButtonNodeRed = isEnabledButton;
        this.IsNodeRedServiceRunning = isNodeRedServiceRunning;
    }


    /// <summary>
    /// Este método se encarga de recargar la conexión
    /// con los modelos.
    /// </summary>
    /// <param name="obj"></param>
    private async void ReloadModels(object obj)
    {
        await LoadModelsAsync();
    }
    /// <summary>
    /// Este método se encarga de enviar un mensaje
    /// introducido por el usuario referido por
    /// por el tipo de tool que desea que el modleo
    /// ejecute.
    /// </summary>
    /// <param name="instruction"></param>
    /// <returns></returns>
    public async Task SendMessageToLLMAsync(string instruction)
    {
        var requestData = new
        {
            model = this.ModeloSeleccionado,
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
        using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
        {
            string json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                string url = ThingsUtils.GetUrl(this.ProtocolModelos, this.HostNameModelos, this.PortModelos, "/v1/chat/completions");
                var response = await client.PostAsync(url, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject responseJson = JObject.Parse(responseBody);

                JToken toolCalls = responseJson["choices"]?[0]?["message"]?["tool_calls"];
                if (toolCalls != null && toolCalls.HasValues)
                {
                    bool isEncenderCommand = true;
                    this.FunctionName = toolCalls.First["function"]?["name"]?.ToString();
                    if (this.FunctionName.Contains("turn_on"))
                    {
                        isEncenderCommand = true;
                        switch (this.FunctionName)
                        {
                            case "turn_on_light_red":
                                this.ColorSeleccionado = "rojas";
                                break;
                            case "turn_on_light_blue":
                                this.ColorSeleccionado = "azules";
                                break;
                            case "turn_on_light_green":
                                this.ColorSeleccionado = "verdes";
                                break;
                            default:
                                ThingsUtils.SendSnakbarMessage("No se ha encontrado una instrucción válida");
                                return;
                        }
                    }
                    else if (this.FunctionName.Contains("turn_off"))
                    {
                        isEncenderCommand = false;
                        switch (this.FunctionName)
                        {
                            case "turn_off_light_red":
                                this.ColorSeleccionado = "rojas";
                                break;
                            case "turn_off_light_blue":
                                this.ColorSeleccionado = "azules";
                                break;
                            case "turn_off_light_green":
                                this.ColorSeleccionado = "verdes";
                                break;
                            default:
                                ThingsUtils.SendSnakbarMessage("No se ha encontrado una instrucción válida");
                                return;
                        }
                    }
                    else
                    {
                        ThingsUtils.SendSnakbarMessage("No se ha encontrado una instrucción válida");
                        return;
                    }
                    await SendCommandToNodeRedAsync(isEncenderCommand, this.ColorSeleccionado);
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
    }
    /// <summary>
    /// Este método se encarga de cargar los modelos
    /// que existen en la dirección ip que hemos indicado.
    /// </summary>
    public async Task LoadModelsAsync()
    {
        this.Models.Clear();
        HttpClient client = null;
        string url = "";
        try
        {
            client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            url = ThingsUtils.GetUrl(this.ProtocolModelos, this.HostNameModelos, this.PortModelos, "/api/v0/models/");
            var response = await client.GetAsync(url);

            string responseBody = await response.Content.ReadAsStringAsync();
            var objResponse = JsonConvert.DeserializeObject<ResponseModels>(responseBody);
            List<ModelIA> models = objResponse.Data.ToList();

            models = models.Where(obj => obj.State == "loaded").ToList();
            if (models.Count == 0)
            {
                NotifyResponseRequestModels("No hay modelos cargados en este host", true, false);
                return;
            }

            models.ForEach(obj => this.Models.Add(obj.Id));
            this.ModeloSeleccionado = this.Models[0];

            NotifyResponseRequestModels("Se han cargado correctamente los modelos", false, true);

        }
        catch (Exception ex)
        {
            NotifyResponseRequestModels("No se han encontrado modelos en el host indicado", true, false);
        }
    }
    /// <summary>
    /// Este método se encarga de enviar un snakbar 
    /// con un mensaje indicando el estado del modelo posteriormente
    /// habilito o desabilito el botón e indico que el modelo esta
    /// corriendo de manera normal para los modelos
    /// </summary>
    /// <param name="mensajeSnakBar"></param>
    /// <param name="isEnabledButton"></param>
    /// <param name="isModeloServiceRunning"></param>
    private void NotifyResponseRequestModels(string mensajeSnakBar, bool isEnabledButton, bool isModeloServiceRunning)
    {
        ThingsUtils.SendSnakbarMessage(mensajeSnakBar);
        this.IsEnabledButtonModelos = isEnabledButton;
        this.IsModeloServiceRunning = isModeloServiceRunning;
    }

    #region INotifyPropertyChanged
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion
}