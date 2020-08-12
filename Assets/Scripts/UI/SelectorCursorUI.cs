using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorCursorUI : MonoBehaviour {
    [SerializeField]
    private static int _currentSelected;
    public int CurrentSelected {
        get {
            return _currentSelected;
        }
        set {
            _currentSelected = value;
            targetPos = defaultPos;
            targetPos.x += deltaPos * _currentSelected;
        }
    }

    [SerializeField]
    private int deltaPos = 100;
    [SerializeField]
    private float timeToMove = 1f;
    private Vector2 defaultPos = Vector2.one * -1;
    private Vector2 targetPos;
    private Vector2 previousPos;
    private RectTransform rectTransform;

    private void Awake () {
        rectTransform = GetComponent<RectTransform> ();
        if (defaultPos == Vector2.one * -1)
            defaultPos = rectTransform.anchoredPosition;
        previousPos = defaultPos;
        targetPos = defaultPos;
    }

    private void Update () {
        if (previousPos.x != targetPos.x) {
            Vector2 newPos = rectTransform.anchoredPosition;
            bool positiveMove = targetPos.x - previousPos.x > 0;
            newPos.x += (targetPos.x - previousPos.x) * Time.deltaTime / timeToMove;
            if ((positiveMove && newPos.x >= targetPos.x) || (!positiveMove && newPos.x <= targetPos.x)) {
                newPos = targetPos;
                previousPos = targetPos;
            }
            rectTransform.anchoredPosition = newPos;
        }
        if (Input.GetKeyDown (KeyCode.Keypad1) || Input.GetKeyDown (KeyCode.Alpha1)) {
            CurrentSelected = 0;
        }
        if (Input.GetKeyDown (KeyCode.Keypad2) || Input.GetKeyDown (KeyCode.Alpha2)) {
            CurrentSelected = 1;
        }
        if (Input.GetKeyDown (KeyCode.Keypad3) || Input.GetKeyDown (KeyCode.Alpha3)) {
            CurrentSelected = 2;
        }
        if (Input.GetKeyDown (KeyCode.Keypad4) || Input.GetKeyDown (KeyCode.Alpha4)) {
            CurrentSelected = 3;
        }
        if (Input.GetKeyDown (KeyCode.Keypad5) || Input.GetKeyDown (KeyCode.Alpha5)) {
            CurrentSelected = 4;
        }
        if (Input.mouseScrollDelta.y > 1) {
            CurrentSelected += CurrentSelected == 4 ? 0 : 1;
        }
        if (Input.mouseScrollDelta.y < -1) {
            CurrentSelected -= CurrentSelected == 0 ? 0 : 1;
        }
    }

    private void OnValidate () {
        targetPos = defaultPos;
        targetPos.x += deltaPos * _currentSelected;
    }
}