using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plus.HabboHotel.Roleplay.Misc;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Roleplay.Timers
{
    public class ATMRobTimer
    {
        Timer timer;
        GameClient Session;
        int timeLeft; // 5 minutes (milliseconds)

        public ATMRobTimer(GameClient Session)
        {
            this.Session = Session;

            int time = 5;
            timeLeft = time * 60000;

            startTimer();
        }

        public void startTimer()
        {
            TimerCallback timerFinished = timerDone;

            timer = new Timer(timerFinished, null, 60000, Timeout.Infinite);
        }

        public void timerDone(object info)
        {
            try
            {
                timeLeft -= 60000;

                #region Conditions
                if (Session == null)
                { stopTimer(); return; }

                if (timeLeft == 4 * 60000)
                {
                    Session.Shout("*Repeatedly smashes the metal casing of ATM, dealing a small dent to it*");
                }
                else if (timeLeft == 3 * 60000)
                {
                    Session.Shout("*Manages to form a medium sized dent in the casing of the ATM*");
                }
                else if (timeLeft == 2 * 60000)
                {
                    Session.Shout("*Forms a narrow opening in the ATM, and begins to repeatedly pry it open with their crowbar*");
                }
                else if (timeLeft == 1 * 60000)
                {
                    Session.Shout("*Pushes with all their might as the at narrow opening, further widening it*");
                }
                if (timeLeft > 0)
                {
                    int minutesRemaining = timeLeft / 60000;
                    Session.SendWhisper("You have " + minutesRemaining + " minutes until you rob the ATM!");
                    timer.Change(60000, Timeout.Infinite);
                    return;
                }

                #endregion

                #region Execute
                Random rnd = new Random();
                int money = rnd.Next(0, 400);

                RoleplayManager.Shout(Session, "*Successfully robs the ATM [+$" + money + "]*");
                RoleplayManager.GiveMoney(Session, +money);
                Session.GetRoleplay().ATMRobbery = false;

                stopTimer();
                #endregion
            }
            catch { stopTimer(); }
        }

        public int getTime()
        {
            int minutesRemaining = timeLeft / 60000;
            return minutesRemaining;
        }

        public void stopTimer()
        {
            timer.Dispose();
        }
    }
}
