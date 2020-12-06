using Chatex.Commands;
using Chatex.Models;
using ServerAssistant;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatex.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {

        #region MainWindow

        #region Properties
        public int AccountId { get; set; }
        public string ContactName { get; set; }
        public Uri ContactPhoto { get; set; }
        public string LastSeen { get; set; }



        #region Search Chats
        protected string LastSearchText { get; set; }
        protected string mSearchText { get; set; }
        public string SearchText 
        {
            get => mSearchText;
            set 
            {
                if (mSearchText == value) return;

                mSearchText = value;

                if (string.IsNullOrEmpty(SearchText)) Search();
            }
        }
        #endregion

        #endregion

        #region Logics
        //To avoid re searching same text again
        public void Search()
        {
            if ((string.IsNullOrEmpty(LastSearchText) && string.IsNullOrEmpty(SearchText)) || string.Equals(LastSearchText, SearchText))
                return;

            if (string.IsNullOrEmpty(SearchText) || Chats == null || Chats.Count <= 0)
            {
                FilteredChats = new ObservableCollection<ChatLisData>(Chats ?? Enumerable.Empty<ChatLisData>());
                OnPropertyChanged("FilteredChats");

                FilteredPinnedChats = new ObservableCollection<ChatLisData>(PinnedChats ?? Enumerable.Empty<ChatLisData>());
                OnPropertyChanged("FilteredPinnedChats");

                //Update last search text 
                LastSearchText = SearchText;
                return;
            }

            FilteredChats = new ObservableCollection<ChatLisData>(Chats.
                Where(chat => chat.ContactName.ToLower().Contains(SearchText) ||
                chat.Message != null && chat.Message.ToLower().Contains(SearchText)));
            OnPropertyChanged("FilteredChats");


            FilteredPinnedChats = new ObservableCollection<ChatLisData>(PinnedChats.
                Where(pinnedchat => pinnedchat.ContactName.ToLower().Contains(SearchText) ||
                pinnedchat.Message != null && pinnedchat.Message.ToLower().Contains(SearchText)));
            OnPropertyChanged("FilteredPinnedChats");

            //Update last search text 
            LastSearchText = SearchText;

        }
        #endregion

        #region Comands
        /// <summary>
        /// Search Command
        /// </summary>
        /// 
        protected ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get 
            {
                if (_searchCommand == null) 
                    _searchCommand = new CommandViewModel(Search);
                return _searchCommand;
            }

            set 
            {
                _searchCommand = value;
            }
        }
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
                    Id = 7,
                    ContactName ="Mike",
                    ContactPhoto = new Uri("/Assets/Mike.jpg", UriKind.RelativeOrAbsolute),
                    StatusImage = new Uri("/Assets/Tree.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus = false
                },

                new StatusDataModel
                {
                    Id = 9,
                    ContactName ="Billy",
                    ContactPhoto = new Uri("/Assets/brain.jpg", UriKind.RelativeOrAbsolute),
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

        public ObservableCollection<ChatLisData> mChats;
        public ObservableCollection<ChatLisData> mPinnedChats;
        public ObservableCollection<ChatLisData> Chats 
        {
            get => mChats;
            set
            {
                //To change the list
                if (mChats == value) return;

                //To update the list
                mChats = value;

                //Updating filtered chats to match
                FilteredChats = new ObservableCollection<ChatLisData>(mChats);
                OnPropertyChanged("Chats");
                OnPropertyChanged("FilteredChats");

            }
        }

        public ObservableCollection<ChatLisData> PinnedChats
        {
            get => mPinnedChats;
            set
            {
                //To change the list
                if (mPinnedChats == value) return;

                //To update the list
                mPinnedChats = value;

                //Updating filtered pinchats to match
                FilteredPinnedChats = new ObservableCollection<ChatLisData>(mPinnedChats);
                OnPropertyChanged("PinnedChats");
                OnPropertyChanged("FilteredPinnedChats");

            }
        }

        public ObservableCollection<ChatLisData> FilteredChats { get; set; }
        public ObservableCollection<ChatLisData> FilteredPinnedChats { get; set; }

        #endregion

        #region Logics
        void LoadChats()
        {
            Chats = new ObservableCollection<ChatLisData>()
            {


                new ChatLisData
                {
                    AccountId = 8,
                    ContactName = "Stive",
                    ContactPhoto = new Uri("/Assets/tree.jpg", UriKind.RelativeOrAbsolute),
                    Message = "Hello friend!",
                    LastMessageTime="Tue, 14:17 PM",
                    ChatIsSelected = true

                },
                new ChatLisData
                {
                    AccountId = 3,
                    ContactName = "Jenn",
                    ContactPhoto = new Uri("/Assets/why.jpeg", UriKind.RelativeOrAbsolute),
                    Message = "I love you",
                    LastMessageTime="Tue, 08:54 AM"
                },
                new ChatLisData
                {
                    AccountId = 1,
                    ContactName = "John Travolta",
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
                            AccountId = v.AccountId;
                            OnPropertyChanged("AccountId");

                            ContactName = v.ContactName;
                            OnPropertyChanged("ContactName");

                            ContactPhoto = v.ContactPhoto;
                            OnPropertyChanged("ContactPhoto");

                            LoadChatConversation(v);
                        }
                    }));
            }
        }

        //To pin chat om Pin Button Click
        protected ICommand _pinChatCommand;
        public ICommand PinChatCommand
        {
            get
            {
                return _pinChatCommand ??
                    (_pinChatCommand = new RelayCommand(parameter =>
                    {
                        if (parameter is ChatLisData v)
                        {
                            if (!FilteredPinnedChats.Contains(v))
                            {
                                //Add selected chat to pin chat
                                PinnedChats.Add(v);
                                FilteredPinnedChats.Add(v);

                                OnPropertyChanged("PinnedChats");
                                OnPropertyChanged("FilteredPinnedChats");

                                v.ChatIsPinned = true;

                                //Remove selected chat from all chats
                                Chats.Remove(v);
                                FilteredChats.Remove(v);
                                OnPropertyChanged("Chats");
                                OnPropertyChanged("FilteredChats");
                            }
                        }
                    }));
            }
        }

        protected ICommand _unPinChatCommand;
        public ICommand UnPinChatCommand
        {
            get
            {
                return _unPinChatCommand ??
                    (_unPinChatCommand = new RelayCommand(parameter =>
                    {
                        if (parameter is ChatLisData v)
                        {
                            if (!FilteredChats.Contains(v))
                            {
                                //Add selected chat to normal chats list
                                Chats.Add(v);
                                FilteredChats.Add(v);

                                OnPropertyChanged("Chats");
                                OnPropertyChanged("FilteredChats");

                                //Remove selected pinned chats list
                                PinnedChats.Remove(v);
                                FilteredPinnedChats.Remove(v);

                                OnPropertyChanged("PinnedChats");
                                OnPropertyChanged("FilteredPinnedChats");

                                v.ChatIsPinned = false;

                            }

                        }
                    }));
            }
        }


        protected ICommand _sendMessageCommand;
        public ICommand SendMessageCommand
        {
            get
            {
                return _sendMessageCommand ??
                    (_sendMessageCommand = new RelayCommand(parameter =>
                    {
                        ChatLisData v = new ChatLisData()
                        {
                            AccountId = this.AccountId,
                        };

                        LoadChatConversation(v);

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

        public bool ChatIsPinned { get; private set; }


        #region Logics
        public void LoadChatConversation(ChatLisData chat)
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            if (Conversations == null)
                Conversations = new ObservableCollection<ChatConversation>();

            Conversations.Clear();

            using (SqlCommand com = new SqlCommand("select* From Conversation where AccountID = @AccountId", connection))
            {
                com.Parameters.AddWithValue("@AccountId", chat.AccountId);
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
                            IsMessageReceived = string.IsNullOrEmpty(reader["ReceivedMsgs"].ToString()) ? false : true

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
            PinnedChats = new ObservableCollection<ChatLisData>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        }
    }
}
