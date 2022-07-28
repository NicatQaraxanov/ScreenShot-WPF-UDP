using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using var listener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);

            var ip = IPAddress.Parse("127.0.0.1");
            var port = 54678;

            EndPoint listenerEP = new IPEndPoint(ip, port);
            listener.Bind(listenerEP);

            var buffer = new byte[ushort.MaxValue - 30];

            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            int len = 0;
            var msg = "";

            while (true)
            {

                len = listener.ReceiveFrom(buffer, ref remoteEP);

                msg = Encoding.Default.GetString(buffer, 0, len);

                if (msg == "SS")
                {
                    Bitmap memoryImage;
                    memoryImage = new Bitmap(500, 450); //Shekil
                    Size s = new Size(memoryImage.Width, memoryImage.Height);
                    Graphics memoryGraphics = Graphics.FromImage(memoryImage);
                    memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);

                    ImageConverter converter = new ImageConverter();
                    var bytes = (byte[])converter.ConvertTo(memoryImage, typeof(byte[]));



                    using var client = new Socket(
                                     AddressFamily.InterNetwork,
                                     SocketType.Dgram,
                                     ProtocolType.Udp);

                    var ipc = IPAddress.Parse("127.0.0.1");
                    var portc = 27121;

                    
                    EndPoint remoteEPc = new IPEndPoint(ipc, portc);
                    client.SendTo(bytes, remoteEPc);
                }

            }
        }
    }
}
