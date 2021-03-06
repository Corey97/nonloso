using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using System.Collections.Generic;

namespace Plus.HabboHotel.Rooms
{
    /// <summary>
    /// Class TradeUser.
    /// </summary>
    internal class TradeUser
    {
        /// <summary>
        /// The user identifier
        /// </summary>
        internal uint UserId;

        /// <summary>
        /// The offered items
        /// </summary>
        internal List<UserItem> OfferedItems;

        /// <summary>
        /// The _room identifier
        /// </summary>
        private readonly uint _roomId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeUser"/> class.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roomId">The room identifier.</param>
        internal TradeUser(uint userId, uint roomId)
        {
            this.UserId = userId;
            this._roomId = roomId;
            this.HasAccepted = false;
            this.OfferedItems = new List<UserItem>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has accepted.
        /// </summary>
        /// <value><c>true</c> if this instance has accepted; otherwise, <c>false</c>.</value>
        internal bool HasAccepted { get; set; }

        /// <summary>
        /// Gets the room user.
        /// </summary>
        /// <returns>RoomUser.</returns>
        internal RoomUser GetRoomUser()
        {
            Room room = Plus.GetGame().GetRoomManager().GetRoom(this._roomId);
            if (room == null)
            {
                return null;
            }
            return room.GetRoomUserManager().GetRoomUserByHabbo(this.UserId);
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns>GameClient.</returns>
        internal GameClient GetClient()
        {
            return Plus.GetGame().GetClientManager().GetClientByUserId(this.UserId);
        }
    }
}