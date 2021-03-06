using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items.Interactor
{
    internal class InteractorFireworks : IFurniInteractor
    {
        public void OnPlace(GameClient session, RoomItem item)
        {
            item.ExtraData = "1";
        }

        public void OnRemove(GameClient session, RoomItem item)
        {
            item.ExtraData = "1";
        }

        public void OnTrigger(GameClient session, RoomItem item, int request, bool hasRights)
        {
            if (item.ExtraData == "" || item.ExtraData == "0")
            {
                item.ExtraData = "1";
                item.UpdateState();
                return;
            }
            if (item.ExtraData == "1")
            {
                item.ExtraData = "2";
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