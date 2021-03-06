using Plus.Database.Manager.Database.Session_Details.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.Messages;
using Plus.Messages.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plus.HabboHotel.Items.Interactor
{
    internal class InteractorMannequin : IFurniInteractor
    {
        public void OnPlace(GameClient session, RoomItem item)
        {
        }

        public void OnRemove(GameClient session, RoomItem item)
        {
        }

        public void OnTrigger(GameClient session, RoomItem item, int request, bool hasRights)
        {
            if (!item.ExtraData.Contains(Convert.ToChar(5).ToString()))
            {
                return;
            }
            string[] array = item.ExtraData.Split(new[]
            {
                Convert.ToChar(5)
            });
            session.GetHabbo().Gender = array[0].ToUpper();
            var dictionary = new Dictionary<string, string>();
            dictionary.Clear();
            string[] array2 = array[1].Split('.');
            foreach (string text in array2)
            {
                string[] array3 = session.GetHabbo().Look.Split('.');
                foreach (string text2 in array3)
                {
                    if (text2.Split('-')[0] == text.Split('-')[0])
                    {
                        if (dictionary.ContainsKey(text2.Split('-')[0]) && !dictionary.ContainsValue(text))
                        {
                            dictionary.Remove(text2.Split('-')[0]);
                            dictionary.Add(text2.Split('-')[0], text);
                        }
                        else
                        {
                            if (!dictionary.ContainsKey(text2.Split('-')[0]) && !dictionary.ContainsValue(text))
                            {
                                dictionary.Add(text2.Split('-')[0], text);
                            }
                        }
                    }
                    else
                    {
                        if (!dictionary.ContainsKey(text2.Split('-')[0]))
                        {
                            dictionary.Add(text2.Split('-')[0], text2);
                        }
                    }
                }
            }
            string text3 = dictionary.Values.Aggregate("", (current1, current) => string.Format("{0}{1}.", current1, current));
            session.GetHabbo().Look = text3.TrimEnd('.');
            using (IQueryAdapter queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery(string.Format("UPDATE users SET look = @look, gender = @gender WHERE id = {0}", session.GetHabbo().Id));
                queryReactor.AddParameter("look", session.GetHabbo().Look);
                queryReactor.AddParameter("gender", session.GetHabbo().Gender);
                queryReactor.RunQuery();
            }
            session.GetMessageHandler().GetResponse().Init(LibraryParser.OutgoingRequest("UpdateUserDataMessageComposer"));
            session.GetMessageHandler().GetResponse().AppendInteger(-1);
            session.GetMessageHandler().GetResponse().AppendString(session.GetHabbo().Look);
            session.GetMessageHandler().GetResponse().AppendString(session.GetHabbo().Gender.ToLower());
            session.GetMessageHandler().GetResponse().AppendString(session.GetHabbo().Motto);
            session.GetMessageHandler().GetResponse().AppendInteger(session.GetHabbo().AchievementPoints);
            session.GetMessageHandler().SendResponse();
            RoomUser roomUserByHabbo = item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("UpdateUserDataMessageComposer"));
            serverMessage.AppendInteger(roomUserByHabbo.VirtualId);
            serverMessage.AppendString(session.GetHabbo().Look);
            serverMessage.AppendString(session.GetHabbo().Gender.ToLower());
            serverMessage.AppendString(session.GetHabbo().Motto);
            serverMessage.AppendInteger(session.GetHabbo().AchievementPoints);
            session.GetHabbo().CurrentRoom.SendMessage(serverMessage);
        }

        public void OnUserWalk(GameClient session, RoomItem item, RoomUser user)
        {
        }

        public void OnWiredTrigger(RoomItem item)
        {
        }
    }
}