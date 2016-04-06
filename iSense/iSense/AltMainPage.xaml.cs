using FaceAPI;
using FaceAPI.Contract;
using iSense.Helpers;
using iSense.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace iSense
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private WebcamHelper webcam;
        private StorageFile currentIdPhotoFile;

        public MainPage()
        {
            this.InitializeComponent();
            //DataContext = new EmotionFacesViewModel();
        }

        private async void CaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            currentIdPhotoFile = await webcam.CapturePhoto();
            var photoStream = await currentIdPhotoFile.OpenAsync(FileAccessMode.ReadWrite);
            var faceServiceClient = new FaceServiceClient(GeneralConstants.OxfordAPIKey);
            Face[] faces = await faceServiceClient.DetectAsync(photoStream.AsStream(), false, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Smile, FaceAttributeType.Glasses, FaceAttributeType.Age });
            Debug.WriteLine("Response: eshta. Detected: {0} face(s) in {1} other info {2}", faces.Length, currentIdPhotoFile.Name , faces.First<Face>().FaceAttributes.Age);
            photoStream.Dispose();
        }

        private async void WebcamFeed_Loaded(object sender, RoutedEventArgs e)
        {
            if (webcam == null || !webcam.IsInitialized())
            {
                // Initialize Webcam Helper
                webcam = new WebcamHelper();
                await webcam.InitializeCameraAsync();

                // Set source of WebcamFeed on MainPage.xaml
                WebcamFeed.Source = webcam.mediaCapture;

                // Check to make sure MediaCapture isn't null before attempting to start preview. Will be null if no camera is attached.
                if (WebcamFeed.Source != null)
                {
                    // Start the live feed
                    await webcam.StartCameraPreview();
                }
            }
            else if (webcam.IsInitialized())
            {
                WebcamFeed.Source = webcam.mediaCapture;

                // Check to make sure MediaCapture isn't null before attempting to start preview. Will be null if no camera is attached.
                if (WebcamFeed.Source != null)
                {
                    await webcam.StartCameraPreview();
                }
            }
        }
        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await webcam.StopCameraPreview();
        }
    }
}
