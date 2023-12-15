using System.Collections;
using UnityEngine;

public class BaseAttack_FireHero : Skill
{
    private int skillHash = 0;

    private bool isBlockKey = true;
    private bool isContinue = false;

    public override void InitSkill(Character character, Animator animator)
    {
        base.InitSkill(character, animator);

        skillHash = Animator.StringToHash(AnimatorKey.Character.FireHero.BASE_ATTACK);
    }

    public override void OnPressed()
    {
        if (isBlockKey) return;

        isContinue = true;
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return activeSkill == null;
    }

    public override IEnumerator ActivateSkill()
    {
        int curCombo = 0, maxCombo = 2;

        character.CanMove = false;
        character.CanJump = false;

        while (curCombo < maxCombo)
        {
            animator.SetTrigger(skillHash);

            // Pre-delay
            yield return Utilities.WaitForSeconds(0.1f);

            // Enable to get key input
            isBlockKey = false;

            // Instantiate the projectile

            // Post-delay
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            if (!isContinue)
            {
                Clear();
                yield break;
            }

            isBlockKey = true;
            isContinue = false;

            curCombo++;
        }

        animator.SetTrigger(skillHash);

        // Pre-delay
        yield return Utilities.WaitForSeconds(0.15f);

        // Instantiate the projectile

        // Post-delay
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        character.CanMove = true;
        character.CanJump = true;
    }

    public override void Clear()
    {
        isBlockKey = true;
        isContinue = false;

        character.CanMove = true;
        character.CanJump = true;
    }
}
