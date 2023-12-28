using UnityEngine;

public class Character : BehaviourController
{
    #region Variables
    
    [Header("Character behaviours")]
    private AttackBehaviour attackBehaviour = null;

    #endregion Variables

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        attackBehaviour = GetComponent<AttackBehaviour>();

        behaviourDictionary.Add(BehaviourCodeList.attackBehaviourCode, attackBehaviour);
    }

    private void Start()
    {
        // Debug
        Camera.main.GetComponent<CameraFollow>().SetTarget(transform);
        attackBehaviour.RegisterSkill(EKeyName.BASEATTACK, FindObjectOfType<BaseAttack_FireHero>());
        attackBehaviour.RegisterSkill(EKeyName.SKILL1, FindObjectOfType<Meteor_FireHero>());
        attackBehaviour.RegisterSkill(EKeyName.SKILL2, FindObjectOfType<ScatterFlame_FireHero>());

        GameManager.Input.SetMovementDelegate(OnJoystickMoved);

        GameManager.Input.SetButtonDelegate(EKeyName.JUMP, OnJumpButtonPressed);

        GameManager.Input.SetButtonDelegate(EKeyName.BASEATTACK, OnAttackButtonPressed, OnAttackButtonReleased);

        GameManager.Input.SetButtonDelegate(EKeyName.SKILL1,
            () => OnSkillButtonPressed(EKeyName.SKILL1),
            () => OnSkillButtonReleased(EKeyName.SKILL1));
        GameManager.Input.SetButtonDelegate(EKeyName.SKILL2,
            () => OnSkillButtonPressed(EKeyName.SKILL2),
            () => OnSkillButtonReleased(EKeyName.SKILL2));
        GameManager.Input.SetButtonDelegate(EKeyName.SKILL3,
            () => OnSkillButtonPressed(EKeyName.SKILL3),
            () => OnSkillButtonReleased(EKeyName.SKILL3));
        GameManager.Input.SetButtonDelegate(EKeyName.SKILL4,
            () => OnSkillButtonPressed(EKeyName.SKILL4),
            () => OnSkillButtonReleased(EKeyName.SKILL4));
    }

    #endregion Unity Events

    #region Event Methods

    public void OnJoystickMoved(Vector3 direction)
    {
        if (!CanMove) return;

        moveBehaviour.Move(direction);
    }

    public void OnJumpButtonPressed()
    {
        if (!CanJump) return;

        moveBehaviour.Jump();
    }

    public void OnAttackButtonPressed()
    {
        attackBehaviour.OnSkillButtonPressed(EKeyName.BASEATTACK);
        
        if (!attackBehaviour.CheckCanAttack(EKeyName.BASEATTACK)) return;

        attackBehaviour.Attack(EKeyName.BASEATTACK);
    }

    public void OnAttackButtonReleased()
    {
        attackBehaviour.OnSkillButtonReleased(EKeyName.BASEATTACK);
    }

    public void OnSkillButtonPressed(EKeyName keyName)
    {
        attackBehaviour.OnSkillButtonPressed(keyName);
        
        if (!attackBehaviour.CheckCanAttack(keyName)) return;

        attackBehaviour.Attack(keyName);
    }

    public void OnSkillButtonReleased(EKeyName keyName)
    {
        attackBehaviour.OnSkillButtonReleased(keyName);
    }

    #endregion Event Methods
}
