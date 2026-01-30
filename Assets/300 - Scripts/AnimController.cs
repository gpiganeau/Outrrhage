using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimController: MonoBehaviour
{
    private Animator animator;

    ActorSetupData data;

    bool locked = false;

    public void Initialize(ActorSetupData data)
    {
        this.data = data;
        
        if (TryGetComponent<Animator>(out var a)){
            animator = a;
        }

        locked = false;
    }

    public void SetSpeed(float speed)
    {
        if (locked) return;
        animator.SetFloat("Speed", speed);
    }


    public void Attack()
    {
        if (locked) return;

        animator.SetTrigger("Slash");
    }

    public void Cast()
    {
        if (locked) return;

        animator.SetTrigger("Cast");
    }

    public void Hit()
    {
        if (locked) return;

        animator.SetTrigger("Hit");
    }

    public void Die()
    {
        if (locked) return;
        locked = true;
        animator.SetTrigger("Death");
    }

    public float ClipLength(string clip) => animator.GetClipLength(clip);
}