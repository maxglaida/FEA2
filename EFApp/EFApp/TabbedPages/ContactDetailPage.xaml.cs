using EFApp.Presistance;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EFApp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactDetailPage : ContentPage
    {
        public event EventHandler<Person> ContactAdded;
        private SQLiteAsyncConnection _connection;
        public Person NewContact { get; set; }

        public ContactDetailPage(Person contact)
        {
            if (contact == null)
                throw new ArgumentNullException(nameof(contact));

            InitializeComponent();

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

        }

        async void OnSave(object sender, System.EventArgs e)
        {
            
            await _connection.InsertOrReplaceAsync(NewContact);
            ContactAdded?.Invoke(this, NewContact);
            await Navigation.PopModalAsync();
        }

        private async void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            string contact = await SendRecieveService.SearchForUserOnServer(SearchBar.Text);
            if (string.IsNullOrEmpty(contact))
            {
                //write to the user that this specific user does not exist.
                BindingContext = new Person
                {
                    email = "",
                    name = "No user found!"
                };
                return;
            }

            NewContact = JsonConvert.DeserializeObject<Person>(contact);

            BindingContext = NewContact;

            if (!string.IsNullOrWhiteSpace(NewContact.public_key)) btn.IsEnabled = true; 

        }



    }
}