using TSD.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TSD.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            BindingContext = App.GetViewModel<SettingPageViewModel>();
            InitializeComponent();
        }
    }
}