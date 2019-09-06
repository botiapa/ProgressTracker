using System;
using Newtonsoft;

namespace APIProgressTracker
{
    public abstract class ProgressTrackerAPI
    {
        private const string SERVER = "www.remotemysql.com";
        private const string DATABASE = "progresstracker";
        private const string UID = "TMrArME2SE";
        private const string PASSWORD = "kJTKB2snGQ";

        ProgressTrackerAPI()
        {

        }

        public void Initialize()
        {
            //TODO: Connect

        }
    
        #region Actual API calls
        public void CreateNewMessage(Message message)
        {
            // var asd = Json.Encode(message);
            // messagesTable.add(ID, asd);
        }
        #endregion
    }
}
