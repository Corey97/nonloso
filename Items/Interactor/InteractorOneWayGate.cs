using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Roleplay.Misc;
using Plus.Database.Manager.Database.Session_Details.Interfaces;

namespace Plus.HabboHotel.Items.Interactor
{
    internal class InteractorOneWayGate : IFurniInteractor
    {
        public void OnPlace(GameClient session, RoomItem item)
        {
            item.ExtraData = "0";

            if (item.InteractingUser != 0)
            {
                var User = item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(item.InteractingUser);

                if (User != null)
                {
                    User.ClearMovement();
                    User.UnlockWalking();
                }

                item.InteractingUser = 0;
            }
        }

        public void OnRemove(GameClient session, RoomItem item)
        {
            item.ExtraData = "0";

            if (item.InteractingUser != 0)
            {
                var User = item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(item.InteractingUser);

                if (User != null)
                {
                    User.ClearMovement();
                    User.UnlockWalking();
                }

                item.InteractingUser = 0;
            }
        }

        public void OnTrigger(GameClient session, RoomItem item, int request, bool hasRights)
        {
            if (session == null)
                return;
            var User = item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            if (User.Coordinate != item.SquareInFront && User.CanWalk)
            {
                User.MoveTo(item.SquareInFront);
                return;
            }

            if (!item.GetRoom().GetGameMap().CanWalk(item.SquareBehind.X, item.SquareBehind.Y, User.AllowOverride))
            {
                return;
            }

            if (item.GetBaseItem().Name.ToLower().Contains("one_way"))
            {
                if (User.GetClient().GetRoleplay().Dead)
                {
                    User.GetClient().SendWhisper("You cannot enter through this gate while you are dead!");
                    return;
                }
                else if (User.GetClient().GetRoleplay().Jailed)
                {
                    User.GetClient().SendWhisper("You cannot enter through this gate while you are jailed!");
                    return;
                }
            }

            if (item.GetBaseItem().Name == "one_way_door*4")
            {
                if (!User.GetClient().GetRoleplay().Working)
                {
                    User.GetClient().SendWhisper("Devi lavorare per passare!");
                    return;
                }
                if (!HabboHotel.Roleplay.Jobs.JobManager.JobRankData[session.GetRoleplay().JobId, session.GetRoleplay().JobRank].isWorkRoom(session.GetHabbo().CurrentRoomId) || session.GetRoleplay().JobId == 1)
                {
                    User.GetClient().SendWhisper("Non lavori qui, non puoi passare!");
                    return;
                }
            }

            if (item.GetBaseItem().Name == "one_way_door*6")
            {
                //HabboHotel.Roleplay.Minigames.Paintball.BlueTeamJoin(User.GetClient());
            }

            if (item.GetBaseItem().Name == "one_way_door*2")
            {
                if (User.GetClient().GetHabbo().CurrentRoom.RoomData.Description.Contains("PAINTBALL_LOBBY"))
                {
                    //HabboHotel.Roleplay.Minigames.Paintball.RedTeamJoin(User.GetClient());
                }
                else
                {
                    RoleplayManager.GiveMoney(User.GetClient(), -25);
                    User.GetClient().SendWhisper("You have been charged $25 for using this gate!");
                }
            }

            if (item.GetBaseItem().Name == "one_way_door*9")
            {

                if (User.GetClient().GetHabbo().CurrentRoom.RoomData.Description.Contains("HUNGERGAMES"))
                {
                    if (!User.GetClient().GetRoleplay().HungerGames || 1 == 1)
                    {
                        User.GetClient().GetRoleplay().InMiniGame = true;
                        User.GetClient().GetRoleplay().HungerGames = true;
                        session.SendWhisper("You are now part of this Hunger Games session, please wait for the game to begin..");
                        session.Shout("*Joins Hunger Games session*");
                        GameClient mClient = session;
                        mClient.GetRoleplay().HungerGames_Cash = 0;
                        mClient.GetRoleplay().HungerGames_Dead = false;
                        mClient.GetRoleplay().HungerGames_Inventory.Clear();
                        mClient.GetRoleplay().HungerGames_Item_Wielding = "";
                        mClient.GetRoleplay().HungerGames_Pts = 0;
                    }
                }
            }

            if (item.GetBaseItem().Name == "one_way_door*8")
            {
                if (User.GetClient().GetHabbo().CurrentRoom.RoomData.Description.Contains("BRAWL"))
                {
                    if (User.GetClient().GetRoleplay().IsNoob)
                    {
                        if (!User.GetClient().GetRoleplay().NoobWarned)
                        {
                            User.GetClient().SendNotif("If you choose to do this again your temporary God Protection will be disabled!");
                            User.GetClient().GetRoleplay().NoobWarned = true;
                            return;
                        }
                        else
                        {
                            using (IQueryAdapter dbClient = Plus.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunFastQuery("UPDATE rp_stats SET is_noob = 0 WHERE id = '" + User.GetClient().GetHabbo().Id + "'");
                            }
                            User.GetClient().SendWhisper("Your god protection has been disabled!");
                            User.GetClient().GetRoleplay().IsNoob = false;
                            User.GetClient().GetRoleplay().SaveQuickStat("is_noob", "0");
                        }
                    }
                    else
                    {
                        if (session.GetRoleplay().inBrawl)
                        {
                            session.SendWhisper("You have already registered for the brawl event, but you have been re-registered!");
                        }
                        else
                        {
                            session.GetRoleplay().Brawl = true;
                            session.GetRoleplay().inBrawl = true;
                            RoleplayManager.Shout(User.GetClient(), "*Applies for Brawl and enters in gate*");
                        }
                    }
                }
                else
                {
                    User.GetClient().SendWhisper("You're not in the brawl!");
                    return;
                }
            }

            if (item.InteractingUser == 0)
            {
                item.InteractingUser = User.HabboId;

                User.CanWalk = false;

                if (User.IsWalking && (User.GoalX != item.SquareInFront.X || User.GoalY != item.SquareInFront.Y))
                {
                    User.ClearMovement();
                }

                User.AllowOverride = true;
                User.MoveTo(item.Coordinate);

                item.ReqUpdate(4, true);
            }
        }

        public void OnUserWalk(GameClient session, RoomItem item, RoomUser user)
        {
        }

        public void OnWiredTrigger(RoomItem item)
        {
        }
    }
}