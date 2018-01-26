using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EFApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();
            
        }

        async void Button_Clicked(object sender, EventArgs e)
        {
            //check if key pair exists, if it does load it and redirect to main page.
            if (!DependencyService.Get<ICryptoService>().DoesKeyExists())
            {
            await Navigation.PushModalAsync(new SignUpPage());
            }
            else await Navigation.PushModalAsync(new TabsHeader());
        }
    }
}