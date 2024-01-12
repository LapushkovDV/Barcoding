
using TSD.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TSD.Views.FlyoutPages.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskView : ContentView
    {
        public TaskView()
        {
            BindingContext = App.GetViewModel<TaskViewModel>();
            InitializeComponent();
        }
    }
}