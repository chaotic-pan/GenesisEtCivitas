using UnityEngine;
public class Dance_random : StateMachineBehaviour
{
    [SerializeField]
    private int numberOfDances;
    private int danceAnim;
    
     // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        danceAnim = Random.Range(0, numberOfDances);
        // Debug.Log(danceAnim);
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("DanceRandomizer", danceAnim, 0.2f, Time.deltaTime);
    }


}
