using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IOTMultiSwitch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer tmr;
        MediaCapture _mediaCapture;

        ObservableCollection<JsonRectangle> listofSwitches = new ObservableCollection<JsonRectangle>();

        public int counter = 0;

        public MainPage()
        {
            this.InitializeComponent();
            //tmr = new DispatcherTimer();
            //tmr.Interval = TimeSpan.FromSeconds(5);
            //tmr.Tick += Tmr_Tick;
           // tmr.Start();
            _mediaCapture = new MediaCapture();

            var str = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (str == "Windows.Desktop")
            {
                //...
            }
            else if (str == "Windows.Mobile")
            {
                txtResponse.FontSize = 70;
            }

            CoreWindow.GetForCurrentThread().KeyDown += MainPage_KeyDown;

            listViewSwitches.ItemsSource = listofSwitches;

        }

        private void MainPage_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            ViewportOperations(args.VirtualKey);
        }

        private void ViewportOperations(Windows.System.VirtualKey args)
        {
            var ttv = rect.TransformToVisual(grid);
            Point screenCoords = ttv.TransformPoint(new Point(0, 0));

            var ttvcanvas = PreviewCanvas.TransformToVisual(grid);
            Point screenCoordscanvas = ttvcanvas.TransformPoint(new Point(0, 0));

            if (args == Windows.System.VirtualKey.Add && rect.ActualWidth + 10 <= previewElement.ActualWidth && rect.ActualHeight <= previewElement.ActualHeight)
            {
                rect.Width = rect.ActualWidth + 10;
                rect.Height = rect.ActualWidth + 10;
            }
            else if (args == Windows.System.VirtualKey.Subtract && rect.Width - 10 >= 0)
            {
                rect.Width = rect.ActualWidth - 10;
                rect.Height = rect.ActualWidth - 10;
            }
            else if (args == Windows.System.VirtualKey.Left && screenCoords.X - 10 >= 0 && screenCoords.X > screenCoordscanvas.X)
            {

                rect.Margin = new Thickness(rect.Margin.Left - 10, rect.Margin.Top, rect.Margin.Right + 10, rect.Margin.Bottom);


            }
            else if (args == Windows.System.VirtualKey.Right && screenCoords.X + 10 <= screenCoordscanvas.X + PreviewCanvas.Width - rect.ActualWidth)
            {
                var pending = (screenCoordscanvas.X + PreviewCanvas.Width - rect.ActualWidth) - (screenCoords.X + 10 + rect.ActualWidth);
                if (pending < 10 && pending >= 0)
                {
                    //edge case 
                    rect.Margin = new Thickness(rect.Margin.Left + pending, rect.Margin.Top, rect.Margin.Right - pending, rect.Margin.Bottom);
                }
                else
                {
                    rect.Margin = new Thickness(rect.Margin.Left + 10, rect.Margin.Top, rect.Margin.Right - 10, rect.Margin.Bottom);
                }

            }
            else if (args == Windows.System.VirtualKey.Up && screenCoords.Y - 10 >= 0 && screenCoords.Y > screenCoordscanvas.Y)
            {
                var pending = screenCoords.Y - 10;
                if (pending < 10)
                {
                    rect.Margin = new Thickness(rect.Margin.Left, rect.Margin.Top - pending, rect.Margin.Right, rect.Margin.Bottom + pending);
                }
                else
                {
                    rect.Margin = new Thickness(rect.Margin.Left, rect.Margin.Top - 10, rect.Margin.Right, rect.Margin.Bottom + 10);
                }

            }
            else if (args == Windows.System.VirtualKey.Down && screenCoords.Y + 10 <= screenCoordscanvas.Y + PreviewCanvas.Height - rect.ActualHeight)
            {
                var pending = (screenCoordscanvas.Y + PreviewCanvas.Height - rect.ActualHeight) - (screenCoords.Y + 10 + rect.ActualHeight);
                if (pending < 10 && pending >= 0)
                {
                    //edge case 
                    rect.Margin = new Thickness(rect.Margin.Left, rect.Margin.Top + pending, rect.Margin.Right, rect.Margin.Bottom - pending);
                }
                else
                {
                    rect.Margin = new Thickness(rect.Margin.Left, rect.Margin.Top + 10, rect.Margin.Right, rect.Margin.Bottom - 10);
                }


            }
        }

        private void MainWindow_KeyDown(object sender, KeyRoutedEventArgs args)
        {
            var ttv = rect.TransformToVisual(previewElement);
            Point screenCoords = ttv.TransformPoint(new Point(0, 0));

            var ttvcanvas = PreviewCanvas.TransformToVisual(grid);
            Point screenCoordscanvas = ttvcanvas.TransformPoint(new Point(0, 0));

            if (args.Key == Windows.System.VirtualKey.Add && rect.ActualWidth<=previewElement.ActualWidth && rect.ActualHeight<=previewElement.ActualHeight)
            {
                rect.Width = rect.ActualWidth + 10;
                rect.Height = rect.ActualWidth + 10;
            }
            else if (args.Key == Windows.System.VirtualKey.Subtract && rect.Width>=0)
            {
                rect.Width = rect.ActualWidth - 10;
                rect.Height = rect.ActualWidth - 10;
            }
            else if (args.Key == Windows.System.VirtualKey.Left && screenCoords.X>=0)
            {
                rect.Margin = new Thickness(rect.Margin.Left - 10, rect.Margin.Top, rect.Margin.Right + 10, rect.Margin.Bottom);
            }
            else if (args.Key == Windows.System.VirtualKey.Right && screenCoords.X<=screenCoordscanvas.X+PreviewCanvas.Width-rect.Width)
            {
                rect.Margin = new Thickness(rect.Margin.Left + 10, rect.Margin.Top, rect.Margin.Right - 10, rect.Margin.Bottom);
            }
            else if (args.Key == Windows.System.VirtualKey.Up && screenCoords.Y>=0)
            {
                rect.Margin = new Thickness(rect.Margin.Left, rect.Margin.Top - 10, rect.Margin.Right, rect.Margin.Bottom + 10);
            }
            else if (args.Key == Windows.System.VirtualKey.Down && screenCoords.Y<= screenCoordscanvas.Y + PreviewCanvas.Height - rect.Height)
            {
                rect.Margin = new Thickness(rect.Margin.Left, rect.Margin.Top + 10, rect.Margin.Right, rect.Margin.Bottom - 10);
            }
        }

        //private async void Tmr_Tick(object sender, object e)
        //{
        //        txtResponse.Text = await CapturePhoto();
        //}
        private async void Tmr_TickTask()
        {
            
                while (true)
                {
                try
                {
                    progress.Visibility = Visibility.Visible;
                    txtResponse.Text = await CapturePhoto();
                    progress.Visibility = Visibility.Collapsed;
                    await Task.Delay(2000);
                }
                catch (Exception)
                {

                }
            }
           
        }

        private async Task<String> CapturePhoto()
        {
            var myPictures = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
            StorageFile file = await myPictures.SaveFolder.CreateFileAsync("photo.jpg", CreationCollisionOption.GenerateUniqueName);


            using (var captureStream = new InMemoryRandomAccessStream())
            {

                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);

                using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var decoder = await BitmapDecoder.CreateAsync(captureStream);
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(fileStream, decoder);

                    var properties = new BitmapPropertySet {
            { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) }
        };
                    await encoder.BitmapProperties.SetPropertiesAsync(properties);

                    await encoder.FlushAsync();
                   // decoder = null;
                   // encoder = null;
                   //await  fileStream.FlushAsync();
                   // fileStream.Dispose();
                   

                }
               //await  captureStream.FlushAsync();
               // captureStream.Dispose();

               // GC.Collect();
            }

            var ttv = rect.TransformToVisual(previewElement);
            Point screenCoords = ttv.TransformPoint(new Point(0, 0));
           // screenCoords.X -= 120;
           // screenCoords.Y -= 120;

            StorageFile croppedFile = await myPictures.SaveFolder.CreateFileAsync("photocrop.jpg", CreationCollisionOption.GenerateUniqueName);
            // var returnedcropImage = await CropBitmap.GetCroppedBitmapAsync(file, screenCoords, rect.RenderSize, 1);
            await CropBitmap.SaveCroppedBitmapAsync(file, croppedFile, screenCoords, rect.RenderSize);

            ///Image bmp = new Image();
            // bmp.Source = returnedcropImage;

            // bmp.sa


            // String ret = await Upload("http://192.168.0.102:8090/api/Switch/UploadFiles", croppedFile);
            //String ret = "";
           await  ObserveObjects(file);

            await file.DeleteAsync();
           //await  croppedFile.DeleteAsync();
            //if(ret.Contains("closed"))
            //{
            //    ret = "Full";
            //}
            //else if(ret.Contains("open"))
            //{
            //    ret = "reserve";
            //}
            //else if (ret.Contains("bird"))
            //{
            //    ret = "empty";
            //}
            return "";
        }

        public async Task ObserveObjects(StorageFile orignalFile)
        {
            foreach(var item in listofSwitches)
            {
                var cropstartPoint = new Point(item.x, item.y);
                var cropSize = new Size(item.x1, item.y1);
                var croppedfile=await  CreateCroppedFile(orignalFile,cropstartPoint,cropSize);
                String ret = await Upload("http://192.168.0.101:8090/api/Switch/UploadFiles", croppedfile);
                if(ret.Contains("open"))
                {
                    item.on = true;
                }
                else 
                {
                    item.on = false;
                }
                await croppedfile.DeleteAsync();

            }
        }

        private async Task<StorageFile> CreateCroppedFile(StorageFile orignalFile,Point corpPoint,Size cropSize)
        {
            var myPictures = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
            StorageFile croppedFile = await myPictures.SaveFolder.CreateFileAsync("photocrop.jpg", CreationCollisionOption.GenerateUniqueName);
            // var returnedcropImage = await CropBitmap.GetCroppedBitmapAsync(file, screenCoords, rect.RenderSize, 1);
            await CropBitmap.SaveCroppedBitmapAsync(orignalFile, croppedFile, corpPoint, cropSize);
            return croppedFile;
        }

        private async Task<string> Upload(string actionUrl, StorageFile fileToUpload)
        {

            try
            {
                var client = new HttpClient();

                MultipartFormDataContent form = new MultipartFormDataContent();
                HttpContent content;
                
                //HttpContent content = new StringContent("fileToUpload");
                //form.Add(content, "fileToUpload");
                var stream = await fileToUpload.OpenStreamForReadAsync();
                content = new StreamContent(stream);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "fileToUpload",
                    FileName = fileToUpload.Name
                };
                form.Add(content);

               
               // var stringContent = new StringContent(JsonConvert.SerializeObject(rectJs));
                //form.Add(stringContent);

                var response = await client.PostAsync(actionUrl, form);
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                return "invalid response";

            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await _mediaCapture.InitializeAsync();

            previewElement.Source = _mediaCapture;
           await _mediaCapture.StartPreviewAsync();
           Tmr_TickTask();
        }
        private void SaveImageUnderRectangle(String file)
        {
           

           
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var btnTapped = sender as Button;
            switch(btnTapped.Name)
            {
                case "btnUp":

                    ViewportOperations(Windows.System.VirtualKey.Up);
                    break;
                case "btnDown":
                    ViewportOperations(Windows.System.VirtualKey.Down);
                    break;
                case "btnLeft":
                    ViewportOperations(Windows.System.VirtualKey.Left);
                    break;
                case "btnRight":
                    ViewportOperations(Windows.System.VirtualKey.Right);
                    break;
                case "btnZoomPlus":
                    ViewportOperations(Windows.System.VirtualKey.Add);
                    break;
                case "btnZoomMinus":
                    ViewportOperations(Windows.System.VirtualKey.Subtract);
                    break;
            }
        }

        private void btnAdd_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var ttv = rect.TransformToVisual(PreviewCanvas);
            Point screenCoords = ttv.TransformPoint(new Point(0, 0));

            listofSwitches.Add(new JsonRectangle{ name = counter.ToString(),x=screenCoords.X,y=screenCoords.Y,x1=rect.RenderSize.Width,y1=rect.RenderSize.Height});
            counter += 1;
        }

        private void btnDel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(listViewSwitches.SelectedItem!=null)
            {
                listofSwitches.Remove(listViewSwitches.SelectedItem as JsonRectangle);
            }
        }
    }
}
