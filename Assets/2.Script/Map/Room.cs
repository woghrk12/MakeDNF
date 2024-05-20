using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    #region Properties

    public List<IDamagable> CharacterList { private set; get; } = new List<IDamagable>();

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
        var characters = FindObjectsOfType<Character>();
        foreach (Character character in characters)
        {
            CharacterList.Add(character.GetComponent<IDamagable>());
        }

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

        MaxXZPos = new Vector3(topRight.x, 0f, topRight.y * GlobalDefine.INV_CONV_RATE);
        MinXZPos = new Vector3(bottomLeft.x, 0f, bottomLeft.y * GlobalDefine.INV_CONV_RATE);

        Transform portals = transform.Find("Portals");
        foreach (Transform child in portals)
        {
            var portal = child.GetComponent<Portal>();
            portal.EnteredPortalEvent += (Portal portal) =>
            {
                foreach (IDamagable character in CharacterList)
                {
                    character.DefenderDNFTransform.Position = portal.DNFTransform.Position;
                }
            };
        }
    }

    #endregion Unity Events

    #region Methods

    public void OnEnableRoom()
    { 
        GameManager.Camera.SetCameraRange(CameraTopRight, CameraBottomLeft);
    }

    #endregion Methods
}
