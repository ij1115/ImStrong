using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string moveFBAixsName = "Vertical";
    public string moveLRAixsName = "Horizontal";
    public string attackButtonName = "Attack";
    public string firstSkillButtonName = "Skill_F";
    public string secondSkillButtonName = "Skill_S";

    public float moveFB { get; private set; }
    public float moveLR { get; private set; }
    public bool attack {  get; private set; }
    public bool firstSkill { get; private set; }
    public bool secondSkill { get; private set; }


    public void Update()
    {

        if(GameManager.instance !=null &&GameManager.instance.isGameover)
        {
            moveFB = 0;
            moveLR = 0;
            attack = false;
            firstSkill = false;
            secondSkill = false;

            return;
        }

        moveFB = Input.GetAxis(moveFBAixsName);
        moveLR = Input.GetAxis(moveLRAixsName);
        attack = Input.GetButtonDown(attackButtonName);
        firstSkill = Input.GetButtonDown(firstSkillButtonName);
        secondSkill = Input.GetButtonDown(secondSkillButtonName);
    }
}
