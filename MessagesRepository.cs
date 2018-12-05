using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
namespace SignalRRealTimeSQL
{
    public class MessagesRepository
    {
        readonly static string conString = ConfigurationManager.ConnectionStrings["_SDE"].ToString();
        readonly string _connString = ConfigurationManager.ConnectionStrings["_SDE"].ConnectionString;
        public IEnumerable<Messages> GetAllMessages()
        {
            var messages = new List<Messages>();
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"SELECT [Name], 
                [MsgContent] FROM [dbo].[Messages]", connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        messages.Add(new Messages(
                        
                            (string)reader["MessageID"],
                            (string)reader["Message"]
                            //EmptyMessage = reader["EmptyMessage"] != DBNull.Value ? (string)reader["EmptyMessage"] : "",
                            //MessageDate = Convert.ToDateTime(reader["Date"])
                        ));
                    }
                }
            }
            return messages;
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MessagesHub.SendMessages();
            }
        }
    }
    public class Messages
    {
        public string Name { get; set; }
        public string MsgContent { get; set; }
        public Messages(string str1, string str2)
        {
            Name = str1;
            MsgContent = str2;
        }
    }
}