using UnityEngine;

public class Dungeon : MonoBehaviour
{
    #region Variables

    [SerializeField] private Room startingRoom = null;

    #endregion Variables

    #region Properties

    public Room[] Rooms { private set; get; }

    #endregion Properties

    #region Unity Events

    private void Awake()
    {
        Rooms = GetComponentsInChildren<Room>();
    }

    private void Start()
    {
        foreach (Room room in Rooms)
        {
            room.gameObject.SetActive(false);
        }

        startingRoom.gameObject.SetActive(true);
        startingRoom.OnEnableRoom();
    }

    #endregion Unity Events
}
