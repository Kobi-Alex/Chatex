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
using ServerAssistant;

namespace Chatex
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// Run StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        private readonly int portUser = new Random().Next(1200, 30000);
        private Assistant clientConnection = new Assistant();
        private BinaryReader reader;

        public StartWindow()
        {
            InitializeComponent();
        }
        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                signIn.IsEnabled = false;
                Tuple<string, string> dataRequest = new Tuple<string, string>(loginTextBox.Text, passwordPasswordBox.Password);

                Task.Run(() =>
                {     
                    Request serverRequest = new Request(TypeRequest.authentication, dataRequest);

                    if (serverRequest != null)
                    {
                        try
                        {
                            clientConnection.SignRequestToServer(serverRequest);
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(new Action(() => { MessageBox.Show("Error send file to server " + ex.Message );}));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connect to server  " + ex.Message);            
            }
  
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            clientConnection.ConnectToServer(portUser);

            Task.Run(() =>
            {
                NetworkStream dataStream = clientConnection.Client.GetStream();
                reader = new BinaryReader(dataStream);

                byte[] buffer = new byte[1000];

                while (clientConnection.Client.Connected)
                {
                    int length = reader.ReadInt32();
                    buffer = reader.ReadBytes(length);

                    Request sRequest = Assistant.ByteArrayToObect(buffer);

                    if (sRequest.TypeRequest == TypeRequest.authentication)
                    {
                        //string login = string.Empty;
                        //Dispatcher.Invoke(new Action(() =>
                        //{\
                        //    login = loginTextBox.Text;
                        //}));

                        //string message = (string)sRequest.Data;
                        bool IsDataCorrect = (bool)sRequest.Data;

                        if (IsDataCorrect == true)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                MainWindow mainWindow = new MainWindow(clientConnection);
                                mainWindow.Show();
                                this.Close();
                            }));
                            break;
                        }
                        else
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                signIn.IsEnabled = true;
                                MessageBox.Show("Don't correct Loggin are Password.\nPlease checking!");
                            }));
                        }
                    }
                }
            });
        }
    }
}