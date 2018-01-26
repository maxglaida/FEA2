using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EFApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {

        Person p = new Person();
        public SignUpPage()
        {
            InitializeComponent();
            infoText.Text = "This registration will generate a public and private key to encrypt" +
                             "the files you uploaded to the server" +
                             "please keep your private key safe!!";
            infoText.FontSize = 14;
            infoText.HorizontalTextAlignment = TextAlignment.Center;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var b = new UserVerificationPage(p);
            b.UsersVerification();
            await Navigation.PushModalAsync(b);
            
        }

        private void Email_Completed(object sender, EventArgs e)
        {
            var emailTemp = ((Entry)sender).Text;
            if (Regex.Match(emailTemp, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                p.email = emailTemp;                   
            }
            else
            {
               DisplayAlert("False Email","You have entered an invalid email sequence, try again", "OK");             
            }
        }

        private void FullName_Completed(object sender, EventArgs e)
        {
            var nameTemp = ((Entry)sender).Text;
            if(string.IsNullOrWhiteSpace(nameTemp))
            {
                DisplayAlert("invalid Full-Name","you have entered an invalid full name", "OK");
            }
            p.name = nameTemp;
            if (p.name != null && p.name != null) submitBtn.IsEnabled = true;
        }
    }
}