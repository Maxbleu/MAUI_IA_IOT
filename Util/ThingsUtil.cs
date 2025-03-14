using CommunityToolkit.Maui.Alerts;

namespace MauiApp_IA_IOT.Util
{
    public class ThingsUtil
    {
        /// <summary>
        /// Este método se encarga de mostrar mensajes
        /// a partir del Sankbar
        /// </summary>
        /// <param name="message"></param>
        public static void SendSnakbarMessage(string message)
        {
            Task.Run(async () =>
            {
                await Snackbar.Make(message).Show();
            });
        }
    }
}
