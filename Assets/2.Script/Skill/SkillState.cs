using System.Collections;

public abstract class SkillState
{
    #region Constructor

    public SkillState(Skill stateController, Character character) { }

    #endregion Constructor

    #region Methods

    public virtual void OnPressed() { }
    public virtual void OnReleased() { }

    public abstract IEnumerator Activate();
    public virtual void Cancel() { }
    public virtual void Clear() { }

    #endregion Methods
}