using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace ClientServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread t;


        public MainWindow()
        {

            InitializeComponent();

            t = new Thread(() =>
            {
                using var listener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);

                var ip = IPAddress.Parse("127.0.0.1");
                var port = 27121;

                try
                {
                    EndPoint listenerEP = new IPEndPoint(ip, port);
                    listener.Bind(listenerEP);


                    var buffer = new byte[ushort.MaxValue - 30];

                    EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

                    while (true)
                    {
                        listener.ReceiveFrom(buffer, ref remoteEP);

                        Dispatcher.Invoke(() => 
                        { 
                        Img.Source = LoadImage(buffer);
                        });
                    }
                }
                catch
                {
                    listener.Close();
                }
            });
            t.Start();
        }

        protected virtual void OnExit(System.Windows.ExitEventArgs e)
        {
            t.Abort();
        }


        private void Start_Click(object sender, RoutedEventArgs e)
        {

            using var client = new Socket(
                                     AddressFamily.InterNetwork,
                                     SocketType.Dgram,
                                     ProtocolType.Udp);

            var ip = IPAddress.Parse("127.0.0.1");
            var port = 54678;

            EndPoint remoteEP = new IPEndPoint(ip, port);

            byte[] buffer = null;

            buffer = Encoding.Default.GetBytes("SS");
            client.SendTo(buffer, remoteEP);
            client.Close();
        }



        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
