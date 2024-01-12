using System.Linq;
using System.Threading.Tasks;
using TSD.Model;
using TSD.Model.User;
using TSD.Services;
using TSD.Services.Extensions;
using TSD.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using ResourcesTerminal = TSD.Resources;

namespace TSD.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UniversalView : ContentPage
    {
        private bool _disposed = false;
        public UniversalView()
        {
            BindingContext = App.GetViewModel<UniversalViewModel>();

            InitializeComponent();
            ListField.ItemTapped += List_ItemTapped;
            
            MessagingCenter.Subscribe<MessageClass<AbstractColumns>>(this, ResourcesTerminal.MsgCenterTagOpenDetail, async x =>
            {
                MessagingCenter.Send(new MessageClass<bool>(true), ResourcesTerminal.MsgCenterTagLoadingView);

                if (UserAccount.IsBlock)
                {
                    MessagingCenter.Send(new MessageClass<bool>(false), ResourcesTerminal.MsgCenterTagLoadingView);
                    return;
                }

               Device.BeginInvokeOnMainThread(async () =>
                {
                    UserAccount.IsBlock = true;

                    if (x.Data.IsBlock)
                    {
                        if (await DisplayAlert(ResourcesTerminal.Information, ResourcesTerminal.MessageWarningPositionBlockContinue, ResourcesTerminal.Yes, ResourcesTerminal.No))
                        {
                            x.Data.ColumnsElement = x.Data.ColumnsElement.Select(element =>
                            {
                                if (element.Modif && element.Nullable)
                                {
                                    element.Value = element.DataType.ToUpper() == ResourcesTerminal.TypeNumeric ? default(int).ToString() : string.Empty;
                                }

                                return element;
                            }).ToObservableCollection();

                            await Navigation.PushAsync(new UniversalDetailView(x.Data));
                        }
                    }
                    else
                    {
                        if (x.Data.IsOpen)
                        {
                            if (await DisplayAlert(ResourcesTerminal.Information, ResourcesTerminal.MessageSeeRepeatPosition, ResourcesTerminal.Yes, ResourcesTerminal.No))
                            {
                                _disposed = true;
                                await Navigation.PushAsync(new UniversalDetailView(x.Data));
                            }
                        }
                        else
                        {
                            _disposed = true;
                            await Navigation.PushAsync(new UniversalDetailView(x.Data));
                        }
                    }

                    UserAccount.IsBlock = false;


                    MessagingCenter.Send(new MessageClass<bool>(false), ResourcesTerminal.MsgCenterTagLoadingView);
                });
                
            });

            MessagingCenter.Subscribe<MessageClass<object>>(this, ResourcesTerminal.MsgCenterTagCloseDetail, x =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopAsync();
                });
            });

            MessagingCenter.Subscribe<MessageClass<bool>>(this, ResourcesTerminal.MsgCenterTagBlockToolbar, x =>
            {
                UniversalViePage.ToolbarItems.ForEach(value =>
                {
                    value.IsEnabled = !x.Data;
                });
            });

            MessagingCenter.Send(new MessageClass<object>(), ResourcesTerminal.MsgCenterUpdateTypeTransfer);
        }

        private void List_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e == null) return;

            ((ListView)sender).SelectedItem = null;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is AbstractColumns item))
                return;

            ((ListView)sender).SelectedItem = null;

            MessagingCenter.Send(new MessageClass<AbstractColumns>(item), ResourcesTerminal.MsgCenterTagOpenDetail);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (!_disposed)
                MessagingCenter.Send(new MessageClass<object>(), ResourcesTerminal.MsgCenterTagCloseContentDialog);
        }
    }
}