using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace MVC_Demo.Models.Hubs
{
    [HubName("messageHub")]
    public class MessageHubs : Hub
    {
        private static string conString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ToString();
        public void Hello()
        {
            Clients.All.hello();
        }

        [HubMethodName("sendMessages")]
        public static void SendMessages()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MessageHubs>();
            context.Clients.All.updateMessages();
        }

    }
}
    
