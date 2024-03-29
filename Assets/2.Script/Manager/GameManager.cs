using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    #region Variables

    [Header("Core Manager")]
    private static InputManager inputManager = new();
    private static ResourceManager resourceManager = new();

    [Header("Content Manager")]
    private static ObjectPoolManager objectPoolManager = null;
    private static EffectManager effectManager = null;
    private static CameraManager cameraManager = null;

    // Debug
    [SerializeField] private Room room = null;

    #endregion Variables

    #region Properties

    public static InputManager Input => inputManager;
    public static ResourceManager Resource => resourceManager;

    public static ObjectPoolManager ObjectPool => objectPoolManager;
    public static EffectManager Effect => effectManager;
    public static CameraManager Camera => cameraManager;

    // Debug
    public static Room Room => Instance.room;

    #endregion Properties

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        objectPoolManager = FindObjectOfType<ObjectPoolManager>();
        effectManager = FindObjectOfType<EffectManager>();
        cameraManager = FindObjectOfType<CameraManager>();

        inputManager.Init();
        resourceManager.Init();

        objectPoolManager.Init();
        effectManager.Init();
    }

    #endregion Unity Events
}
