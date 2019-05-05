using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using System;
using System.Collections.Generic;
using Plus.HabboHotel.Roleplay.Misc;
using System.Linq;
using System.Text;
using Plus.HabboHotel.Roleplay.Jobs.Space;
using Plus.HabboHotel.Roleplay.Jobs.Farming;
using Plus.HabboHotel.Roleplay.Jobs;
using Plus.HabboHotel.Roleplay.Gangs;
using Plus.HabboHotel.Roleplay.Combat;
using Plus.HabboHotel.Roleplay.Timers;
using Plus.HabboHotel.Roleplay.Radio;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorRP : IFurniInteractor
    {
        public void OnUserWalk(GameClient session, RoomItem item, RoomUser user)
        {

        }

        public void OnPlace(GameClients.GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";
            Item.UpdateNeeded = true;
        }

        public void OnRemove(GameClients.GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";
        }

        public void OnTrigger(GameClients.GameClient Session, RoomItem Item, int Request, bool HasRights)
        {
            if (Item.GetBaseItem().Name.ToLower().Contains("atm"))
            {
                HandleATM(Session, Item, Request, HasRights);
            }

            if (Item.BaseItem == 1943) // prison_stones baseID
            {
                handleRock(Session, Item);
            }

            if (Item.BaseItem == 1737) // dirt nest baseID
            {
                handleFarmingSpot(Session, Item);
            }

            if (Item.GetBaseItem().Name.ToLower().Contains("wf_floor_switch1"))
            {
                HandlePullTheHandleForPolice(Session, Item, Request, HasRights);
            }

            if (Item.GetBaseItem().Name.ToLower().Contains("hc_machine"))
            {
                HandleNPA(Session, Item, Request, HasRights);
            }

            uint Modes = Item.GetBaseItem().Modes - 1;

            if (Session == null || !HasRights || Modes <= 0)
            {
                return;
            }

            Plus.GetGame().GetQuestManager().ProgressUserQuest(Session, HabboHotel.Quests.QuestType.FurniSwitch);

            int CurrentMode = 0;
            int NewMode = 0;

            if (!int.TryParse(Item.ExtraData, out CurrentMode))
            {

            }

            if (CurrentMode <= 0)
            {
                NewMode = 1;
            }
            else if (CurrentMode >= Modes)
            {
                NewMode = 0;
            }
            else
            {
                NewMode = CurrentMode + 1;
            }

            Item.ExtraData = NewMode.ToString();
            Item.UpdateState();

        }

        public void handleRock(GameClient Session, RoomItem Item)
        {
            RoomUser User = Session.GetHabbo().GetRoomUser();
            Rock theRock = spaceManager.getRockByItem(Item);

            if (!JobManager.JobRankData[Session.GetRoleplay().JobId, Session.GetRoleplay().JobRank].hasRights("spaceminer"))
            {
                Session.SendWhisperBubble("You must be a space miner to mine this rock!", 1);
                return;
            }

            if (!Session.GetRoleplay().Working)
            {
                Session.SendWhisperBubble("Devi lavorare per farlo!", 1);
                return;
            }
            if (theRock != null && spaceManager.isUserNearRock(theRock, User))
            {
                if (theRock.beingMined == false)
                {
                    mineTimer timer = new mineTimer(Session, theRock);
                    timer.startTimer();
                }
                else
                {
                    Session.SendWhisperBubble("This rock is already being mined!", 1);
                }
            }
            else
            {
                Session.SendWhisperBubble("You aren't close enough to the rock!", 1);
            }
        }

        public void handleFarmingSpot(GameClient Session, RoomItem Item)
        {
            RoomUser User = Session.GetHabbo().GetRoomUser();
            FarmingSpot theFarmingSpot = farmingManager.getFarmingSpotByItem(Item);

            if (!JobManager.JobRankData[Session.GetRoleplay().JobId, Session.GetRoleplay().JobRank].hasRights("farming"))
            {
                if (theFarmingSpot.type == "weed") { Session.SendWhisperBubble("You must be a farmer to plant weed here!", 1); }
                else if (theFarmingSpot.type == "carrot") { Session.SendWhisperBubble("You must be a farmer to plant carrots here!", 1); }
                else { Session.SendWhisperBubble("You must be a farmer to plant here!", 1); }
                return;
            }

            if (!Session.GetRoleplay().Working)
            {
                if (theFarmingSpot.type == "weed") { Session.SendWhisperBubble("You must be working to plant weed here!", 1); }
                else if (theFarmingSpot.type == "carrot") { Session.SendWhisperBubble("You must be working to plant carrots here!", 1); }
                else { Session.SendWhisperBubble("You must be working to plant here!", 1); }
                return;
            }

            if (theFarmingSpot != null && farmingManager.isUserNearFarmingSpot(theFarmingSpot, User))
            {
                if (theFarmingSpot.beingFarmed2 == false && theFarmingSpot.Part1Complete == true)
                {
                    farmingTimer2 timer = new farmingTimer2(Session, theFarmingSpot);
                    timer.startTimer();
                }
                else if (theFarmingSpot.beingFarmed == false)
                {
                    farmingTimer1 timer = new farmingTimer1(Session, theFarmingSpot);
                    timer.startTimer();
                }
                else
                {
                    Session.SendWhisperBubble("This spot is already being farmed!", 1);
                }
            }
            else
            {
                Session.SendWhisperBubble("You aren't close enough to the farming spot!", 1);
            }
        }

        public void HandleATM(GameClients.GameClient Session, RoomItem Item, int Request, bool HasRights)
        {
            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);


            if (Item.InteractingUser2 != User.UserId)
                Item.InteractingUser2 = User.UserId;

            if (User == null)
            {
                return;
            }

            if (User.Coordinate != Item.SquareInFront && User.CanWalk)
            {
                User.MoveTo(Item.SquareInFront);
                return;
            }
            if (Session.GetRoleplay().inATM == true)
            {
                Session.SendWhisperBubble("[Bancomat MSG] Sei entrato con successo!", 1);
                return;
            }

            Session.GetRoleplay().inATM = true;

            Session.SendWhisperBubble("[Bancomat MSG] Transazione in corso, attendere...", 1);

            int amount = Session.GetRoleplay().AtmSetAmount;

            if (amount > Session.GetRoleplay().Bank)
            {
                Session.SendWhisperBubble("[Bancomat MSG] Transazione fallita, fondi insufficenti!", 1);
                Session.GetHabbo().GetRoomUser().UnlockWalking();
                Session.GetRoleplay().inATM = false;
                return;
            }
            else
            {

                System.Threading.Thread.Sleep(2000);

                Session.SendWhisperBubble("[Bancomat MSG] Transazione eseguita con successo!", 1);
                RoleplayManager.Shout(Session, "*Si avvicina al bancomat e ritira la bellezza di " + amount + " euro*");
                Session.GetHabbo().GetRoomUser().UnlockWalking();
                Session.GetRoleplay().inATM = false;
                Session.GetRoleplay().Bank -= amount;
                Session.GetRoleplay().SaveQuickStat("bank", "" + Session.GetRoleplay().Bank);
                RoleplayManager.GiveMoney(Session, +amount);
                Session.GetRoleplay().AtmSetAmount = 20;
            }
        }

        public void HandlePullTheHandleForPolice(GameClients.GameClient Session, RoomItem Item, int Request, bool HasRights)
        {

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);


            if (Item.InteractingUser2 != User.UserId)
                Item.InteractingUser2 = User.UserId;

            if (User == null)
            {
                return;
            }

            if (User.Coordinate != Item.SquareInFront && User.CanWalk)
            {
                User.MoveTo(Item.SquareInFront);
                return;
            }

            if (!Session.GetRoleplay().MultiCoolDown.ContainsKey("rp_vswitch"))
            {
                Session.GetRoleplay().MultiCoolDown.Add("rp_vswitch", 0);
            }
            if (Session.GetRoleplay().MultiCoolDown["rp_vswitch"] > 0)
            {
                Session.SendWhisperBubble("You must wait until you can pull the switch! [" + Session.GetRoleplay().MultiCoolDown["rp_vswitch"] + "/15]", 1);
                return;
            }

            lock (Plus.GetGame().GetClientManager().Clients.Values)
            {
                foreach (GameClient client in Plus.GetGame().GetClientManager().Clients.Values)
                {

                    if (client == null)
                        continue;
                    if (client.GetRoleplay() == null)
                        continue;
                    if (!JobManager.validJob(client.GetRoleplay().JobId, client.GetRoleplay().JobRank))
                        continue;
                    if (!client.GetRoleplay().JobHasRights("police"))
                        continue;
                    if (!client.GetRoleplay().JobHasRights("swat"))
                        continue;
                    if (!client.GetRoleplay().Working)
                        continue;

                    string msg = Session.GetHabbo().UserName + " has pulled the switch at [Room ID: " + Session.GetHabbo().CurrentRoomId + "], asking for help!";
                    Radio.send(msg, Session);
                }
            }
            RoleplayManager.Shout(Session, "*Pulls the trigger of the switch, notifying the cops*", 4);
            Session.GetRoleplay().MultiCoolDown["rp_vswitch"] = 15;
            Session.GetRoleplay().CheckingMultiCooldown = true;
        }

        public void HandleNPA(GameClients.GameClient Session, RoomItem Item, int Request, bool HasRights)
        {

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);


            if (Item.InteractingUser2 != User.UserId)
                Item.InteractingUser2 = User.UserId;

            if (User == null)
            {
                return;
            }

            if (User.Coordinate != Item.SquareInFront && User.CanWalk)
            {
                User.MoveTo(Item.SquareInFront);
                return;
            }

            if (Item.OnNPAUsing)
            {
                User.GetClient().SendWhisperBubble("Someone is already using this machine to nuke the city!", 1);
                return;
            }

            if (RoleplayManager.NukesOccurred > 5)
            {
                User.GetClient().SendWhisperBubble("The system has reached a maximum amount of nukes per emulator reboot. Please try again later.", 1);
                return;
            }

            RoleplayManager.Shout(User.GetClient(), "*Starts the process in nuking the city*", 4);
            User.GetClient().GetRoleplay().npaTimer = new nukeTimer(User.GetClient());
            User.GetClient().GetRoleplay().NPA = true;
            User.GetClient().SendWhisperBubble("You have " + User.GetClient().GetRoleplay().npaTimer.getTime() + " minutes until you nuke the city.", 1);
            Item.OnNPAUsing = true;
            RoleplayManager.NukesOccurred++;


        }

        public void OnWiredTrigger(RoomItem Item)
        {
            if (Item.ExtraData == "0")
            {
                Item.ExtraData = "1";
                Item.UpdateState(false, true);
                Item.ReqUpdate(4, true);
            }
        }
    }
}