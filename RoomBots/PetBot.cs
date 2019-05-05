using Plus.Configuration;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Pets;
using Plus.HabboHotel.Rooms;
using Plus.Messages;
using Plus.Messages.Parsers;
using Plus.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Plus.HabboHotel.RoomBots
{
    /// <summary>
    /// Class PetBot.
    /// </summary>
    internal class PetBot : BotAI
    {
        /// <summary>
        /// The _speech timer
        /// </summary>
        private int _speechTimer;

        /// <summary>
        /// The _action timer
        /// </summary>
        private int _actionTimer;

        /// <summary>
        /// The _energy timer
        /// </summary>
        private int _energyTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PetBot"/> class.
        /// </summary>
        /// <param name="virtualId">The virtual identifier.</param>
        internal PetBot(int virtualId)
        {
            {
                _speechTimer = new Random((virtualId ^ 2) + DateTime.Now.Millisecond).Next(10, 60);
                _actionTimer = new Random((virtualId ^ 2) + DateTime.Now.Millisecond).Next(10, 30 + virtualId);
                _energyTimer = new Random((virtualId ^ 2) + DateTime.Now.Millisecond).Next(10, 60);
            }
        }

        /// <summary>
        /// Called when [self enter room].
        /// </summary>
        internal override void OnSelfEnterRoom()
        {
            Point randomWalkableSquare = GetRoom().GetGameMap().GetRandomWalkableSquare();
            if (GetRoomUser() != null && GetRoomUser().PetData.Type != 16u)
            {
                GetRoomUser().MoveTo(randomWalkableSquare.X, randomWalkableSquare.Y);
            }
        }

        /// <summary>
        /// Called when [self leave room].
        /// </summary>
        /// <param name="kicked">if set to <c>true</c> [kicked].</param>
        internal override void OnSelfLeaveRoom(bool kicked)
        {
        }

        /// <summary>
        /// Modifieds this instance.
        /// </summary>
        internal override void Modified()
        {
        }

        /// <summary>
        /// Called when [user enter room].
        /// </summary>
        /// <param name="user">The user.</param>
        internal override void OnUserEnterRoom(RoomUser user)
        {
            if (user.GetClient() == null || user.GetClient().GetHabbo() == null)
                return;

            RoomUser roomUser = GetRoomUser();
            if (roomUser == null || user.GetClient().GetHabbo().UserName != roomUser.PetData.OwnerName)
                return;

            var random = new Random();
            string[] value = PetLocale.GetValue("welcome.speech.pet");
            string message = value[random.Next(0, (value.Length - 1))];

            message += user.GetUserName();
            roomUser.Chat(null, message, false, 0, 0);
        }

        /// <summary>
        /// Called when [user leave room].
        /// </summary>
        /// <param name="client">The client.</param>
        internal override void OnUserLeaveRoom(GameClient client)
        {
        }

        /// <summary>
        /// Called when [user say].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="msg">The MSG.</param>
        internal override void OnUserSay(RoomUser user, string msg)
        {
            RoomUser roomUser = GetRoomUser();

            if (roomUser.PetData.OwnerId != user.GetClient().GetHabbo().Id)
            {
                return;
            }
            if (string.IsNullOrEmpty(msg))
            {
                msg = " ";
            }
            msg = msg.Substring(1);

            bool lazy = false;
            bool unknown = false;
            bool sleeping = false;
            try
            {
                int command = PetCommandHandler.TryInvoke(msg);
                switch (command)
                {
                    case 1:
                        RemovePetStatus();
                        roomUser.FollowingOwner = null;
                        break;

                    case 2:
                        if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                        {
                            lazy = true;
                            break;
                        }
                        RemovePetStatus();
                        roomUser.PetData.AddExperience(10);
                        roomUser.Statusses.Add("sit", "");
                        roomUser.Statusses.Add("gst", "joy");
                        roomUser.UpdateNeeded = true;
                        _actionTimer = 25;
                        _energyTimer = 10;

                        SubtractAttributes();
                        break;

                    case 3:
                        if (!roomUser.PetData.HasCommand(2))
                        {
                            unknown = true;
                            break;
                        }
                        if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                        {
                            lazy = true;
                            break;
                        }
                        RemovePetStatus();
                        roomUser.PetData.AddExperience(10);
                        roomUser.Statusses.Add("lay", "");
                        roomUser.Statusses.Add("gst", "sml");
                        roomUser.UpdateNeeded = true;
                        _actionTimer = 25;
                        _energyTimer = 10;

                        SubtractAttributes();
                        break;

                    case 4:
                        if (!roomUser.PetData.HasCommand(3))
                        {
                            unknown = true;
                            break;
                        }
                        if (!roomUser.PetData.HasCommand(7))
                        {
                            unknown = true;
                            break;
                        }
                        if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                        {
                            lazy = true;
                            break;
                        }
                        RemovePetStatus();
                        roomUser.PetData.AddExperience(11);
                        roomUser.MoveTo(user.SquareInFront);
                        roomUser.Statusses.Add("gst", "sml");
                        roomUser.UpdateNeeded = true;

                        _actionTimer = 25;
                        _energyTimer = 10;
                        SubtractAttributes();
                        break;

                    case 5:
                        if (!roomUser.PetData.HasCommand(8))
                        {
                            unknown = true;
                            break;
                        }
                        if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                        {
                            lazy = true;
                            break;
                        }
                        RemovePetStatus();
                        roomUser.PetData.AddExperience(25);
                        roomUser.Statusses.Add("std", "");
                        roomUser.UpdateNeeded = true;

                        _actionTimer = 25;
                        _energyTimer = 10;
                        SubtractAttributes();
                        break;

                    case 6:
                        if (!roomUser.PetData.HasCommand(19))
                        {
                            unknown = true;
                            break;
                        }
                        if (!roomUser.PetData.HasCommand(9))
                        {
                            unknown = true;
                            break;
                        }
                        if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                        {
                            lazy = true;
                            break;
                        }
                        RemovePetStatus();
                        roomUser.PetData.AddExperience(35);
                        roomUser.Statusses.Add("jmp", "");
                        roomUser.Statusses.Add("gst", "joy");
                        roomUser.UpdateNeeded = true;

                        _actionTimer = 45;
                        _energyTimer = 20;
                        SubtractAttributes();
                        break;

                    case 7:
                        if (!roomUser.PetData.HasCommand(15))
                        {
                            unknown = true;
                            break;
                        }
                        if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                        {
                            lazy = true;
                            break;
                        }
                        roomUser.FollowingOwner = roomUser;

                        RemovePetStatus();
                        switch (roomUser.RotBody)
                        {
                            case 0:
                                roomUser.MoveTo(roomUser.X + 2, roomUser.Y);
                                break;

                            case 1:
                                roomUser.MoveTo(roomUser.X - 2, roomUser.Y - 2);
                                break;

                            case 2:
                                roomUser.MoveTo(roomUser.X, roomUser.Y + 2);
                                break;

                            case 3:
                                roomUser.MoveTo(roomUser.X + 2, roomUser.Y - 2);
                                break;

                            case 4:
                                roomUser.MoveTo(roomUser.X - 2, roomUser.Y);
                                break;

                            case 5:
                                roomUser.MoveTo(roomUser.X + 2, roomUser.Y + 2);
                                break;

                            case 6:
                                roomUser.MoveTo(roomUser.X, roomUser.Y - 2);
                                break;

                            case 7:
                                roomUser.MoveTo(roomUser.X - 2, roomUser.Y + 2);
                                break;
                        }
                        roomUser.PetData.AddExperience(35);
                        roomUser.Statusses.Add("gst", "sml");
                        roomUser.UpdateNeeded = true;
                        break;

                    case 8:
                        if (!roomUser.PetData.HasCommand(4))
                        {
                            unknown = true;
                            break;
                        }
                        if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                        {
                            lazy = true;
                            break;
                        }
                        RemovePetStatus();
                        roomUser.PetData.AddExperience(11);
                        roomUser.Statusses.Add("beg", "");
                        roomUser.Statusses.Add("gst", "sml");
                        roomUser.UpdateNeeded = true;

                        _actionTimer = 25;
                        _energyTimer = 10;
                        SubtractAttributes();
                        break;

                    case 9:
                        if (!roomUser.PetData.HasCommand(10))
                        {
                            unknown = true;
                            break;
                        }
                        if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                        {
                            lazy = true;
                            break;
                        }
                        _actionTimer = 1;
                        _energyTimer = 10;
                        roomUser.Statusses.Add("gst", "sml");
                        roomUser.UpdateNeeded = true;
                        roomUser.PetData.AddExperience(35);
                        SubtractAttributes();
                        break;

                    case 10:
                        RemovePetStatus();

                        IEnumerable<RoomItem> petNest = GetRoom().GetRoomItemHandler().FloorItems.Values.Where(x => x.GetBaseItem().InteractionType == Items.Interaction.PetNest);
                        if (!petNest.Any())
                        {
                            lazy = true;
                        }
                        RoomItem roomItems = petNest.FirstOrDefault();
                        roomUser.MoveTo(roomItems.X, roomItems.Y);
                        roomUser.PetData.AddExperience(40);
                        int rndmEnergy = new Random().Next(25, 51);
                        if (roomUser.PetData.Energy < (Pet.MaxEnergy - rndmEnergy))
                        {
                            roomUser.PetData.Energy += rndmEnergy;
                        }
                        roomUser.PetData.Nutrition += 15;
                        roomUser.AddStatus("lay", "");
                        roomUser.AddStatus("gst", "eyb");
                        roomUser.UpdateNeeded = true;
                        sleeping = true;
                        _actionTimer = 500;
                        _energyTimer = 500;
                        break;

                    case 46:
                        RemovePetStatus();

                        var coord = new Point();
                        switch (roomUser.PetData.Type)
                        {
                            case 3:
                                coord = GetRoom().GetRoomItemHandler().GetRandomBreedingTerrier(roomUser.PetData);
                                break;

                            case 4:
                                coord = GetRoom().GetRoomItemHandler().GetRandomBreedingBear(roomUser.PetData);
                                break;
                        }

                        if (coord == new Point())
                        {
                            var alert = new ServerMessage(LibraryParser.OutgoingRequest("PetBreedErrorMessageComposer"));
                            alert.AppendInteger(0);
                            user.GetClient().SendMessage(alert);

                            return;
                        }

                        roomUser.MoveTo(coord);
                        roomUser.PetData.AddExperience(0);
                        roomUser.PetData.PetEnergy(true);

                        _actionTimer = 25;
                        _energyTimer = 120;
                        break;

                    default:
                        lazy = true;
                        SubtractAttributes();
                        break;
                }
                /*
                case "DESCANSA":
                case "RELAX":
                case "REST":
                case "FREE":
                this.RemovePetStatus();
                break;

                case "COMER":
                case "EAT":
                if (!roomUser.PetData.HasCommand(43))
                {
                unknown = true;
                break;
                }
                this.RemovePetStatus();
                break;

                case "SI�NTATE":
                case "SIENTATE":
                case "SIT":
                case "SIÉNTATE":
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                roomUser.PetData.AddExperience(10);
                roomUser.Statusses.Add("sit", "");
                roomUser.Statusses.Add("gst", "joy");
                roomUser.UpdateNeeded = true;
                this._actionTimer = 25;
                this._energyTimer = 10;
                this.SubtractAttributes();
                break;

                case "TUMBATE":
                case "LAY":
                case "ACUESTATE":
                case "ACUÉSTATE":
                case "DOWN":
                case "TÚMBATE":
                if (!roomUser.PetData.HasCommand(2))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                roomUser.PetData.AddExperience(10);
                roomUser.Statusses.Add("lay", "");
                roomUser.Statusses.Add("gst", "sml");
                roomUser.UpdateNeeded = true;
                this._actionTimer = 25;
                this._energyTimer = 10;
                this.SubtractAttributes();
                break;

                case "VEN":
                case "VEN AQUÍ":
                case "VEN AQU�":
                case "VEN AQUí":
                case "SÍGUEME":
                case "HERE":
                case "FOLLOW":
                if (!roomUser.PetData.HasCommand(3))
                {
                unknown = true;
                break;
                }
                if (!roomUser.PetData.HasCommand(7))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                roomUser.PetData.AddExperience(11);
                roomUser.MoveTo(user.SquareInFront);
                roomUser.Statusses.Add("gst", "sml");
                roomUser.UpdateNeeded = true;
                this._actionTimer = 25;
                this._energyTimer = 10;
                this.SubtractAttributes();
                break;

                case "LEVANTA":
                case "STAND":
                if (!roomUser.PetData.HasCommand(8))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                roomUser.PetData.AddExperience(25);
                roomUser.Statusses.Add("std", "");
                roomUser.UpdateNeeded = true;
                this._actionTimer = 25;
                this._energyTimer = 10;
                this.SubtractAttributes();
                break;

                case "JUMP":
                case "SALTA":
                case "BOTA":
                if (!roomUser.PetData.HasCommand(19))
                {
                unknown = true;
                break;
                }
                if (!roomUser.PetData.HasCommand(9))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                roomUser.PetData.AddExperience(35);
                roomUser.Statusses.Add("jmp", "");
                roomUser.Statusses.Add("gst", "joy");
                roomUser.UpdateNeeded = true;
                this._actionTimer = 45;
                this._energyTimer = 20;
                this.SubtractAttributes();
                break;

                case "ADELANTE":
                case "FORWARD":
                case "DELANTE":
                case "MOVE FORWARD":
                case "STRAIGHT":
                if (!roomUser.PetData.HasCommand(24))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                roomUser.MoveTo(roomUser.SquareInFront);
                roomUser.PetData.AddExperience(35);
                roomUser.Statusses.Add("gst", "sml");
                roomUser.UpdateNeeded = true;
                break;

                case "IZQUIERDA":
                case "FOLLOW LEFT":
                case "LEFT":
                if (!roomUser.PetData.HasCommand(15))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                switch (roomUser.RotBody)
                {
                case 0:
                roomUser.MoveTo(roomUser.X + 2, roomUser.Y);
                break;

                case 1:
                roomUser.MoveTo(roomUser.X - 2, roomUser.Y - 2);
                break;

                case 2:
                roomUser.MoveTo(roomUser.X, roomUser.Y + 2);
                break;

                case 3:
                roomUser.MoveTo(roomUser.X + 2, roomUser.Y - 2);
                break;

                case 4:
                roomUser.MoveTo(roomUser.X - 2, roomUser.Y);
                break;

                case 5:
                roomUser.MoveTo(roomUser.X + 2, roomUser.Y + 2);
                break;

                case 6:
                roomUser.MoveTo(roomUser.X, roomUser.Y - 2);
                break;

                case 7:
                roomUser.MoveTo(roomUser.X - 2, roomUser.Y + 2);
                break;
                }
                roomUser.PetData.AddExperience(35);
                roomUser.Statusses.Add("gst", "sml");
                roomUser.UpdateNeeded = true;
                break;

                case "DERECHA":
                case "FOLLOW RIGHT":
                case "RIGHT":
                if (!roomUser.PetData.HasCommand(16))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                switch (roomUser.RotBody)
                {
                case 0:
                roomUser.MoveTo(roomUser.X - 2, roomUser.Y);
                break;

                case 1:
                roomUser.MoveTo(roomUser.X + 2, roomUser.Y + 2);
                break;

                case 2:
                roomUser.MoveTo(roomUser.X, roomUser.Y - 2);
                break;

                case 3:
                roomUser.MoveTo(roomUser.X - 2, roomUser.Y + 2);
                break;

                case 4:
                roomUser.MoveTo(roomUser.X + 2, roomUser.Y);
                break;

                case 5:
                roomUser.MoveTo(roomUser.X - 2, roomUser.Y - 2);
                break;

                case 6:
                roomUser.MoveTo(roomUser.X, roomUser.Y + 2);
                break;

                case 7:
                roomUser.MoveTo(roomUser.X + 2, roomUser.Y - 2);
                break;
                }
                roomUser.PetData.AddExperience(35);
                roomUser.Statusses.Add("gst", "sml");
                roomUser.UpdateNeeded = true;
                break;

                case "PIDE":
                case "BEG":
                if (!roomUser.PetData.HasCommand(4))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                roomUser.PetData.AddExperience(11);
                roomUser.Statusses.Add("beg", "");
                roomUser.Statusses.Add("gst", "sml");
                roomUser.UpdateNeeded = true;
                this._actionTimer = 25;
                this._energyTimer = 10;
                this.SubtractAttributes();
                break;

                case "DEAD":
                case "PLAY DEAD":
                case "HAZ EL MUERTO":
                //
                if (!roomUser.PetData.HasCommand(5))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                roomUser.PetData.AddExperience(12);
                roomUser.Statusses.Add("ded", "");
                roomUser.UpdateNeeded = true;
                this._actionTimer = 25;
                this._energyTimer = 10;
                break;

                case "FUTBOL":
                case "FOOTBALL":
                case "SOCCER":
                case "FÚTBOL":
                if (!roomUser.PetData.HasCommand(5))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                var footballs = this.GetRoom().GetRoomItemHandler().MFloorItems.Values.Where(x => x.GetBaseItem().InteractionType == Items.InteractionType.football);
                if (!footballs.Any())
                {
                lazy = true;
                break;
                }
                var item = footballs.FirstOrDefault();
                this._actionTimer = 50;
                this._energyTimer = 30;
                roomUser.MoveTo(item.X, item.Y);
                roomUser.PetData.AddExperience(35);
                this.SubtractAttributes();
                break;

                case "JUEGA":
                case "JUGAR":
                case "PLAY":
                if (!roomUser.PetData.HasCommand(11))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this.RemovePetStatus();
                this.SubtractAttributes();
                break;

                case "QUIETO":
                case "CALLA":
                case "CALM":
                case "ESTATUA":
                case "STATUE":
                case "SHUT":
                case "SHUT UP":
                case "SILENCE":
                case "SILENT":
                case "STAY":
                this.RemovePetStatus();
                this._actionTimer = 650;
                this._energyTimer = 20;
                roomUser.AddStatus("wav", "");
                roomUser.UpdateNeeded = true;
                break;

                case "HABLA":
                case "SPEAK":
                case "TALK":
                if (!roomUser.PetData.HasCommand(10))
                {
                unknown = true;
                break;
                }
                if (roomUser.PetData.Energy < 20 || roomUser.PetData.Nutrition < 25)
                {
                lazy = true;
                break;
                }
                this._actionTimer = 1;
                this._energyTimer = 10;
                roomUser.Statusses.Add("gst", "sml");
                roomUser.UpdateNeeded = true;
                roomUser.PetData.AddExperience(35);
                this.SubtractAttributes();
                break;

                case "DORMIR":
                case "DUERME":
                case "A CASA":
                case "TO NEST":
                case "A DORMIR":
                case "NEST":
                this.RemovePetStatus();
                var petNest = this.GetRoom().GetRoomItemHandler().MFloorItems.Values.Where(x => x.GetBaseItem().InteractionType == Items.InteractionType.petnest);
                if (!petNest.Any())
                {
                lazy = true;
                }
                var roomItem = petNest.FirstOrDefault();
                roomUser.MoveTo(roomItem.X, roomItem.Y);
                roomUser.PetData.AddExperience(40);
                var rndmEnergy = new Random().Next(25, 51);
                if (roomUser.PetData.Energy < (Pet.MaxEnergy - rndmEnergy))
                {
                roomUser.PetData.Energy += rndmEnergy;
                }
                roomUser.PetData.Nutrition += 15;
                roomUser.AddStatus("lay", "");
                roomUser.AddStatus("gst", "eyb");
                roomUser.UpdateNeeded = true;
                sleeping = true;
                this._actionTimer = 500;
                this._energyTimer = 500;
                break;

                default:
                lazy = true;
                this.SubtractAttributes();
                break;
                }*/
            }
            catch (Exception)
            {
                lazy = true;
                SubtractAttributes();
            }

            if (sleeping)
            {
                string[] value = PetLocale.GetValue("tired");
                string message = value[new Random().Next(0, (value.Length - 1))];

                roomUser.Chat(null, message, false, 0, 0);
            }
            else if (unknown)
            {
                string[] value = PetLocale.GetValue("pet.unknowncommand");
                string message = value[new Random().Next(0, (value.Length - 1))];

                roomUser.Chat(null, message, false, 0, 0);
            }
            else if (lazy)
            {
                string[] value = PetLocale.GetValue("pet.lazy");
                string message = value[new Random().Next(0, (value.Length - 1))];

                roomUser.Chat(null, message, false, 0, 0);
            }
            else
            {
                string[] value = PetLocale.GetValue("pet.done");
                string message = value[new Random().Next(0, (value.Length - 1))];

                roomUser.Chat(null, message, false, 0, 0);
            }
        }

        /// <summary>
        /// Called when [user shout].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="message">The message.</param>
        internal override void OnUserShout(RoomUser user, string message)
        {
        }

        /// <summary>
        /// Called when [timer tick].
        /// </summary>
        internal override void OnTimerTick()
        {
            if (_speechTimer <= 0)
            {
                RoomUser roomUser = GetRoomUser();
                if (roomUser != null)
                {
                    if (roomUser.PetData.DbState != DatabaseUpdateState.NeedsInsert) roomUser.PetData.DbState = DatabaseUpdateState.NeedsUpdate;
                    var random = new Random();
                    RemovePetStatus();
                    string[] value = PetLocale.GetValue(string.Format("speech.pet{0}", roomUser.PetData.Type));
                    string text = value[random.Next(0, value.Length - 1)];
                    if (GetRoom() != null && !GetRoom().MutedPets) roomUser.Chat(null, text, false, 0, 0);
                    else roomUser.Statusses.Add(text, TextHandling.GetString(roomUser.Z));
                }
                _speechTimer = Plus.GetRandomNumber(20, 120);
            }
            else _speechTimer--;
            if (_actionTimer <= 0 && GetRoomUser() != null)
            {
                try
                {
                    _actionTimer = GetRoomUser().FollowingOwner != null
                        ? 2
                        : Plus.GetRandomNumber(15, 40 + GetRoomUser().PetData.VirtualId);
                    RemovePetStatus();
                    _actionTimer = Plus.GetRandomNumber(15, 40 + GetRoomUser().PetData.VirtualId);
                    if (GetRoomUser().RidingHorse != true)
                    {
                        RemovePetStatus();

                        if (GetRoomUser().FollowingOwner != null)
                        {
                            GetRoomUser().MoveTo(GetRoomUser().FollowingOwner.SquareBehind);
                        }
                        else
                        {
                            if (GetRoomUser().PetData.Type == 16) return; //Monsterplants can't move
                            var nextCoord = GetRoom().GetGameMap().GetRandomValidWalkableSquare();
                            GetRoomUser().MoveTo(nextCoord.X, nextCoord.Y);
                        }
                    }
                    if (new Random().Next(2, 15) % 2 == 0)
                    {
                        if (GetRoomUser().PetData.Type == 16)
                        {
                            MoplaBreed breed = GetRoomUser().PetData.MoplaBreed;
                            GetRoomUser().PetData.Energy--;
                            GetRoomUser().AddStatus("gst", (breed.LiveState == MoplaState.Dead) ? "sad" : "sml");
                            GetRoomUser()
                                .PetData.MoplaBreed.OnTimerTick(GetRoomUser().PetData.LastHealth,
                                    GetRoomUser().PetData.UntilGrown);
                        }
                        else
                        {
                            if (GetRoomUser().PetData.Energy < 30) GetRoomUser().AddStatus("lay", "");
                            else
                            {
                                GetRoomUser().AddStatus("gst", "joy");
                                if (new Random().Next(1, 7) == 3) GetRoomUser().AddStatus("snf", "");
                            }
                        }
                        GetRoomUser().UpdateNeeded = true;
                    }
                    goto IL_1B5;
                }
                catch (Exception pException)
                {
                    Logging.HandleException(pException, "PetBot.OnTimerTick");
                    goto IL_1B5;
                }
            }
            _actionTimer--;
            IL_1B5:
            if (_energyTimer <= 0)
            {
                RemovePetStatus();
                var roomUser2 = GetRoomUser();
                if (roomUser2 != null) roomUser2.PetData.PetEnergy(true);
                _energyTimer = Plus.GetRandomNumber(30, 120);
                return;
            }
            _energyTimer--;
        }

        /// <summary>
        /// Removes the pet status.
        /// </summary>
        private void RemovePetStatus()
        {
            RoomUser roomUser = GetRoomUser();

            if (roomUser == null) return;
            roomUser.Statusses.Clear();
            roomUser.UpdateNeeded = true;
        }

        /// <summary>
        /// Subtracts the attributes.
        /// </summary>
        private void SubtractAttributes()
        {
            RoomUser roomUser = GetRoomUser();
            if (roomUser == null) return;

            if (roomUser.PetData.Energy < 11)
                roomUser.PetData.Energy = 0;
            else
                roomUser.PetData.Energy -= 10;
            if (roomUser.PetData.Nutrition < 6)
                roomUser.PetData.Nutrition = 0;
            else
                roomUser.PetData.Nutrition -= 5;
        }
    }
}