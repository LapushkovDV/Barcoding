using System;
using System.Linq;
using TSD.Model;
using TSD.Model.User;
using TSD.Services;
using TSD.Services.Extensions;
using TSD.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ResourcesTerminal = TSD.Resources;

namespace TSD.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppPage : FlyoutPage
    {
        public AppPage()
        {
            InitializeComponent();

            MessagingCenter.Send(new MessageClass<object>(), ResourcesTerminal.MsgCenterTagUpdateMenus);

            FlyoutPage.ListView.ItemSelected += ListView_ItemSelected;

            MessagingCenter.Subscribe<MessageClass<object>>(this, ResourcesTerminal.MsgCenterTagDefaultPage, x =>
            {
                if (UserAccount.SelectedMenu != null)
                {
                    UserAccount.SelectedMenu.Fields = UserAccount.SelectedMenu.Fields.Select(element => { element.Value = string.Empty; return element; }).ToObservableCollection();
                    UserAccount.SelectedMenu.ColumnsValue.Clear();
                    UserAccount.SelectedMenu.CurrentDocument = new CurrentDocument();
                }

                var page = (Page)Activator.CreateInstance(typeof(UniversalView));

                Detail = new NavigationPage(page);
                IsPresented = false;
            });

            MessagingCenter.Subscribe<MessageClass<object>>(this, ResourcesTerminal.MsgCenterTagCloseHamburger, x =>
            {
                IsPresented = !IsPresented;
            });
            MessagingCenter.Subscribe<MessageClass<int>>(this, ResourcesTerminal.MsgCenterTagGestureFlyout, x =>
            {
                if (x.Data == 1)
                    IsGestureEnabled = true;
                else
                    IsGestureEnabled = false;
            });
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            FlyoutPage.ListView.SelectedItem = null;

            if (!(e.SelectedItem is AbstractMenu item))
                return;

            if (!item.IsEnabled) return;

            if (UserAccount.SelectedMenu != null)
            {
                UserAccount.SelectedMenu.Fields = UserAccount.SelectedMenu.Fields.Select(element => { element.Value = ""; return element; }).ToObservableCollection();
                UserAccount.SelectedMenu.ColumnsValue.Clear();
            }

            UserAccount.SelectedMenu = item;

            var page = (Page)Activator.CreateInstance(typeof(UniversalView));
            ((UniversalViewModel)page.BindingContext).HomeTitle = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MessagingCenter.Send(new MessageClass<object>(), ResourcesTerminal.MsgCenterTagUpdateUniversal);
        }

        protected override void OnDisappearing()
        {
            var page = (Page)Activator.CreateInstance(typeof(UniversalView));

            ((UniversalViewModel)page.BindingContext).HomeTitle = ResourcesTerminal.TitleHome;
            
            base.OnDisappearing();
        }
    }
}