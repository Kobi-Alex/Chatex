using Chatex.Commands;
using ChatModelsDLL;
using ServerAssistant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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

namespace Chatex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Assistant assistant;
        private BinaryWriter writer;
        private BinaryReader reader;
        private ViewModels.ViewModel viewModel = new ViewModels.ViewModel();


        public MainWindow(Assistant clientConnection)
        {
            InitializeComponent();
            this.assistant = clientConnection;  
        }


        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            // Listenet server
            Task.Run(() =>
            {
                try
                {
                    NetworkStream dataStream = assistant.Client.GetStream();
                    writer = new BinaryWriter(dataStream);
                    reader = new BinaryReader(dataStream);

                    byte[] buffer = new byte[1000];

                    while (assistant.Client.Connected)
                    {
                        int length = reader.ReadInt32();
                        buffer = reader.ReadBytes(length);

                        Request sRequest = Assistant.ByteArrayToObect(buffer);

                        switch (sRequest.TypeRequest)
                        {
                            case TypeRequest.text:
                                {
                                    //string message = (string)sRequest.Data;

                                    //this.listMsg.Dispatcher.Invoke(new Action(() =>
                                    //{
                                    //    UserControlMessageReceive mr = new UserControlMessageReceive(message);

                                    //    ListViewItem listViewItem = new ListViewItem();
                                    //    listViewItem.Content = mr;
                                    //    listViewItem.HorizontalAlignment = HorizontalAlignment.Left;
                                    //    this.listMsg.Items.Add(listViewItem);

                                    //}));
                                }
                                break;
                            case TypeRequest.image:
                                break;
                            case TypeRequest.sound:
                                break;
                            case TypeRequest.loadData:
                                {
                                    List<Account> data = (List<Account>)sRequest.Data;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            });


            //LoadData from server

            Request serverRequest = new Request(TypeRequest.loadData);

            if (serverRequest != null)
            {
                try
                {
                    assistant.SignRequestToServer(serverRequest);
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() => { MessageBox.Show("Error send file to server " + ex.Message);}));
                }
            }
        }

        TypeRequest typeRequest;
        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            object data = null;

            switch (typeRequest)
            {
                case TypeRequest.text:
                    {
                        data = messageBox.Text;
                        messageBox.Text = null;
                    }
                    break;
                case TypeRequest.image:
                    break;
                case TypeRequest.sound:
                    break;
                case TypeRequest.logOut:
                    break;
                default:
                    break;
            }

            Task.Run(() =>
            {
                string receiver = assistant.Client.Client.LocalEndPoint.ToString();
                Request serverRequest = new Request(receiver, typeRequest, data);

                if (serverRequest != null)
                {
                    try
                    {
                        assistant.SignRequestToServer(serverRequest);
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
        private void messageBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            typeRequest = TypeRequest.text;
        }
        private void InsertPhoto_Click(object sender, RoutedEventArgs e)
        {
            typeRequest = TypeRequest.image;
        }
        private void InsertAudioFile_Click(object sender, RoutedEventArgs e)
        {
            typeRequest = TypeRequest.sound;
        }


        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void buttonMaximaze_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else WindowState = WindowState.Normal;
        }
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void OpenContactInfoWindow_Click(object sender, RoutedEventArgs e)
        {
            ContactInfoScreen.Visibility = Visibility.Visible;
        }

    }
}
