using System.Threading.Tasks;

public interface IRoomOccupant
{
    public Task<bool> OnPlayerEnterRoom(MimicGuy guy);
    public void OnRoomIdChange(int x, int y);
    public void OnRoomRotate(bool clockwise);
}
