using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public GameObject[] camPos;

    private Vector3 _offset = new Vector3(0.0f, 0.0f, -15.0f);
    private float smoothSpeed = 0.01f;
    private float lookAtSpeed = 0.25f;
    private float lookAhead = 0.1f; 

    private Vector3 _lookAheadPos; 
    private Vector3 _targetPos;
    private GameObject _previousTarget;
    private int _previousSelectedIndex;
    private Vector3 desiredPosition, smoothedPosition, lookAtPos;
    private TMPro.TMP_Dropdown dropdown;
    private TextMeshProUGUI intruction;

    // Start is called before the first frame update

    private void CalculateCameraDistance()
    {
        _targetPos = target.transform.position;
        _previousTarget = target;
    }

    private void MoveCam()
    {
        // Smoothly move the camera towards the target GameObject
        desiredPosition = _targetPos + _offset;
        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    private void RotateCam()
    {
        // Smoothly look at the target GameObject
        _lookAheadPos = _targetPos + (target.transform.forward * lookAhead);
        lookAtPos = Vector3.Lerp(transform.position + transform.forward, _lookAheadPos, lookAtSpeed * Time.deltaTime);
        transform.LookAt(lookAtPos);
    }

    public void ManualSwitchCam(int location)
    {
        switch (location)
        {
            case 0:
                target = camPos[0];
                intruction.SetText("Select the component to repair or put it back into the PC");
                break;
            case 1:
                target = camPos[1];
                intruction.SetText("Select the component to remove it from the PC");
                break;
            case 2:
                target = camPos[2];
                break;
            case 3:
                target = camPos[3];
                intruction.SetText("For this motherboard:\nThe first light is CPU\nThe second light is DRAM\nThe third light is VGA\nThe fourth light is BOOT");
                break;
        }
    }

    public void ResetCam()
    {
        int selectedView = dropdown.value;
        switch (selectedView)
        {
            case 0:
                target = camPos[0];
                intruction.SetText("Select the component to repair or put it back into the PC");
                break;
            case 1:
                target = camPos[1];
                intruction.SetText("Select the component to remove it from the PC");
                break;
            case 2:
                target = camPos[2];
                intruction.SetText("");
                break;
            case 3:
                target = camPos[3];
                intruction.SetText("For this motherboard:\nThe first light is CPU\nThe second light is DRAM\nThe third light is VGA\nThe fourth light is BOOT");
                break;
        }
    }

    private void AutoSwitchCam()
    {
        if (_previousSelectedIndex != dropdown.value)
        {
            int selectedView = dropdown.value;
            _previousSelectedIndex = selectedView;
            switch (selectedView)
            {
                case 0:
                    target = camPos[0];
                    intruction.SetText("Select the component to repair or put it back into the PC");
                    break;
                case 1:
                    target = camPos[1];
                    intruction.SetText("Select the component to remove it from the PC");
                    break;
                case 2:
                    target = camPos[2];
                    intruction.SetText("");
                    break;
                case 3:
                    target = camPos[3];
                    intruction.SetText("For this motherboard:\nThe first light is CPU\nThe second light is DRAM\nThe third light is VGA\nThe fourth light is BOOT");
                    break;
            }
        }
    }

    void Start()
    {
        CalculateCameraDistance();
        dropdown = GameObject.FindWithTag("ViewList").GetComponent<TMPro.TMP_Dropdown>();
        _previousSelectedIndex = dropdown.value;
        intruction = GameObject.FindWithTag("Instruction").gameObject.GetComponent<TextMeshProUGUI>();
    }


    // Update is called once per frame
    void Update()
    {
        if (_previousTarget != target)
        {
            CalculateCameraDistance();
        }

        MoveCam();
        RotateCam();
        AutoSwitchCam();
    }
}
