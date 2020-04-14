using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace FaceRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string subscriptionKey = "b997a2201bb749e0b680c37f7f0b616c";


        private const string faceEndpoint = "https://facerecogition.cognitiveservices.azure.com/";

        private readonly IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });

        IList<DetectedFace> faceList;
        String[] faceDescriptions;
        double resizeFactor;
      
        public MainWindow()
        {
            InitializeComponent();

            if (Uri.IsWellFormedUriString(faceEndpoint, UriKind.Absolute))
            {
                faceClient.Endpoint = faceEndpoint;
            }
            else
            {
                MessageBox.Show(faceEndpoint,
                    "Invalid URI", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }
        private void FacePhoto_MouseMove(object sender, MouseEventArgs e)
        { }
        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "JPG (*.jpg)|*.jpg";
            bool? result = dialog.ShowDialog(this);

            if (!(bool)result)
            {
                return;
            }

            string filePath = dialog.FileName;
            Uri uriSource = new Uri(filePath);
            BitmapImage source = new BitmapImage();
          //  Image Image1;
            source.BeginInit();
            source.CacheOption = BitmapCacheOption.None;
            source.UriSource = uriSource;
            source.EndInit();

            FacePhoto.Source = source;

            faceList = await UploadAndDetectFaces(filePath);

            if (faceList.Count > 0)
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext context = visual.RenderOpen();
                context.DrawImage(source, new Rect(0, 0, source.Width, source.Height));
                double dpi = source.DpiX;
                resizeFactor = (dpi > 0) ? 96 / dpi : 1;
                faceDescriptions = new String[faceList.Count];

                for (int i = 0; i < faceList.Count; ++i)
                {
                    DetectedFace face = faceList[i];

                    context.DrawRectangle(
                        Brushes.Transparent,
                        new Pen(Brushes.Green, 5),
                        new Rect(
                            face.FaceRectangle.Left * resizeFactor,
                            face.FaceRectangle.Top * resizeFactor,
                            face.FaceRectangle.Width * resizeFactor,
                            face.FaceRectangle.Height * resizeFactor
                            )
                    );
                } 
                context.Close();

                RenderTargetBitmap facewithRectangle = new RenderTargetBitmap(
                    (int)(source.PixelWidth * resizeFactor),
                    (int)(source.PixelHeight * resizeFactor),
                    96,
                    96,
                    PixelFormats.Default);

                facewithRectangle.Render(visual);
                FacePhoto.Source = facewithRectangle;
            }
        }

        private async Task<IList<DetectedFace>> UploadAndDetectFaces(string imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    IList<DetectedFace> faceList =
                        await faceClient.Face.DetectWithStreamAsync(
                            imageFileStream, true, false, null);
                    return faceList;
                }
            }
            catch (APIErrorException f)
            {
                MessageBox.Show(f.Message);
                return new List<DetectedFace>();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
                return new List<DetectedFace>();
            }
        }
    }

//}
}
