using MauiApp_IA_IOT.ViewModels;

namespace MauiApp_IA_IOT
{
    public partial class ChatPage : ContentPage
    {
        public ChatPage(ChatViewModel chatViewModel)
        {
            InitializeComponent();
            this.BindingContext = chatViewModel;
        }
    }

}
