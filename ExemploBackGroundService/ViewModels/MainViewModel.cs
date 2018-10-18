using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Parsers.Rss;
using ExemploBackGroundService.Models;
using Xamarin.Forms;
using MonkeyCache.SQLite;

namespace ExemploBackGroundService.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Property

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        private string _key = "rssFeed"; //Chave com o nome do objeto que armazena os dados.


        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public ObservableCollection<RssData> RSSFeed { get; }

        public MainViewModel()
        {
            RSSFeed = new ObservableCollection<RssData>();

            CarregaRSS(null);

            MessagingCenter.Subscribe<List<RssData>>(this, "Update", async (lista) =>
            {
                await CarregaRSS(lista);
            });

        }


        private ICommand _refreshNewsFeedCommand;

        public ICommand RefreshNewsFeedCommand =>
        _refreshNewsFeedCommand ?? (_refreshNewsFeedCommand = new Command(
        async () =>
        {
            await CarregaRSS(null);
        }));



        private async Task CarregaRSS(List<RssData> lista = null)
        {
            IsBusy = true;
            RSSFeed.Clear();

            if (lista == null)
            {
                lista = Barrel.Current.Get<List<RssData>>(_key) ?? new List<RssData>();

            }

            foreach (var rssData in lista)
            {
                RSSFeed.Add(rssData);
            }

            IsBusy = false;
        }

    }
}
