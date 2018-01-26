using EFApp.Presistance;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EFApp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendFilePage : ContentPage
    {

        private ObservableCollection<Person> _contacts;
        private SQLiteAsyncConnection _connection;
        private bool _isDataLoaded;
        public SendFilePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

        }

        protected override async void OnAppearing()
        {
            // In a multi-page app, everytime we come back to this page, OnAppearing
            // method is called, but we want to load the data only the first time
            // this page is loaded. In other words, when we go to ContactDetailPage
            // and come back, we don't want to reload the data. The data is already
            // there. We can control this using a switch: isDataLoaded.
            if (_isDataLoaded)
                return;

            _isDataLoaded = true;

            // I've extracted the logic for loading data into LoadData method. 
            // Now the code in OnAppearing method looks a lot cleaner. The 
            // purpose is very explicit. If data is loaded, return, otherwise,
            // load data. Details of loading the data is delegated to LoadData
            // method. 
            await LoadData();

            base.OnAppearing();
        }

        private async Task LoadData()
        {
            await _connection.CreateTableAsync<Person>();

            var contacts = await _connection.Table<Person>().ToListAsync();

            _contacts = new ObservableCollection<Person>(contacts);
            listView.ItemsSource = _contacts;
        }

        async void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            var contact = e.SelectedItem as Person;
            var page = new SendFileDetailPage(contact);
            await Navigation.PushModalAsync(new NavigationPage(page));
        }
    }
}