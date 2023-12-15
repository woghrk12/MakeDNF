using System.Collections;
using UnityEngine;

public class BaseAttack_FireHero : Skill
{
    private int skillHash = 0;

    private bool isBlockKey = true;
    private bool isContinue = false;

    public override void InitSkill(Animator animator)
    {
        base.InitSkill(animator);

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

        while (curCombo < maxCombo)
        {
            characterAnimator.SetTrigger(skillHash);

            // Pre-delay
            yield return Utilities.WaitForSeconds(0.1f);

            // Enable to get key input
            isBlockKey = false;

            // Instantiate the projectile

            // Post-delay
            yield return new WaitUntil(() => characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            if (!isContinue)
            {
                Clear();
                yield break;
            }

            isBlockKey = true;
            isContinue = false;

            curCombo++;
        }

        characterAnimator.SetTrigger(skillHash);

        // Pre-delay
        yield return Utilities.WaitForSeconds(0.15f);

        // Instantiate the projectile

        // Post-delay
        yield return new WaitUntil(() => characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    }

    public override void Clear()
    {
        isBlockKey = true;
        isContinue = false;
    }
}
