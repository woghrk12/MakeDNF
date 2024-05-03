using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    #region Properties

    public IDamagable[] Players { private set; get; }
    public IDamagable[] Monsters { private set; get; }
    public IDamagable[] BossMonsters { private set; get; }
    public IDamagable[] NamedMonsters { private set; get; }
    public IDamagable[] NormalMonsters { private set; get; }

    public Vector3 MaxXZPos { private set; get; }
    public Vector3 MinXZPos { private set; get; }

    public Vector3 CameraTopRight { private set; get; }
    public Vector3 CameraBottomLeft { private set; get; }

    #endregion Properties

    #region Unity Events

    private void Awake()
    {
        Players = FindObjectsOfType<Character>();

        var monsterParent = transform.Find("Monsters");
        var monsterList = new List<IDamagable>();
        foreach (Transform child in monsterParent)
        {
            IDamagable damagableObject = child.GetComponent<IDamagable>();
            monsterList.Add(damagableObject);
        }

        Monsters = ArrayHelper.ListToArray(monsterList);

        CameraTopRight = transform.Find("CameraTopRight").position;
        CameraBottomLeft = transform.Find("CameraBottomLeft").position;

        Vector3 topRight = transform.Find("TopRight").position;
        Vector3 bottomLeft = transform.Find("BottomLeft").position;

        MaxXZPos = new Vector3(topRight.x, 0f, topRight.y * GlobalDefine.CONV_RATE);
        MinXZPos = new Vector3(bottomLeft.x, 0f, bottomLeft.y * GlobalDefine.CONV_RATE);
    }

    private void Start()
    {
        GameManager.Camera.SetCameraRange(CameraTopRight, CameraBottomLeft);
    }

    #endregion Unity Events
}
