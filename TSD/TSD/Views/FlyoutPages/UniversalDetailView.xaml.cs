using TSD.Model;
using TSD.Services;
using TSD.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static System.Net.Mime.MediaTypeNames;
using ResourcesTerminal = TSD.Resources;

namespace TSD.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UniversalDetailView : ContentPage
    {
        public UniversalDetailView(AbstractColumns element)
        {
            BindingContext = App.GetViewModel<UniversalDetailViewModel>();

            MessagingCenter.Send(new MessageClass<AbstractColumns>(element), ResourcesTerminal.MsgCenterTagSendDataDetailView);
            InitializeComponent();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e == null) return;

            ((ListView)sender).SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Send(new MessageClass<bool>(true), ResourcesTerminal.MsgCenterTagIsOpenDetail);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Send(new MessageClass<object>(), ResourcesTerminal.MsgCenterTagSyncRow);
            MessagingCenter.Send(new MessageClass<object>(), ResourcesTerminal.MsgCenterTagSaveDocumentLocal);
            MessagingCenter.Send(new MessageClass<object>(), ResourcesTerminal.MsgCenterTagUpdateUniversal);
            MessagingCenter.Send(new MessageClass<bool>(false), ResourcesTerminal.MsgCenterTagIsOpenDetail);
            MessagingCenter.Send(new MessageClass<object>(), ResourcesTerminal.MsgCenterTagUpdateSelect);
        }

        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            MessagingCenter.Send(new MessageClass<Editor>(((Editor)sender)), "SelectEntry");
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            MessagingCenter.Send(new MessageClass<Editor>(null), "SelectEntry");
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((Editor)sender).Keyboard == Keyboard.Numeric)
            {
                ((Editor)sender).Text = e.NewTextValue.Replace(",", ".");
            }        
        }
    }
}
