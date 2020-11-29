using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatex.Models
{
    public class StatusDataModel
    {
        public int Id { get; set; } 
        public string ContactName { get; set; }
        public Uri ContactPhoto { get; set; }
        public Uri StatusImage {get;set;}
        
        // if we want to add our status
        public bool IsMeAddStatus {get;set;}


        /// <summary>
        /// To-Do: StatusMessage
        /// </summary>
        //public string StatusMessage { get; set; }
    }
}
