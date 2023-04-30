using System.Threading.Tasks;

public interface IRoomOccupant
{
    public Task<bool> OnPlayerEnterRoom(MimicGuy guy);
    public abstract void OnRoomIdChange(int x, int y);
}
