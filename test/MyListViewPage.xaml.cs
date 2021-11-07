using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Plugin.FilePicker;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyListViewPage : ContentPage
    {

        public MyListViewPage()
        {
            InitializeComponent();
        }

        private async void PickPhotoButtonOnClicked(object sender, EventArgs e)
        {
            // initialize
            await CrossMedia.Current.Initialize();

            // pick photo
            //var mediaFile = await CrossMedia.Current.PickPhotoAsync(
            //    new PickMediaOptions { PhotoSize = PhotoSize.MaxWidthHeight, CompressionQuality = 10 });

            System.Diagnostics.Debug.WriteLine("Choosing Video...");

            MediaFile mediaFile = await CrossMedia.Current.PickVideoAsync();
            
            System.Diagnostics.Debug.WriteLine("Chosen Video...", mediaFile);

            if (mediaFile == null)
                return;

            await DependencyService.Get<IVideoConverter>().SaveVideo(mediaFile.GetStream());
            await DependencyService.Get<IVideoConverter>().CompressVideo(mediaFile, 1);
            await DependencyService.Get<IVideoConverter>().CompressVideo(mediaFile, 2);

            // show image
            PickedImage.Source = ImageSource.FromStream(() => mediaFile.GetStream());
        }

        private async void TakePhotoButtonOnClicked(object sender, EventArgs e)
        {
            // initialize
            await CrossMedia.Current.Initialize();

            // check if camera is available
            if (!CrossMedia.Current.IsCameraAvailable ||
                        !CrossMedia.Current.IsTakePhotoSupported)
                return;

            // take photo
            var mediaFile = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                    Directory = "XFPickAndTakePhoto",
                    Name = $"{Guid.NewGuid()}.jpg",
                    SaveToAlbum = true,
                    SaveMetaData = true
                });
            if (mediaFile == null)
                return;

            // show image
            PickedImage.Source = ImageSource.FromStream(() => mediaFile.GetStream());
        }
    }
}

