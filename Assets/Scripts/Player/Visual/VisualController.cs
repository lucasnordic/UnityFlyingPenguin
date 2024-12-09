using System.Collections;
using UnityEngine;

public class VisualController : MonoBehaviour
{
    [SerializeField] private GameObject visualsObject;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public bool IsFacingLeft => spriteRenderer.flipX;

    void Awake()
    {
        animator = visualsObject.GetComponent<Animator>();
        spriteRenderer = visualsObject.GetComponent<SpriteRenderer>();
    }

    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }

    public void PlaySlideSequence()
    {
        StartCoroutine(SlideSequence());
    }

    private IEnumerator SlideSequence()
    {
        PlayAnimation("penguin_preslide");
        yield return new WaitForSeconds(0.1f);
        PlayAnimation("penguin_slide");
    }

    public void FlipSprite(bool flip)
    {
        spriteRenderer.flipX = flip;
    }

    public string GetCurrentAnimation()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("penguin_idle")) return "penguin_idle";
        if (state.IsName("penguin_walk")) return "penguin_walk";
        if (state.IsName("penguin_jump")) return "penguin_jump";
        if (state.IsName("penguin_slide")) return "penguin_slide";
        if (state.IsName("penguin_preslide")) return "penguin_preslide";
        if (state.IsName("penguin_attack")) return "penguin_attack";
        if (state.IsName("penguin_land")) return "penguin_land";

        return "";
    }
}