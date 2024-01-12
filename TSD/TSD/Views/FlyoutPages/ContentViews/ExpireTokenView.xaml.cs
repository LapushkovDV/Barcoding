
using TSD.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TSD.Views.FlyoutPages.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExpireTokenView : ContentView
    {
        public ExpireTokenView()
        {
            BindingContext = App.GetViewModel<ExpireTokenViewModel>();
            InitializeComponent();
        }
    }
}