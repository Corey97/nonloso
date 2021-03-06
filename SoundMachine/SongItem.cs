#region

using Plus.Database.Manager.Database.Session_Details.Interfaces;
using Plus.HabboHotel.Items;

#endregion

namespace Plus.HabboHotel.SoundMachine
{
    /// <summary>
    /// Class SongItem.
    /// </summary>
    internal class SongItem
    {
        /// <summary>
        /// The item identifier
        /// </summary>
        internal uint ItemId;

        /// <summary>
        /// The song identifier
        /// </summary>
        internal uint SongId;

        /// <summary>
        /// The base item
        /// </summary>
        internal Item BaseItem;

        /// <summary>
        /// The extra data
        /// </summary>
        internal string ExtraData;

        /// <summary>
        /// The song code
        /// </summary>
        internal string SongCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="SongItem"/> class.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="songId">The song identifier.</param>
        /// <param name="baseItem">The base item.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="songCode">The song code.</param>
        public SongItem(uint itemId, uint songId, int baseItem, string extraData, string songCode)
        {
            ItemId = itemId;
            SongId = songId;
            BaseItem = Plus.GetGame().GetItemManager().GetItem(((uint)baseItem));
            ExtraData = extraData;
            SongCode = songCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SongItem"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public SongItem(UserItem item)
        {
            ItemId = item.Id;
            SongId = SongManager.GetSongId(item.SongCode);
            BaseItem = item.BaseItem;
        }

        /// <summary>
        /// Saves to database.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        internal void SaveToDatabase(uint roomId)
        {
            using (IQueryAdapter queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.RunFastQuery(string.Concat("REPLACE INTO items_songs VALUES (", ItemId, ",", roomId, ",", SongId, ")"));
            }
        }

        /// <summary>
        /// Removes from database.
        /// </summary>
        internal void RemoveFromDatabase()
        {
            using (IQueryAdapter queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.RunFastQuery(string.Format("DELETE FROM items_songs WHERE itemid = {0}", ItemId));
            }
        }
    }
}