using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimationsController : MonoBehaviour
{
   public Animator animator;

   public void PlayAnimation(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }
}
