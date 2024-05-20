using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    #region Variables

    [SerializeField] private Room currentRoom = null;
    [SerializeField] private Portal connectedPortal = null;

    private float enableTime = 0.5f;
    private float curTime = 0f;

    private bool isEnabled = false;

    private Action<Portal> enteredPortalEvent = null;

    #endregion Variables

    #region Properties

    public DNFTransform DNFTransform { private set; get; }

    public HitboxController HitboxController { private set; get; }

    public event Action<Portal> EnteredPortalEvent
    {
        add
        {
            enteredPortalEvent += value;
        }
        remove
        {
            enteredPortalEvent -= value;
        }
    }

    #endregion Properties

    #region Unity Events

    private void Awake()
    {
        DNFTransform = GetComponent<DNFTransform>();
        HitboxController = GetComponent<HitboxController>();

        HitboxController.Init(DNFTransform);

        EnteredPortalEvent += (Portal connectedPortal) => 
        {
            currentRoom.gameObject.SetActive(false);

            connectedPortal.currentRoom.gameObject.SetActive(true);
            connectedPortal.currentRoom.OnEnableRoom();
        };
    }

    private void OnEnable()
    {
        isEnabled = true;
        HitboxController.EnableHitbox(0);
    }

    private void OnDisable()
    {
        HitboxController.DisableHitbox();
    }

    private void Update()
    {
        if (CheckEnterPortal() && !isEnabled)
        {
            curTime += Time.deltaTime;

            if (curTime > enableTime)
            {
                curTime = 0f;
                enteredPortalEvent?.Invoke(connectedPortal);
            }
        }
    }

    #endregion Unity Events

    #region Methods

    protected virtual bool CheckEnterPortal()
    {
        foreach (IDamagable character in currentRoom.CharacterList)
        {
            if (!HitboxController.CheckCollision(character.DefenderHitboxController))
            {
                isEnabled = false;
                curTime = 0f;

                return false;
            }
        }

        return true;
    }

    #endregion Methods
}
