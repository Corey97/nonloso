namespace Plus.HabboHotel.Rooms
{
    /// <summary>
    /// Class RoomEvent.
    /// </summary>
    internal class RoomEvent
    {
        /// <summary>
        /// The name
        /// </summary>
        internal string Name;

        /// <summary>
        /// The description
        /// </summary>
        internal string Description;

        /// <summary>
        /// The time
        /// </summary>
        internal int Time;

        /// <summary>
        /// The room identifier
        /// </summary>
        internal uint RoomId;

        /// <summary>
        /// The category
        /// </summary>
        internal int Category;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomEvent"/> class.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="time">The time.</param>
        /// <param name="category">The category.</param>
        internal RoomEvent(uint roomId, string name, string description, int time = 0, int category = 1)
        {
            this.RoomId = roomId;
            this.Name = name;
            this.Description = description;
            this.Time = ((time == 0) ? (Plus.GetUnixTimeStamp() + 7200) : time);

            this.Category = category;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has expired.
        /// </summary>
        /// <value><c>true</c> if this instance has expired; otherwise, <c>false</c>.</value>
        internal bool HasExpired
        {
            get
            {
                return Plus.GetUnixTimeStamp() > this.Time;
            }
        }
    }
}