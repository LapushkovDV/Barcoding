using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSD.Model;
using TSD.Model.User;
using TSD.Services;
using TSD.Services.Extensions;
using ResourcesTerminal = TSD.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace TSD.Views.FlyoutPages.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectFiltrIdent : ContentView
    {
        public SelectFiltrIdent()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<MessageClass<object>>(this, ResourcesTerminal.MsgCenterTagCancelSearch, x =>
            {
                searchBar.Text = string.Empty;
            });

            MessagingCenter.Subscribe<MessageClass<string>>(this, ResourcesTerminal.MsgCenterTagUpdateTextSearch, x =>
            {
                searchBar.Text = x.Data;
            });


        }



    }
}