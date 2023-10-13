using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

//public class PlayerController : MonoBehaviour, IDragHandler, IEndDragHandler
//{
//    private bool inputMove = true;

//    private RectTransform joystickArea;
//    [SerializeField] private RectTransform joystickBackGround;
//    [SerializeField] private RectTransform joystickHandle;

//    private float joystickMaxValue;
//    private float joystickMaxValueInverse;

//    private bool isDragging = false;
//    private bool isKeyboardDown = true;

//    private Vector2 axis;

//    public UnityEvent<Vector2> onDragEvent;

//    private void Awake()
//    {
//        joystickArea = GetComponent<RectTransform>();
//        joystickMaxValue = Mathf.Min(joystickBackGround.rect.width, joystickBackGround.rect.height);
//        joystickMaxValue *= 0.5f;
//        joystickMaxValueInverse = 1.0f/ joystickMaxValue;
//    }

//    private void Update()
//    {
//        Vector2 joystickAxis = axis;
//        if(inputMove)
//        {
//            if(isKeyboardDown && !isDragging)
//            {
//                joystickAxis.x += Input.GetAxis("Horizontal");
//                joystickAxis.y += Input.GetAxis("Vertical");
//                joystickAxis.Normalize();
//                onDragEvent.Invoke(joystickAxis);
//            }
//            joystickHandle.anchoredPosition = joystickAxis * joystickMaxValue;
//        }
//    }

//    public void OnDrag(PointerEventData eventData)
//    {
//        isDragging = true;
//        Vector2 inputPosition = eventData.position;
//        Vector2 joystickCenter = joystickBackGround.position;

//        Vector2 normal = (inputPosition - joystickCenter).normalized;

//        float magnitude = (inputPosition - joystickCenter).magnitude;
//        magnitude = Mathf.Clamp(magnitude, -joystickMaxValue, joystickMaxValue);

//        if(isKeyboardDown)
//        {
//            normal.x += Input.GetAxis("Horizontal");
//            normal.y += Input.GetAxis("Vertical");
//            normal.Normalize();
//        }

//        axis = joystickMaxValueInverse * magnitude * normal;
//        onDragEvent.Invoke(axis);
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        isDragging = false;
//        axis = Vector2.zero;
//        onDragEvent.Invoke(axis);
//    }
//}

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

        moveFB =  Input.GetAxis(moveFBAixsName);
        moveLR =  Input.GetAxis(moveLRAixsName);
        attack = Input.GetKeyDown(KeyCode.L); //Input.GetButtonDown(attackButtonName) ||
        firstSkill = Input.GetKeyDown(KeyCode.K); //Input.GetButtonDown(firstSkillButtonName) ||
        secondSkill = Input.GetKeyDown(KeyCode.O); //Input.GetButtonDown(secondSkillButtonName) ||
        evade = Input.GetKeyDown(KeyCode.Space);
    }
}
