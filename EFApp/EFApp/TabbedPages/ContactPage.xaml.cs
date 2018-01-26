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
	public partial class ContactPage : ContentPage
	{
        private ObservableCollection<Person> _contacts;
        private SQLiteAsyncConnection _connection;
        private bool _isDataLoaded;

        public ContactPage()
        {
            InitializeComponent();

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

        // Note that this method returns a Task, instead of void. Void should 
        // only be used for event handlers (e.g. OnAppearing). In all other cases,
        // you should return a Task or Task<T>.
        private async Task LoadData()
        {
            await _connection.CreateTableAsync<Person>();

            var contacts = await _connection.Table<Person>().ToListAsync();

            _contacts = new ObservableCollection<Person>(contacts);
            contactsListView.ItemsSource = _contacts;
        }

        void OnAddContact(object sender, System.EventArgs e)
        {
            var page = new ContactDetailPage(new Person());

            page.ContactAdded += (source, contact) =>
            {
                _contacts.Add(contact);
            };

            Navigation.PushModalAsync(new NavigationPage(page));
        }

        async void OnDeleteContact(object sender, System.EventArgs e)
        {
            var contact = (sender as MenuItem).CommandParameter as Person;

            if (await DisplayAlert("Warning", $"Are you sure you want to delete {contact.name}?", "Yes", "No"))
            {
                _contacts.Remove(contact);

                await _connection.DeleteAsync(contact);
            }
        }
    }
}