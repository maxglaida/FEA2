using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EFApp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendFileDetailPage : ContentPage
    {
        private Person _contact;
        private Stream _file;
        private string _fileName;
        private string _fileExtension;
        public SendFileDetailPage(Person contact)
        {
            _contact = contact;
            InitializeComponent();
           ContactName.Text = "Reciever: " + _contact.name;
           ContactEmail.Text = "Recievers Email: " + _contact.email;
        }

        async Task btn_ClickedAsync(object sender, EventArgs e)
        {
            
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                try
                {
                using (var photo = await CrossMedia.Current.PickPhotoAsync())
                    {
                        int idx = photo.Path.LastIndexOf('/');
                        _fileName = photo.Path.Substring(idx+1);
                        fileName.Text = "Selected File: " + _fileName;
                        _fileExtension = photo.Path.Substring(_fileName.LastIndexOf('.'));
                        _file = photo.GetStream();
                    }
                }
                catch (Exception err)
                {
                    await DisplayAlert("Alert", err.ToString(), "OK");
                    throw err;
                }
                
            }
            if (_file != null) sendBtn.IsEnabled = true;
        }

        private async void sendBtn_Clicked(object sender, EventArgs e)
        {
            
            var file = DependencyService.Get<ICryptoService>().EncryptFile(_file, _contact, _fileExtension, _fileName);
            try
            {
                await SendRecieveService.SendEncryptedFileTo(file, _contact, _fileName);
            }
            catch (Exception err)
            {
                await DisplayAlert("Error", err.ToString(), "OK");
            }
            
        }
    }
}