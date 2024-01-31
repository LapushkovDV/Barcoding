using TSD.Views;
using Xamarin.Forms;

namespace TSD
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            Navigation.PushModalAsync(new LoginPage());
        }
    }
}
