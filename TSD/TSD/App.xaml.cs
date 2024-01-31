using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.Extensions.DependencyInjection;
using System;
using TSD.Services.DataBase;
using TSD.Services.FileServices;
using TSD.Services.Interfaces;
using TSD.ViewModels;
using Xamarin.Forms;

using ResourceTerminal = TSD.Resources;

namespace TSD
{
    public partial class App : Application
    {
        protected static IServiceProvider ServiceProvider { get; set; }
        public App(Action<IServiceCollection> addPlatformServices = null)
        {
            InitializeComponent();
            FileService.InitFolder();
            DependencyService.Get<IWorkService>().StartForegroundServiceCompact();
            WorkDataBase.AddStatuses();

            SetupServices(addPlatformServices);

            MainPage = new NavigationPage(new MainPage());
        }

        protected override async void OnStart()
        {
            AppCenter.Start("android=0e4e9141-99d5-4600-8016-a767d610cab0",
                  typeof(Analytics), typeof(Crashes), typeof(Distribute));

            if (await Crashes.HasCrashedInLastSessionAsync())
            {
                await MainPage.DisplayAlert(ResourceTerminal.Information, ResourceTerminal.MessageSessionCrash, ResourceTerminal.OK);
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private void SetupServices(Action<IServiceCollection> addPlatformServices = null)
        {
            var services = new ServiceCollection();

            addPlatformServices?.Invoke(services);

            services.AddSingleton<LoginPageViewModel>();
            services.AddSingleton<UniversalViewModel>();
            services.AddSingleton<AppPageViewModel>();
            services.AddSingleton<SettingPageViewModel>();
            services.AddSingleton<ExpireTokenViewModel>();
            services.AddSingleton<TaskViewModel>();
            services.AddSingleton<UniversalDetailViewModel>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public static ViewModelBase GetViewModel<TViewModel>()
    where TViewModel : ViewModelBase
        {
            return ServiceProvider.GetService<TViewModel>();
        }
    }
}
