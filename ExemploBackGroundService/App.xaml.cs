using System;
using ExemploBackGroundService.Services;
using Matcha.BackgroundService;
using MonkeyCache.SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ExemploBackGroundService
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Barrel.ApplicationId = "NewsfeedAppId";

            MainPage = new NavigationPage(new MainPage());
        }

        //Called when the application starts.
        protected override void OnStart()
        {
            StartBackgroundService();
        }

        private static void StartBackgroundService()
        {
            //Atualiza o RSS a cada 3 Minutos
            BackgroundAggregatorService.Add(() => new PeriodicRSSFeedService(1));

            //Inicia o Background Service
            BackgroundAggregatorService.StartBackgroundService();
        }
    }
}
