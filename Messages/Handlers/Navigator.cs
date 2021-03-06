﻿using Plus.HabboHotel.Navigators;
using Plus.HabboHotel.Rooms;
using Plus.Messages.Parsers;

namespace Plus.Messages.Handlers
{
    /// <summary>
    /// Class GameClientMessageHandler.
    /// </summary>
    internal partial class GameClientMessageHandler
    {
        /// <summary>
        /// Gets the flat cats.
        /// </summary>
        internal void GetFlatCats()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializeFlatCategories(Session));
        }

        /// <summary>
        /// Enters the inquired room.
        /// </summary>
        internal void EnterInquiredRoom()
        {
        }

        /// <summary>
        /// Gets the pub.
        /// </summary>
        internal void GetPub()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            uint roomId = Request.GetUInteger();
            RoomData roomData = Plus.GetGame().GetRoomManager().GenerateRoomData(roomId);
            if (roomData == null)
                return;
            GetResponse().Init(LibraryParser.OutgoingRequest("453"));
            GetResponse().AppendInteger(roomData.Id);
            GetResponse().AppendString(roomData.CCTs);
            GetResponse().AppendInteger(roomData.Id);
            SendResponse();
        }

        /// <summary>
        /// Opens the pub.
        /// </summary>
        internal void OpenPub()
        {
            Request.GetInteger();
            uint roomId = Request.GetUInteger();
            Request.GetInteger();
            RoomData roomData = Plus.GetGame().GetRoomManager().GenerateRoomData(roomId);
            if (roomData == null)
                return;
            PrepareRoomForUser(roomData.Id, "");
        }

        /// <summary>
        /// News the navigator.
        /// </summary>
        internal void NewNavigator()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            if (Session == null)
                return;
            Plus.GetGame().GetNavigator().EnableNewNavigator(Session);
        }

        /// <summary>
        /// Searches the new navigator.
        /// </summary>
        internal void SearchNewNavigator()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session == null)
                return;
            string name = Request.GetString();
            string junk = Request.GetString();
            Session.SendMessage(Plus.GetGame().GetNavigator().SerlializeNewNavigator(name, junk, Session));
        }

        /// <summary>
        /// Saveds the search.
        /// </summary>
        internal void SavedSearch()
        {
            if (Session.GetHabbo().NavigatorLogs.Count > 50)
            {
                Session.SendNotif(Plus.GetLanguage().GetVar("navigator_max"));
                return;
            }
            string value1 = Request.GetString();
            string value2 = Request.GetString();
            var naviLogs = new NaviLogs(Session.GetHabbo().NavigatorLogs.Count, value1, value2);
            if (!Session.GetHabbo().NavigatorLogs.ContainsKey(naviLogs.Id))
                Session.GetHabbo().NavigatorLogs.Add(naviLogs.Id, naviLogs);
            var Message = new ServerMessage(LibraryParser.OutgoingRequest("NavigatorSavedSearchesComposer"));
            Message.AppendInteger(Session.GetHabbo().NavigatorLogs.Count);
            foreach (NaviLogs navi in Session.GetHabbo().NavigatorLogs.Values)
            {
                Message.AppendInteger(navi.Id);
                Message.AppendString(navi.Value1);
                Message.AppendString(navi.Value2);
                Message.AppendString("");
            }
            Session.SendMessage(Message);
        }

        /// <summary>
        /// Serializes the saved search.
        /// </summary>
        /// <param name="textOne">The text one.</param>
        /// <param name="textTwo">The text two.</param>
        internal void SerializeSavedSearch(string textOne, string textTwo)
        {
            GetResponse().AppendString(textOne);
            GetResponse().AppendString(textTwo);
            GetResponse().AppendString("fr");
        }

        /// <summary>
        /// News the navigator resize.
        /// </summary>
        internal void NewNavigatorResize()
        {
            int x = Request.GetInteger();
            int y = Request.GetInteger();
            int width = Request.GetInteger();
            int height = Request.GetInteger();
            Session.GetHabbo().Preferences.NewnaviX = x;
            Session.GetHabbo().Preferences.NewnaviY = y;
            Session.GetHabbo().Preferences.NewnaviWidth = width;
            Session.GetHabbo().Preferences.NewnaviHeight = height;
            Session.GetHabbo().Preferences.Save();
        }

        /// <summary>
        /// News the navigator add saved search.
        /// </summary>
        internal void NewNavigatorAddSavedSearch()
        {
            SavedSearch();
        }

        /// <summary>
        /// News the navigator delete saved search.
        /// </summary>
        internal void NewNavigatorDeleteSavedSearch()
        {
            int searchId = Request.GetInteger();
            if (!Session.GetHabbo().NavigatorLogs.ContainsKey(searchId))
                return;
            Session.GetHabbo().NavigatorLogs.Remove(searchId);
            var Message = new ServerMessage(LibraryParser.OutgoingRequest("NavigatorSavedSearchesComposer"));
            Message.AppendInteger(Session.GetHabbo().NavigatorLogs.Count);
            foreach (NaviLogs navi in Session.GetHabbo().NavigatorLogs.Values)
            {
                Message.AppendInteger(navi.Id);
                Message.AppendString(navi.Value1);
                Message.AppendString(navi.Value2);
                Message.AppendString("");
            }
            Session.SendMessage(Message);
        }

        /// <summary>
        /// News the navigator collapse category.
        /// </summary>
        internal void NewNavigatorCollapseCategory()
        {
            Request.GetString();
        }

        /// <summary>
        /// News the navigator uncollapse category.
        /// </summary>
        internal void NewNavigatorUncollapseCategory()
        {
            Request.GetString();
        }

        /// <summary>
        /// Gets the pubs.
        /// </summary>
        internal void GetPubs()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializePublicRooms());
        }

        /// <summary>
        /// Gets the room information.
        /// </summary>
        internal void GetRoomInfo()
        {
            if (Session.GetHabbo() == null)
                return;
            uint roomId = Request.GetUInteger();
            Request.GetBool();
            Request.GetBool();
            RoomData roomData = Plus.GetGame().GetRoomManager().GenerateRoomData(roomId);
            if (roomData == null)
                return;
            GetResponse().Init(LibraryParser.OutgoingRequest("1491"));
            GetResponse().AppendInteger(0);
            roomData.Serialize(GetResponse(), false);
            SendResponse();
        }

        /// <summary>
        /// Gets the popular rooms.
        /// </summary>
        internal void GetPopularRooms()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializeNavigator(Session, int.Parse(Request.GetString())));
        }

        /// <summary>
        /// Gets the recommended rooms.
        /// </summary>
        internal void GetRecommendedRooms()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializeNavigator(Session, -1));
        }

        /// <summary>
        /// Gets the popular groups.
        /// </summary>
        internal void GetPopularGroups()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializeNavigator(Session, -2));
        }

        /// <summary>
        /// Gets the high rated rooms.
        /// </summary>
        internal void GetHighRatedRooms()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializeNavigator(Session, -2));
        }

        /// <summary>
        /// Gets the friends rooms.
        /// </summary>
        internal void GetFriendsRooms()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializeNavigator(Session, -4));
        }

        /// <summary>
        /// Gets the rooms with friends.
        /// </summary>
        internal void GetRoomsWithFriends()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializeNavigator(Session, -5));
        }

        /// <summary>
        /// Gets the own rooms.
        /// </summary>
        internal void GetOwnRooms()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session == null || Session.GetHabbo() == null)
                return;

            if (Session.GetHabbo().OwnRoomsSerialized == false)
            {
                Session.GetHabbo().UpdateRooms();
                Session.GetHabbo().OwnRoomsSerialized = true;
            }

            Session.SendMessage(Plus.GetGame().GetNavigator().SerializeNavigator(Session, -3));
        }

        /// <summary>
        /// News the navigator flat cats.
        /// </summary>
        internal void NewNavigatorFlatCats()
        {
            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializeNewFlatCategories());
        }

        /// <summary>
        /// Gets the favorite rooms.
        /// </summary>
        internal void GetFavoriteRooms()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializeFavoriteRooms(Session));
        }

        /// <summary>
        /// Gets the recent rooms.
        /// </summary>
        internal void GetRecentRooms()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializeRecentRooms(Session));
        }

        /// <summary>
        /// Gets the popular tags.
        /// </summary>
        internal void GetPopularTags()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(Plus.GetGame().GetNavigator().SerializePopularRoomTags());
        }

        /// <summary>
        /// Gets the event rooms.
        /// </summary>
        internal void GetEventRooms()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(HabboHotel.Navigators.Navigator.SerializePromoted(Session, Request.GetInteger()));
        }

        /// <summary>
        /// Performs the search.
        /// </summary>
        internal void PerformSearch()
        {
            if (!Session.GetHabbo().HasFuse("user"))
            {
                return;
            }

            if (Session.GetHabbo() == null)
                return;
            Session.SendMessage(
                HabboHotel.Navigators.Navigator.SerializeSearchResults(Request.GetString()));
        }

        /// <summary>
        /// Searches the by tag.
        /// </summary>
        internal void SearchByTag()
        {
            if (Session.GetHabbo() == null)
                return;
            //this.Session.SendMessage(MercuryEnvironment.GetGame().GetNavigator().SerializeSearchResults(string.Format("tag:{0}", this.Request.PopFixedString())));
        }

        /// <summary>
        /// Performs the search2.
        /// </summary>
        internal void PerformSearch2()
        {
            if (Session.GetHabbo() == null)
                return;
            Request.GetInteger();
            //this.Session.SendMessage(MercuryEnvironment.GetGame().GetNavigator().SerializeSearchResults(this.Request.PopFixedString()));
        }

        /// <summary>
        /// Opens the flat.
        /// </summary>
        internal void OpenFlat()
        {
            if (Session.GetHabbo() == null)
                return;

            var roomId = Request.GetUInteger();
            var pWd = Request.GetString();

            PrepareRoomForUser(roomId, pWd);
        }
        internal void ToggleStaffPick()
        {
            var roomId = Request.GetUInteger();
            var current = Request.GetBool();
            var room = Plus.GetGame().GetRoomManager().GetRoom(roomId);
            if (room == null) return;
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                var pubItem = Plus.GetGame().GetNavigator().GetPublicItem(roomId);
                if (pubItem == null) // not picked
                {

                    queryReactor.SetQuery("INSERT INTO navigator_publics (bannertype, room_id, category_parent_id) VALUES ('0', @roomId, '-2')");
                    queryReactor.AddParameter("roomId", room.RoomId);
                    queryReactor.RunQuery();
                    queryReactor.RunFastQuery("SELECT last_insert_id()");
                    var publicItemId = queryReactor.GetInteger();
                    var publicItem = new PublicItem(publicItemId, 0, string.Empty, string.Empty, string.Empty, PublicImageType.Internal, room.RoomId, 0, -2, false, 1, string.Empty);
                    Plus.GetGame().GetNavigator().AddPublicItem(publicItem);
                }
                else // picked
                {
                    queryReactor.SetQuery("DELETE FROM navigator_publics WHERE id = @pubId");
                    queryReactor.AddParameter("pubId", pubItem.Id);
                    queryReactor.RunQuery();
                    Plus.GetGame().GetNavigator().RemovePublicItem(pubItem.Id);
                }
                room.RoomData.SerializeRoomData(Response, Session, false, true);
                Plus.GetGame().GetNavigator().LoadNewPublicRooms();
            }
        }
    }
}