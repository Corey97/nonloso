using Plus.HabboHotel.GameClients;
using System;
using System.Collections.Generic;

namespace Plus.HabboHotel.Guides
{
    /// <summary>
    /// Class GuideManager.
    /// </summary>
    internal class GuideManager
    {
        /// <summary>
        /// The en cours
        /// </summary>
        public Dictionary<uint, GameClient> EnCours = new Dictionary<uint, GameClient>();

        //internal int HelpersCount = 0;
        //internal int GuardiansCount = 0;
        /// <summary>
        /// The guides on duty
        /// </summary>
        internal List<GameClient> GuidesOnDuty = new List<GameClient>();

        /// <summary>
        /// Gets or sets the guides count.
        /// </summary>
        /// <value>The guides count.</value>
        public int GuidesCount
        {
            get
            {
                return this.GuidesOnDuty.Count;
            }
            set
            {
            }
        }

        /*public int Helpers
        {
        get { return HelpersCount; }
        set { HelpersCount = value; }
        }
        public int Guardians
        {
        get { return GuardiansCount; }
        set { GuardiansCount = value; }
        }*/

        /// <summary>
        /// Gets the random guide.
        /// </summary>
        /// <returns>GameClient.</returns>
        public GameClient GetRandomGuide()
        {
            var random = new Random();
            return this.GuidesOnDuty[random.Next(0, this.GuidesCount - 1)];
        }

        /// <summary>
        /// Adds the guide.
        /// </summary>
        /// <param name="guide">The guide.</param>
        public void AddGuide(GameClient guide)
        {
            if (!this.GuidesOnDuty.Contains(guide))
            {
                this.GuidesOnDuty.Add(guide);
            }
        }

        /// <summary>
        /// Removes the guide.
        /// </summary>
        /// <param name="guide">The guide.</param>
        public void RemoveGuide(GameClient guide)
        {
            if (this.GuidesOnDuty.Contains(guide))
            {
                this.GuidesOnDuty.Remove(guide);
            }
        }
    }
}