using Chatex.CustomerControls;
using Chatex.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chatex.ViewModels
{
    public class ViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<StatusDataModel> statusThumbsCollection { get; set; }
        public ViewModel()
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

           // OnPropertyChanged("statusThumbsCollection");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        }
    }
}
