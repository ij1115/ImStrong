using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class JoystickController : MonoBehaviour, IDragHandler, IEndDragHandler
{ 
        private bool inputMove = true;

        private RectTransform joystickArea;
        [SerializeField] private RectTransform joystickBackGround;
        [SerializeField] private RectTransform joystickHandle;

        private float joystickMaxValue;
        private float joystickMaxValueInverse;

        private bool isDragging = false;
        private bool isKeyboardDown = true;

        private Vector2 axis;

        public UnityEvent<Vector2> onDragEvent;

        private void Awake()
        {
            joystickArea = GetComponent<RectTransform>();
            joystickMaxValue = Mathf.Min(joystickBackGround.rect.width, joystickBackGround.rect.height);
            joystickMaxValue *= 0.5f;
            joystickMaxValueInverse = 1.0f / joystickMaxValue;
        }

        private void Update()
        {
            Vector2 joystickAxis = axis;
            if (inputMove)
            {
                if (isKeyboardDown && !isDragging)
                {
                    joystickAxis.x += Input.GetAxis("Horizontal");
                    joystickAxis.y += Input.GetAxis("Vertical");
                    joystickAxis.Normalize();
                    onDragEvent.Invoke(joystickAxis);
                }
                joystickHandle.anchoredPosition = joystickAxis * joystickMaxValue;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            isDragging = true;
            Vector2 inputPosition = eventData.position;
            Vector2 joystickCenter = joystickBackGround.position;

            Vector2 normal = (inputPosition - joystickCenter).normalized;

            float magnitude = (inputPosition - joystickCenter).magnitude;
            magnitude = Mathf.Clamp(magnitude, -joystickMaxValue, joystickMaxValue);

            if (isKeyboardDown)
            {
                normal.x += Input.GetAxis("Horizontal");
                normal.y += Input.GetAxis("Vertical");
                normal.Normalize();
            }

            axis = joystickMaxValueInverse * magnitude * normal;
            onDragEvent.Invoke(axis);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            axis = Vector2.zero;
            onDragEvent.Invoke(axis);
        }
    }