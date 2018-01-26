using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EFApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserVerificationPage : ContentPage
    {
        public Person User { get; set; }
        public UserVerificationPage(Person user)
        {
            InitializeComponent();
            User = user;
        }

        async void StartTimer()
        {
            await Task.Delay(3000);
        }

        private async Task<bool> UserVerification()
        {
            // check if email and name exist on server
            bool i = await SendRecieveService.CheckUserExistsAsync(User);
            if (!i)
            {
                DependencyService.Get<ICryptoService>().GenerateKeys();
                var publicKeyJson = DependencyService.Get<ICryptoService>().ExportPublicKeyToString(User);
                return await SendRecieveService.SendPOST(publicKeyJson, User);
            }
            
            return false;
        }

        async void FailedVerification()
        {
           
            verificationResponse.Text = "FAILED!";
            verificationResponse.TextColor = Color.Red;
            StartTimer();
            await Navigation.PushModalAsync(new SignUpPage());

        }

        async void SuccesVerification()
        {
            StartTimer();
            verificationResponse.Text = "SUCCESS!";
            verificationResponse.TextColor = Color.Green;
            StartTimer();
            await Navigation.PushModalAsync(new TabsHeader());
        }

        public async void UsersVerification()
        {
            bool i = await UserVerification();
            if (!i)
            {
                FailedVerification();
            }
            else
            {
                SuccesVerification();
            }
        }

    }
}