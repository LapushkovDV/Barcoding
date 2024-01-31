using TSD.Services.Interfaces;
using TSD.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ResourcesTerminal = TSD.Resources;

namespace TSD.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            BindingContext = App.GetViewModel<LoginPageViewModel>();

            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await Application.Current.MainPage.DisplayAlert("Подтверждение выхода", "Вы действительно хотите выйти из системы?", ResourcesTerminal.Yes, ResourcesTerminal.No))
                {
                    DependencyService.Get<IAndroidMethods>().CloseApp();
                }
            });

            
            return true;
        }
    }
}