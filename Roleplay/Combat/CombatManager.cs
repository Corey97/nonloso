﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.RoomInvokedItems;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Roleplay.Gangs;
using Plus.HabboHotel.Roleplay.Misc;
using Plus.HabboHotel.Roleplay.Jobs;
using Plus.HabboHotel.Roleplay.Timers;
using Plus.HabboHotel.PathFinding;
using Plus.HabboHotel.RoomBots;
using Plus.HabboHotel.Pets;
using Plus.Messages;
using System.Drawing;
using Plus.Util;
using System.Threading;
using Plus.HabboHotel.Rooms.Games;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Roleplay;
using System.Net;
using Plus.Messages.Parsers;
using Plus.HabboHotel.Achievements.Composer;
using Plus.Database.Manager.Database.Session_Details.Interfaces;
using Plus.HabboHotel.Roleplay.Combat.WeaponExtras;
using Plus.HabboHotel.Roleplay.Minigames.Colour_Wars;

namespace Plus.HabboHotel.Roleplay.Combat
{
    public class CombatManager
    {

        public static int DamageCalculator(GameClient Session, bool isWeapon = false)
        {

            int Damage = 0;
            int Bonus = 0;
            int str = 0;

            int fistDamage = 0;

            if (!isWeapon)
            {
                if (Session.GetRoleplay().inColourWars)
                {
                    str = new Random().Next(5, 13);
                }
                else
                {
                    str = Session.GetRoleplay().Strength + Session.GetRoleplay().UsingWeed_Bonus;
                }

                int rndNum = new Random().Next(1, 15);

                if (rndNum == 1)
                {
                    fistDamage += (str + Bonus + 1) * 3;
                }

                else if (rndNum < 5)
                {
                    fistDamage += (str + Bonus + 1) * 2;
                }

                else if (rndNum < 10)
                {
                    fistDamage += str + Bonus + 3;
                }

                else
                {
                    fistDamage += str + Bonus + 1;
                }

                Damage = new Random().Next(Session.GetRoleplay().Strength, fistDamage);

            }
            else
            {
                string WeaponName = "";
                double WeaponLevelKills = Session.GetRoleplay().GunKills / 500;
                int WeaponLevel = (int)Math.Round(WeaponLevelKills, 0);
                int WeaponDamage = WeaponManager.WeaponsData[Session.GetRoleplay().Equiped].Max_Damage;

                foreach (KeyValuePair<string, Weapon> Weapon in Session.GetRoleplay().Weapons)
                {
                    string WeaponData = Weapon.Key;
                    WeaponName = WeaponManager.GetWeaponName(WeaponData);
                }

                Damage = new Random().Next(WeaponManager.WeaponsData[Session.GetRoleplay().Equiped].Min_Damage, WeaponManager.WeaponsData[Session.GetRoleplay().Equiped].Max_Damage);
            }

            return Damage;
        }

        public static bool CanAttack(GameClient User1, GameClient User2, bool Weapon = false)
        {

            #region Fundamental Null Checks
            if (User2 == null)
            {
                User1.SendWhisper("This user was not found in this room!");
                return false;
            }

            if (User2.GetHabbo() == null)
            {
                User1.SendWhisper("This user was not found in this room!");
                return false;
            }

            if (User2.GetHabbo().CurrentRoom == null)
            {
                User1.SendWhisper("This user was not found in this room!");
                return false;
            }

            if (User2.GetHabbo().CurrentRoom != User1.GetHabbo().CurrentRoom)
            {
                User1.SendWhisper("This user was not found in this room!");
                return false;
            }
            #endregion

            Room Room = User1.GetHabbo().CurrentRoom;
            RoomUser RoomUser1 = User1.GetHabbo().GetRoomUser();
            RoomUser RoomUser2 = User2.GetHabbo().GetRoomUser();

            Vector2D Pos1 = new Vector2D(RoomUser1.X, RoomUser1.Y);
            Vector2D Pos2 = new Vector2D(RoomUser2.X, RoomUser2.Y);

            #region Cooldown Checks
            if (!Weapon)
            {
                if (User1.GetRoleplay().CoolDown > 0)
                {
                    User1.SendWhisper("You are cooling down! [" + User1.GetRoleplay().CoolDown + "/3]");
                    return false;
                }
            }
            #endregion

            #region Game Checks
            if (User2.GetRoleplay().inColourWars && !User1.GetRoleplay().inColourWars)
            {
                Room mRoom = RoleplayManager.GenerateRoom(ColourManager.MainLobby);
                User1.GetRoleplay().RequestedTaxi_Arrived = false;
                User1.GetRoleplay().RecentlyCalledTaxi = true;
                User1.GetRoleplay().RecentlyCalledTaxi_Timeout = 10;
                User1.GetRoleplay().RequestedTaxiDestination = mRoom;
                User1.GetRoleplay().RequestedTaxi = true;
                User1.GetRoleplay().taxiTimer = new taxiTimer(User1);
                User1.SendNotif("You cannot attack this person as they are in color wars! Please taxi out of the color wars room.");
                return false;
            }
            if (User1.GetRoleplay().inColourWars && !User2.GetRoleplay().inColourWars)
            {
                User1.SendNotif("You cannot attack this person as they are not in color wars!");
                return false;
            }
            #endregion

            #region Special Checks
            if(User1.GetRoleplay().RayFrozen)
            {
                User1.SendWhisper("Non puoi farlo perché sei momentaneamente congelato!");
                return false;
            }
            #endregion

            if (!Weapon)
            {

                #region Fundamental Checks
                if (Room.RoomData.Description.Contains("NOHIT") && RoleplayManager.PurgeTime == false)
                {
                    User1.SendWhisper("Mi spiace, ma qui non puoi colpire!");
                    User1.GetRoleplay().CoolDown = 3;
                    return false;
                }
                #endregion

                #region Secondary Checks

                if (Room.RoomData.Description.Contains("SAFEZONE"))
                {
                    User1.SendWhisper("Mi spiace, ma qui non puoi colpire!");
                    User1.GetRoleplay().CoolDown = 3;
                    return false;
                }
                if (Room.RoomData.Description.Contains("BRAWL") && !User1.GetRoleplay().inBrawl && !User1.GetRoleplay().Brawl)
                {
                    User1.SendWhisper("Non partecipi alla rissa!");
                    return false;
                }
                if (User2.GetRoleplay().usingPlane)
                {
                    User1.SendWhisper("You cannot hit this user as they are in the sky!");
                    User1.GetRoleplay().CoolDown = 3;
                    return false;
                }
                if (User1.GetRoleplay().Energy <= 0 && !User1.GetRoleplay().inColourWars)
                {
                    User1.SendWhisper("Non hai abbastanza energia per farlo!");
                    return false;
                }
                if (User1.GetRoleplay().Dead)
                {
                    User1.SendWhisper("Non puoi colpire nessuno perché sei morto!");
                    User1.GetRoleplay().CoolDown = 3;
                    return false;
                }

                if (User1.GetRoleplay().Jailed)
                {
                    User1.SendWhisper("Non puoi colpire nessuno perché sei in prigione!");
                    User1.GetRoleplay().CoolDown = 3;
                    return false;
                }

                if (User2.GetRoleplay().Dead)
                {
                    User1.SendWhisper("Non puoi colpire questo utente perché già morto!");
                    User1.GetRoleplay().CoolDown = 3;
                    return false;
                }

                if (User2.GetRoleplay().Jailed)
                {
                    User1.SendWhisper("Non puoi colpire questo utente perché è in prigione!");
                    User1.GetRoleplay().CoolDown = 3;
                    return false;
                }

                if (RoomUser1.Stunned)
                {
                    User1.SendWhisper("Non puoi colpire mentre sei stordito!");
                    User1.GetRoleplay().CoolDown = 3;
                    return false;
                }
                if (RoomUser2.IsAsleep)
                {
                    User1.SendWhisper("Non puoi colpire un utente mentre è assente!");
                    User1.GetRoleplay().CoolDown = 3;
                    return false;
                }
                if (User1.GetRoleplay().IsNoob && !User1.GetHabbo().CurrentRoom.RoomData.Description.Contains("COLOR") && !User1.GetHabbo().CurrentRoom.RoomData.Description.Contains("MAFIA"))
                {
                    if (!User1.GetRoleplay().NoobWarned)
                    {
                        User1.SendNotif("If you choose to do this again your temporary God Protection will be disabled!");
                        User1.GetRoleplay().NoobWarned = true;
                        return false;
                    }
                    else
                    {
                        using (IQueryAdapter dbClient = Plus.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("UPDATE rp_stats SET is_noob = 0 WHERE id = '" + User1.GetHabbo().Id + "'");
                        }

                        User1.SendWhisper("Your god protection has been disabled!");
                        User1.GetRoleplay().IsNoob = false;
                        User1.GetRoleplay().SaveQuickStat("is_noob", "0");

                    }
                }

                if (User2.GetRoleplay().IsNoob && !User1.GetHabbo().CurrentRoom.RoomData.Description.Contains("COLOR") && !User1.GetHabbo().CurrentRoom.RoomData.Description.Contains("MAFIA") && !User1.GetHabbo().CurrentRoom.RoomData.Description.Contains("NPA"))
                {
                    User1.SendWhisper("Cannot complete this action as this user is under God Protection!");
                    return false;
                }
                #endregion

                #region Final Checks
                if (RoleplayManager.WithinAttackDistance(RoomUser1, RoomUser2))
                {

                }
                else if (RoleplayManager.Distance(Pos1, Pos2) > 2 && RoleplayManager.Distance(Pos1, Pos2) <= 4)
                {
                    RoomUser1.LastBubble = 3;

                    User1.Shout("*Swings at " + User2.GetHabbo().UserName + ", but misses*");
                    User1.GetRoleplay().CoolDown = 3;

                    RoomUser1.LastBubble = 0;
                    return false;
                }
                else if (RoleplayManager.Distance(Pos1, Pos2) >= 5)
                {
                    User1.SendWhisper("You are too far away!");
                    User1.GetRoleplay().CoolDown = 3;
                    return false;
                }
                #endregion

            }
            else if (Weapon)
            {

                #region Fundamental Checks
                if (User1.GetRoleplay().Energy < WeaponManager.WeaponsData[User1.GetRoleplay().Equiped].Need_Energy)
                {
                    User1.SendWhisper("Non hai abbastanza energia.!");
                    return false;
                }
                if (User1.GetRoleplay().inBrawl == true)
                {
                    User1.SendWhisper("Non fai parte della rissa.");
                    return false;
                }

                if (User1.GetRoleplay().Dead)
                {
                    User1.SendWhisper("Non puoi colpire mentre sei morto!");
                    return false;
                }

                if (User1.GetRoleplay().Jailed)
                {
                    User1.SendWhisper("Non puoi colpire questo utente perché sei in prigione!");
                    return false;
                }

                if (User2.GetRoleplay().Dead)
                {
                    User1.SendWhisper("Non puoi colpire questo utente perché è morto!");
                    return false;
                }

                if (User2.GetRoleplay().Jailed)
                {
                    User1.SendWhisper("Non puoi colpire questo utente perché è in prigione!");
                    return false;
                }

                if (RoomUser1.Stunned)
                {
                    User1.SendWhisper("Non puoi colpire questo utente perché sei stordito!");
                    return false;
                }
                if (RoomUser2.IsAsleep)
                {
                    User1.SendWhisper("Non puoi colpire un utente mentre è assente!");
                    return false;
                }
                if (RoomUser1.IsAsleep)
                {
                    User1.SendWhisper("Non puoi colpire un utente mentre è assente!");
                    return false;
                }
                if (User1.GetRoleplay().IsNoob && !User1.GetHabbo().CurrentRoom.RoomData.Description.Contains("COLOR") && !User1.GetHabbo().CurrentRoom.RoomData.Description.Contains("MAFIA"))
                {
                    if (!User1.GetRoleplay().NoobWarned)
                    {
                        User1.SendNotif("If you choose to do this again your temporary God Protection will be disabled!");
                        User1.GetRoleplay().NoobWarned = true;
                        return false;
                    }
                    else
                    {
                        using (IQueryAdapter dbClient = Plus.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("UPDATE rp_stats SET is_noob = 0 WHERE id = '" + User1.GetHabbo().Id + "'");
                        }

                        User1.SendWhisper("Your god protection has been disabled!");
                        User1.GetRoleplay().IsNoob = false;
                        User1.GetRoleplay().SaveQuickStat("is_noob", "0");

                    }
                }

                if (User2.GetRoleplay().IsNoob && !User1.GetHabbo().CurrentRoom.RoomData.Description.Contains("COLOR") && !User1.GetHabbo().CurrentRoom.RoomData.Description.Contains("MAFIA") && !User1.GetHabbo().CurrentRoom.RoomData.Description.Contains("NPA"))
                {
                    User1.SendWhisper("Cannot complete this action as this user is under God Protection!");
                    return false;
                }
                #endregion

                #region Secondary Checks
                if (Room.RoomData.Description.Contains("NOSHOOT") && RoleplayManager.PurgeTime == false)
                {
                    User1.SendWhisper("Mi spiace ma qui non puoi colpire!");
                    return false;
                }
                if (Room.RoomData.Description.Contains("SAFEZONE"))
                {
                    User1.SendWhisper("Mi spiace ma qui non puoi colpire!");
                    return false;
                }
                if (User1.GetRoleplay().Equiped == null)
                {
                    User1.SendWhisper("Non hai nessun'arma alla mano!");
                    return false;
                }
                if (!WeaponManager.WeaponsData[User1.GetRoleplay().Equiped].Type.ToLower().Contains("gun"))
                {
                    User1.SendWhisper("L'arma che hai equipaggiato, non è un arma da fuoco!");
                    return false;
                }
                if (User2.GetRoleplay().usingPlane && !WeaponManager.WeaponsData[User1.GetRoleplay().Equiped].Name.ToLower().Contains("rpg"))
                {
                    User1.Shout("*Prova a colpire: " + User2.GetHabbo().UserName + ", ma lo manca poiché su un aereo.*");
                    User1.GetRoleplay().GunShots++;
                    return false;
                }
                int MyJobId = User1.GetRoleplay().JobId;
                int MyJobRank = User1.GetRoleplay().JobRank;
                if (User1.GetRoleplay().Bullets <= 0 && !JobManager.JobRankData[MyJobId, MyJobRank].hasRights("army") && !User1.GetRoleplay().Working)
                {
                    User1.SendWhisper("Non hai abbastanza proiettili!");
                    return false;
                }
                if (User1.GetRoleplay().Intelligence < WeaponManager.WeaponsData[User1.GetRoleplay().Equiped].Min_Intel)
                {
                    User1.SendWhisper("Devi avere un livello minimo di intelligenza: " + WeaponManager.WeaponsData[User1.GetRoleplay().Equiped].Min_Intel + " per usare quest'arma!");
                    return false;
                }
                if (User1.GetRoleplay().Strength < WeaponManager.WeaponsData[User1.GetRoleplay().Equiped].Need_Str)
                {
                    User1.SendWhisper("Devi avere un livelli minimo di forza: " + WeaponManager.WeaponsData[User1.GetRoleplay().Equiped].Need_Str + " per usare quest'arma!");
                    return false;
                }
                if (User1.GetRoleplay().GunShots >= WeaponManager.WeaponsData[User1.GetRoleplay().Equiped].Reload_Time)
                {
                    RoomUser1.LastBubble = 3;

                    User1.Shout("*Ricarica l'arma, riempiendo il caricatore*");

                    RoomUser1.LastBubble = 0;
                    ServerMessage message = new ServerMessage(LibraryParser.OutgoingRequest("FloodFilterMessageComposer"));
                    message.AppendInteger(5);
                    RoomUser1.IsGunReloaded = true;
                    RoomUser1.ReloadExpiryTime = Plus.GetUnixTimeStamp() + 5;
                    User1.SendMessage(message);

                    User1.GetRoleplay().GunShots = 0;
                    // will re-inplement this later -> HabboHotel.Misc.ChatCommandHandler.Parse(User1, ":shoot " + TargetUser1.GetHabbo().UserName);
                    return false;
                }
                #endregion

                #region Final Checks
                if (RoleplayManager.Distance(Pos1, Pos2) > WeaponManager.WeaponsData[User1.GetRoleplay().Equiped].Range)
                {
                    /*if (Misc.Distance(Pos1, Pos2) > weaponManager.WeaponsData[User1.GetRoleplay().Equiped].Range + 4)
                    {
                        User1.SendWhisper("You must be closer to do this!");
                        return false;
                    }*/

                    RoomUser1.LastBubble = 3;

                    User1.Shout("*Spara con: " + WeaponManager.WeaponsData[User1.GetRoleplay().Equiped].Name + " a " + User2.GetHabbo().UserName + ", ma lo manca.*");

                    RoomUser1.LastBubble = 0;
                    User1.GetRoleplay().GunShots++;
                    return false;
                }
                #endregion

            }

            return true;
        }

        public static void HandleGun(GameClient Session, GameClient TargetSession, Room Room, RoomUser RoomUser, RoomUser Target, string WeaponName, int Damage)
        {
            // string WeaponName = 
            switch (WeaponName)
            {

                case "freezeray":

                    #region Freeze Ray

                    Session.GetRoleplay().FreezeRay.HandleGeneralAttack(TargetSession);
                            
                    #endregion

                    break;




                default:

                    #region Normal Weapons
                    if (TargetSession == null)
                        return;

                    RoomUser.LastBubble = 3;
                    Target.LastBubble = 3;

                    if (Session.GetRoleplay().Equiped == null)
                        return;

                    if (!WeaponManager.isWeapon(Session.GetRoleplay().Equiped.ToLower()))
                        return;

                    if (WeaponManager.WeaponsData[Session.GetRoleplay().Equiped].Speech == null || WeaponManager.WeaponsData[Session.GetRoleplay().Equiped].Speech == "")
                    {
                        Session.Shout("*Colpisce a " + TargetSession.GetHabbo().UserName + ", causando " + Damage + " di danno [-" + WeaponManager.WeaponsData[Session.GetRoleplay().Equiped].Need_Energy + " Energia]*");
                    }
                    else
                    {
                        string finalspeech = WeaponManager.WeaponsData[Session.GetRoleplay().Equiped].Speech.Replace("%energy%", Convert.ToString(WeaponManager.WeaponsData[Session.GetRoleplay().Equiped].Need_Energy)).Replace("%damage%", Convert.ToString(Damage)).Replace("%username%", TargetSession.GetHabbo().UserName);
                        Session.Shout(finalspeech);
                    }

                    if (TargetSession.GetRoleplay().Armor >= 1)
                    {
                        TargetSession.commandShout("*[" + TargetSession.GetRoleplay().Armor + "AP rimanenti!]*");
                    }
                    else
                    {
                        TargetSession.commandShout("*[" + TargetSession.GetRoleplay().CurHealth + "/" + TargetSession.GetRoleplay().MaxHealth + "]*");
                    }
                    RoomUser.LastBubble = 0;
                    Target.LastBubble = 0;
                    #endregion

                    break;
            }

        }
    }
}