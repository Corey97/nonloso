#region

using Plus.Database.Manager.Session_Details.Interfaces;
using System.Collections.Generic;
using System.Data;

#endregion

namespace Plus.Configuration
{
    /// <summary>
    /// Class ConfigData.
    /// </summary>
    internal class ConfigData
    {
        /// <summary>
        /// The database data
        /// </summary>
        internal Dictionary<string, string> DbData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigData"/> class.
        /// </summary>
        /// <param name="dbClient">The database client.</param>
        internal ConfigData(IRegularQueryAdapter dbClient)
        {
            try
            {
                DbData = new Dictionary<string, string>();
                DbData.Clear();
                dbClient.SetQuery("SELECT * FROM `server_settings`");
                var table = dbClient.GetTable();
                foreach (DataRow dataRow in table.Rows) DbData.Add(dataRow[0].ToString(), dataRow[1].ToString());
            }
            catch
            {
            }
        }
    }
}