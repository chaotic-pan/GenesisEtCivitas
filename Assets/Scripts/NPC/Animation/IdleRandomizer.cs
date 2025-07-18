using UnityEngine;
public class AnimationRandomizer : StateMachineBehaviour
{
    [SerializeField]
    private int numberOfAnims;
    [SerializeField]
    private string randomizer;
    [SerializeField]
    private string delay;
    
     // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(randomizer, Random.Range(0, numberOfAnims));
        
        animator.SetFloat(delay, (float)Random.Range(20, 100)/100);
        
    }

}
