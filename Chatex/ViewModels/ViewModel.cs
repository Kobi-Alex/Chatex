using Chatex.Commands;
using Chatex.CustomerControls;
using Chatex.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatex.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {

        #region MainWindow

        #region Properties
        public string ContactName { get; set; }
        public Uri ContactPhoto { get; set; }
        public string LastSeen { get; set; }
        #endregion

        #endregion

        #region Status Thumbs

        #region  Properties
        public ObservableCollection<StatusDataModel> statusThumbsCollection { get; set; }
        #endregion

        #region Logics
        void LoadStatusThumbs()
        {
            statusThumbsCollection = new ObservableCollection<StatusDataModel>()
            {
                new StatusDataModel
                {
                    IsMeAddStatus = true,
                },

                new StatusDataModel
                {
                    ContactName ="Mike",
                    ContactPhoto = new Uri("/Assets/Mike.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Tree.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },

                new StatusDataModel
                {
                    ContactName ="Jenn",
                    ContactPhoto = new Uri("/Assets/Lopez.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Batterfly.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                }
            };

            OnPropertyChanged("statusThumbsCollection");

        }
        #endregion
        #endregion

        #region ChatList

        #region Properties
        public ObservableCollection<ChatLisData> Chats { get; set; }
        #endregion

        #region Logics
        void LoadChats()
        {
            Chats = new ObservableCollection<ChatLisData>()
            {
                new ChatLisData
                {
                    ContactName = "Stive",
                    ContactPhoto = new Uri("/Assets/tree.jpg", UriKind.RelativeOrAbsolute),
                    Message = "Hello friend!",
                    LastMessageTime="Tue, 14:17 PM",
                    ChatIsSelected = true

                },
                new ChatLisData
                {
                    ContactName = "Jenn",
                    ContactPhoto = new Uri("/Assets/why.jpeg", UriKind.RelativeOrAbsolute),
                    Message = "I love you",
                    LastMessageTime="Tue, 08:54 AM"
                },
                new ChatLisData
                {
                    ContactName = "Travolta",
                    ContactPhoto = new Uri("/Assets/dracaena.jpg", UriKind.RelativeOrAbsolute),
                    Message = "I'm sure",
                    LastMessageTime="Tue, 19:43 PM"
                }
            };

            OnPropertyChanged();
        }
        #endregion

        #region Commands
        protected ICommand _getSelectedChatCommand;
        public ICommand GetSelectedChatCommand 
        {
            get
            {
                return _getSelectedChatCommand ??
                    (_getSelectedChatCommand = new RelayCommand(parameter =>
                    {
                         if (parameter is ChatLisData v)
                         {
                             ContactName = v.ContactName;
                             OnPropertyChanged("ContactName");

                             ContactPhoto = v.ContactPhoto;
                             OnPropertyChanged("ContactPhoto");
                         }
                    }));
            }
        }
        #endregion

        #endregion


        #region Conversations

        #region Properties
        protected ObservableCollection<ChatConversation> mConversations;
        public ObservableCollection<ChatConversation> Conversations
        {
            get => mConversations;
            set 
            { 
                mConversations = value;
                OnPropertyChanged();
            }
        }
        #endregion

        protected string messageText;
        public string MessageText
        {
            get => messageText;
            set 
            {
                messageText = value;
                OnPropertyChanged("messageText");
            }
        }


        #region Logics
        void LoadChatConversation()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            if (Conversations == null)
                Conversations = new ObservableCollection<ChatConversation>();
            using(SqlCommand com = new SqlCommand("select* From Conversation where AccountID = 1", connection))
            {
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string MsgReceivedOn = !string.IsNullOrEmpty(reader["MsgReseivedOn"].ToString()) ?
                            Convert.ToDateTime(reader["MsgReseivedOn"].ToString()).ToString("MMM dd, hh:mm tt") : "";

                        string MsgSentOn = !string.IsNullOrEmpty(reader["MsgSentOn"].ToString()) ?
                           Convert.ToDateTime(reader["MsgSentOn"].ToString()).ToString("MMM dd, hh:mm tt") : "";

                        var conversation = new ChatConversation()
                        {
                            ContactName = reader["AccountID"].ToString(),
                            ReceivedMessage = reader["ReceivedMsgs"].ToString(),
                            MsgReceivedOn = MsgReceivedOn,
                            SentMessage = reader["SentMsg"].ToString(),
                            MsgSentOn = MsgSentOn,
                            IsMessageReceived = string.IsNullOrEmpty(reader["ReceivedMsgs"].ToString())?false:true

                        };
                        Conversations.Add(conversation);
                        OnPropertyChanged("Conversations");
                    }
                }
            }

        }


        #endregion

        #endregion


        SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-5DLJ44R;Initial Catalog=ChatexDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        public ViewModel()
        {
            LoadStatusThumbs();
            LoadChats();
            LoadChatConversation();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        }
    }
}
