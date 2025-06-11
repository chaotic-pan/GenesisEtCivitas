using Player;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private UISkillTree uiSkillTree;

    private void Start()
    {
        uiSkillTree.SetPlayerSkills(playerController.GetPlayerSkillSet());
    }
}
