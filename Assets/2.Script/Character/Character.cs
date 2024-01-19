using UnityEngine;

public class Character : BehaviourController
{
    #region Variables

    [Header("Character behaviours")]
    private JumpBehaviour jumpBehaviour = null;
    private AttackBehaviour attackBehaviour = null;

    #endregion Variables

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        jumpBehaviour = GetComponent<JumpBehaviour>();
        attackBehaviour = GetComponent<AttackBehaviour>();

        behaviourDictionary.Add(BehaviourCodeList.JUMP_BEHAVIOUR_CODE, jumpBehaviour);
        behaviourDictionary.Add(BehaviourCodeList.ATTACK_BEHAVIOUR_CODE, attackBehaviour);
    }

    private void Start()
    {
        // Debug
        Camera.main.GetComponent<CameraFollow>().SetTarget(transform);
        attackBehaviour.RegisterSkill(EKeyName.BASEATTACK, FindObjectOfType<BaseAttack_FireHero>());
        attackBehaviour.RegisterSkill(EKeyName.SKILL1, FindObjectOfType<Meteor_FireHero>());
        attackBehaviour.RegisterSkill(EKeyName.SKILL2, FindObjectOfType<ScatterFlame_FireHero>());
        attackBehaviour.RegisterSkill(EKeyName.SKILL3, FindObjectOfType<Dash_FireHero>());
        attackBehaviour.RegisterSkill(EKeyName.SKILL4, FindObjectOfType<FlameStrike_FireHero>());

        GameManager.Input.AddMovementDelegate(OnJoystickMoved);

        GameManager.Input.AddButtonDownDelegate(EKeyName.JUMP, OnJumpButtonPressed);

        GameManager.Input.AddButtonDownDelegate(EKeyName.BASEATTACK, OnAttackButtonPressed);
        GameManager.Input.AddButtonUpDelegate(EKeyName.BASEATTACK, OnAttackButtonReleased);

        GameManager.Input.AddButtonDownDelegate(EKeyName.SKILL1, () => OnSkillButtonPressed(EKeyName.SKILL1));
        GameManager.Input.AddButtonUpDelegate(EKeyName.SKILL1, () => OnSkillButtonReleased(EKeyName.SKILL1));
        GameManager.Input.AddButtonDownDelegate(EKeyName.SKILL2, () => OnSkillButtonPressed(EKeyName.SKILL2));
        GameManager.Input.AddButtonUpDelegate(EKeyName.SKILL2, () => OnSkillButtonReleased(EKeyName.SKILL2));
        GameManager.Input.AddButtonDownDelegate(EKeyName.SKILL3, () => OnSkillButtonPressed(EKeyName.SKILL3));
        GameManager.Input.AddButtonUpDelegate(EKeyName.SKILL3, () => OnSkillButtonReleased(EKeyName.SKILL3));
        GameManager.Input.AddButtonDownDelegate(EKeyName.SKILL4, () => OnSkillButtonPressed(EKeyName.SKILL4));
        GameManager.Input.AddButtonUpDelegate(EKeyName.SKILL4, () => OnSkillButtonReleased(EKeyName.SKILL4));
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

        jumpBehaviour.Jump();
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
