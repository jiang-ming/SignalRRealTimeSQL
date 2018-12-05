using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using System.Web.Script.Services;
using System.Data;
using System.IO;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
namespace SignalRRealTimeSQL
{
    /// <summary>
    /// Summary description for MiscUtil
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class MiscUtil : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string HelloWorld()
        {
            return "Hello World";
        }


        readonly static string conString = ConfigurationManager.ConnectionStrings["_SDE"].ToString();
        readonly string _connString = ConfigurationManager.ConnectionStrings["_SDE"].ConnectionString;
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAllMessages()
        {
            var names = "";
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
                        names += (string)reader["Name"];
                        messages.Add(new Messages(

                            (string)reader["Name"],
                            (string)reader["MsgContent"]
                            //EmptyMessage = reader["EmptyMessage"] != DBNull.Value ? (string)reader["EmptyMessage"] : "",
                            //MessageDate = Convert.ToDateTime(reader["Date"])
                        ));
                    }
                }
            }
            return names;
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MessagesHub.SendMessages();
            }
        }
    }
}
