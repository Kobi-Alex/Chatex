using RequestsDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

                if (UserAuthentication())
                {
                    MainWindow mainWindow = new MainWindow(client);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Don't correct Loggin are Password.\nPlease checking!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("System error!! " + ex.Message);
            }
        }

       
        private bool UserAuthentication()
        {

            string login = loginTextBox.Text;
            string password = passwordPasswordBox.Password;

            ServerRequest serverRequest = new ServerRequest();

            return serverRequest.ClientAuthentication(new ServerRequest(login, password, client, TypeRequest.authentication));
        }     
    }
}
