
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public FixedJoystick joystick;
    public string moveFBAixsName = "Vertical";
    public string moveLRAixsName = "Horizontal";
    public string attackButtonName = "Attack";
    public string firstSkillButtonName = "Skill_F";
    public string secondSkillButtonName = "Skill_S";

    public float moveFB { get; private set; }
    public float moveLR { get; private set; }
    public bool attack { get; private set; }
    public bool firstSkill { get; private set; }
    public bool secondSkill { get; private set; }
    public bool evade { get; private set; }

    public void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            moveFB = 0;
            moveLR = 0;
            attack = false;
            firstSkill = false;
            secondSkill = false;

            return;
        }

        moveFB = joystick.Vertical + Input.GetAxis(moveFBAixsName);
        moveLR = joystick.Horizontal + Input.GetAxis(moveLRAixsName);
        attack = Input.GetKeyDown(KeyCode.L) || UIManager.Instance.uis[2].GetComponent<DungeonUi>().attack.ButtonPressed; 
        firstSkill = Input.GetKeyDown(KeyCode.K) || UIManager.Instance.uis[2].GetComponent<DungeonUi>().fSkill.ButtonPressed; 
        secondSkill = Input.GetKeyDown(KeyCode.O) || UIManager.Instance.uis[2].GetComponent<DungeonUi>().sSkill.ButtonPressed;
        evade = Input.GetKeyDown(KeyCode.Space) || UIManager.Instance.uis[2].GetComponent<DungeonUi>().evade.ButtonPressed;
    }
    
}
