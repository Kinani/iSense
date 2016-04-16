using EmotionAPI;
using EmotionAPI.Contract;
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
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;
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
        private EmotionFacesViewModel emoFacesViewModel;
        CancellationTokenSource wtoken;
        Task task;
        TimeSpan period = TimeSpan.FromSeconds(8);
        public MainPage()
        {
            this.InitializeComponent();

            emoFacesViewModel = new EmotionFacesViewModel();
            this.DataContext = emoFacesViewModel;

            //DataContext = new EmotionFacesViewModel();
        }



        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            //this.DataContext = await emoFacesViewModel.GetEmotionsWithFaces()
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            //await emoFacesViewModel.InitLocalStoreAsync();
            
            

        }

        private async void CaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {



                await CaptureAndAnalysis();


                //this.EmotionsColumnSeries.UpdateLayout();
                //this.UpdateLayout();

                //Debug.WriteLine("Emotions: Detected: {0} face(s), with the folllowing emotions: {1}", emotions.Length, emotions.ToString());

            }
            catch (Exception ex)
            {
                Debug.WriteLine(": {0}", ex.Message);
                throw ex;
            }

        }
        private async Task CaptureAndAnalysis()
        {
            currentIdPhotoFile = await webcam.CapturePhoto();
            var photoStream = await currentIdPhotoFile.OpenAsync(FileAccessMode.ReadWrite);
            this.EmotionsColumnSeries.ItemsSource = null;
            this.EmotionsColumnSeries.ItemsSource = await emoFacesViewModel.GetEmotionsWithFaces(photoStream.AsStream());
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
        void StopWork()
        {
            wtoken.Cancel();

            try
            {
                task.Wait();
            }
            catch (AggregateException) { }
        }
        void StartWork()
        {
            wtoken = new CancellationTokenSource();

            task = Task.Run(async () =>  
            {
                while (true)
                {
                    Dispatcher.RunAsync(CoreDispatcherPriority.High,
                        async () =>
                        {
                            await CaptureAndAnalysis();

                         });
                    
                    await Task.Delay(8000, wtoken.Token); 
                }
            }, wtoken.Token);
        }

        private async void ToggleAutoSense_Toggled(object sender, RoutedEventArgs e)
        {
            if(ToggleAutoSense.IsOn)
            {
                StartWork();

            }
            else
            {
                //ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) =>
                //{

                //    // 
                //    // Update the UI thread by using the UI core dispatcher.
                //    // 
                //    Dispatcher.RunAsync(CoreDispatcherPriority.High,
                //        async () =>
                //        {
                //            await CaptureAndAnalysis();
                //        // 
                //        // UI components can be accessed within this scope.
                //        // 

                //    });

                //}, period);
                StopWork();
            }
        }
    }
}
