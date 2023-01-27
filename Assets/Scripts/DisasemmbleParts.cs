using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisasemmbleParts : MonoBehaviour
{
    public GameObject[] components;
    public Camera camera;
    public GameObject CanvasUI;
    public GameObject chosenComponent;

    private Vector3 targetPosition;
    private GameObject highlight;
    private bool _isFollowingMouse = false;
    private GameObject removeButton, repairButton, putbackButton;
    private CameraController camcontroller;
    private GameObject desk;
    private GameObject screen;
    private TextMeshProUGUI intruction;
    private List<GameObject> removedComponents;
    private List<Vector3> componentsPosition;
    private DebugLightsController debuglightsController;
    public float xMin;
    public float xMax;
    public float zMin;
    public float zMax;
    public float offset;

    private void RemoveHighLight()
    {
        for (int i = 0; i < components.Length; i++)
        {
            highlight = components[i].transform.Find("Highlight").gameObject;
            highlight.SetActive(false);
        }
    }

    private void SelectComponent()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == hitObject)
                    {
                        if (hitObject.name == "PowerButton")
                        {
                            RemoveHighLight();
                            highlight = components[i].transform.Find("Highlight").gameObject;
                            highlight.SetActive(true);
                            removeButton.SetActive(false);
                            repairButton.SetActive(false);
                            putbackButton.SetActive(false);
                            Debug.Log("Power the PC");
                            if (debuglightsController.problemPart == null)
                            {
                                Debug.Log("Power on Successfully");
                                screen.GetComponent<Renderer>().material.color = new Color(0,0,1,1);
                                camcontroller.ManualSwitchCam(3);
                                StartCoroutine(SwitchCamAndWait(2));
                            }
                            else
                            {
                                Debug.Log("Power on Failed");
                                intruction.SetText("The PC is not booting up, something is wrong!");
                                camcontroller.ManualSwitchCam(2);
                                StartCoroutine(SwitchCamAndWait(3));
                            }
                            break;
                        }
                        else
                        {
                            RemoveHighLight();
                            Debug.Log("Chose " + hitObject.name);
                            chosenComponent = components[i];
                            highlight = components[i].transform.Find("Highlight").gameObject;
                            highlight.SetActive(true);
                            repairButton.SetActive(false);
                            removeButton.SetActive(true);
                            putbackButton.SetActive(false);
                            camcontroller.target = chosenComponent;
                            if (removedComponents.Contains(chosenComponent))
                            {
                                removeButton.SetActive(false);
                                repairButton.SetActive(true);
                                putbackButton.SetActive(true);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    private IEnumerator SwitchCamAndWait(int location)
    {

        if (location == 3)
        {
            yield return new WaitForSeconds(2f);
            camcontroller.ManualSwitchCam(location);
            intruction.SetText("The LEDs are lighting up meaning something is wrong with one of the component!");
            yield return new WaitForSeconds(3f);
            intruction.SetText("For this motherboard:\nThe first light is CPU\nThe second light is DRAM\nThe third light is VGA\nThe fourth light is BOOT");
        }
        else if(location == 2)
        {
            intruction.SetText("The LEDs are lighting up meaning the PC should be able to boot now!");
            yield return new WaitForSeconds(3f);
            camcontroller.ManualSwitchCam(location);
            intruction.SetText("Congrats, you have fixed the PC!");
        }


    }

    private void ArrangeComponents()
    {
        float x = xMin;
        float z = zMin;
        foreach (GameObject component in removedComponents)
        {
            component.transform.position = -new Vector3(x, 0, z) + desk.transform.position;
            x += offset;
            if (x > xMax)
            {
                x = xMin;
                z += offset;
                if (z > zMax)
                {
                    z = zMin;
                }
            }
        }
    }

    private async void RepairComponent()
    {
        if (debuglightsController.problemPart.name == chosenComponent.name)
        {
            Debug.Log(chosenComponent.name + " is fixed!");
            debuglightsController.problemPart = null;
        }
        else
        {
            Debug.Log(chosenComponent.name + " is not broken!");
        }
    }

    private async void RemoveComponent()
    {
        componentsPosition.Add(chosenComponent.transform.position);
        removedComponents.Add(chosenComponent);
        chosenComponent.transform.position = desk.transform.position;
        camcontroller.ResetCam();
        ArrangeComponents();
        removeButton.SetActive(false);
        repairButton.SetActive(true);
        putbackButton.SetActive(true);
    }

    private async void PutBackComponent()
    {
        int componentIndex = removedComponents.IndexOf(chosenComponent);
        chosenComponent.transform.position = componentsPosition[componentIndex];
        camcontroller.ResetCam();
        removeButton.SetActive(true);
        repairButton.SetActive(false);
        putbackButton.SetActive(false);
    }

    void Start()
    {
        removeButton = CanvasUI.transform.Find("RemoveButton").gameObject;
        repairButton = CanvasUI.transform.Find("RepairButton").gameObject;
        putbackButton = CanvasUI.transform.Find("PutBackButton").gameObject;
        intruction = GameObject.FindWithTag("Instruction").gameObject.GetComponent<TextMeshProUGUI>();
        camcontroller = camera.GetComponent<CameraController>();
        removeButton.GetComponent<Button>().onClick.AddListener(RemoveComponent);
        repairButton.GetComponent<Button>().onClick.AddListener(RepairComponent);
        putbackButton.GetComponent<Button>().onClick.AddListener(PutBackComponent);
        desk = GameObject.FindWithTag("Desk");
        screen = GameObject.FindWithTag("Screen");
        removedComponents = new List<GameObject>();
        componentsPosition = new List<Vector3>();
        debuglightsController = GameObject.FindWithTag("Motherboard").GetComponent<DebugLightsController>();
        intruction.SetText("Try powering on the PC by pressing the white button on the top right of the case!");
    }

    void Update()
    {
        SelectComponent();
    }
}
