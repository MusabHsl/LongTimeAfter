using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator playerAC;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    // ManageAnimations PlayerMovement tarafindan cagrilacagi icin Update'e gerek yok.

    public void ManageAnimations(float speed)
    {
        if (speed > 0)
        {
            PlayRunAnimaton();
        }
        else
        {
            PlayIdleAnimation();
        }
    }

    public void PlayRunAnimaton()
    {
        playerAC.Play("RUN");
    }

    public void PlayIdleAnimation()
    {
        playerAC.Play("IDLE");
    } 
}
