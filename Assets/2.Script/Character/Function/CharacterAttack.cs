using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    #region Variables

    #endregion Variables

    #region Methods

    public void UseSkill(EKeyName keyName)
    {
        Debug.Log(keyName + " Skill use.");
    }

    public void ReleaseSkill(EKeyName keyName)
    {
        Debug.Log(keyName + " Skill release.");
    }

    #endregion Methods
}
