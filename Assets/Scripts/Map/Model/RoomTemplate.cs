using UnityEngine;

[SerializeField]
public class RoomTemplate : MonoBehaviour
{
    [SerializeField] private string name;
    [SerializeField] private int numberOfRooms;
    [SerializeField] private int roomWidthMin = 3;
    [SerializeField] private int roomWidthMax = 6;
    [SerializeField] private int roomHeightMin = 3;
    [SerializeField] private int roomHeightMax = 6;
    [SerializeField] private Texture2D layoutTexture;
    public int NumberOfRooms => numberOfRooms;
    public int RoomWidthMin => roomWidthMin;
    public int RoomWidthMax => roomWidthMax;
    public int RoomHeightMin => roomHeightMin;
    public int RoomHeightMax => roomHeightMax;
    public Texture2D LayoutTexture => layoutTexture;

    public RectInt GenerateRoomCandidateRect()
    {
        var randomService = RandomSingleton.Instance;

        if (layoutTexture == null)
        {
            return new RectInt
            {
                width = randomService.NextInt(roomWidthMin, roomWidthMax),
                height = randomService.NextInt(roomHeightMin, roomHeightMax)
            };
        }
        else
        {
            return new RectInt
            {
                width = layoutTexture.width,
                height = layoutTexture.height,
            };
        }
    }
}
