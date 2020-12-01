using RequestsDLL;
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
using System.Windows.Shapes;

namespace Chatex
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// Run StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        private readonly int portServer = 1024;
        private int portUser = new Random().Next(1200, 30000);

        private TcpClient client;

        private BinaryWriter writer;
        private BinaryReader reader;

        public StartWindow()
        {
            InitializeComponent();
        }

        private void signIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //connect to server   


                IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portUser);
                client = new TcpClient(clientEP);

                IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portServer);
                client.Connect(serverEP);

                // serverRequest.GetReplyFromServer(client);

                string login = loginTextBox.Text;
                string password = passwordPasswordBox.Password;

                Task.Run(() =>
                {
                    NetworkStream dataStream = client.GetStream();
                    writer = new BinaryWriter(dataStream);
                    reader = new BinaryReader(dataStream);

                    byte[] buffer = new byte[1000];

                    while (client.Connected)
                    {
                        int length = reader.ReadInt32();
                        buffer = reader.ReadBytes(length);

                        ServerRequest sRequest = ServerRequest.ByteArrayToObect(buffer);

                        if (sRequest.TypeRequest == TypeRequest.authentication)
                        {
                            string message = (string)sRequest.Data;

                            if (message == login)
                            {
                                MainWindow mainWindow = new MainWindow(client);
                                mainWindow.Show();
                                this.Close();
                            }
                            else
                            {
                                Dispatcher.Invoke(new Action(() =>
                                  {
                                      MessageBox.Show("Don't correct Loggin are Password.\nPlease checking!");
                                  }));
                            }
                        }
                    }
                });


                ServerRequest serverRequest = new ServerRequest(portUser, TypeRequest.authentication, password);

                Task.Run(() =>
                {
                    if (serverRequest != null)
                    {
                        try
                        {
                            byte[] objectArr = ServerRequest.ObjectToByteArray(serverRequest);
                            writer.Write(objectArr.Length);
                            writer.Write(objectArr);
                            writer.Flush();
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                MessageBox.Show("Error send file to server " + ex.Message);
                            }));
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {

                    MessageBox.Show("Error send file to server  " + ex.Message);

                }));
            }
        }


        private bool UserAuthentication(string ms)
        {

            string login = loginTextBox.Text;
            string password = passwordPasswordBox.Password;

            //ServerRequest sRequest = new ServerRequest(portUser, TypeRequest.authentication, password);
            //serverRequest.SendRequestToServer(sRequest);



            if (login == ms) { return true; }
            return false;
        }

    }
}