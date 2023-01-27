using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLightsController : MonoBehaviour
{
    public GameObject[] debugLights;
    public GameObject problemPart;

    private GameObject chosenLight;
    private CameraController camera;

    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, debugLights.Length);
        chosenLight = debugLights[rand];
        chosenLight.SetActive(true);
        problemPart = GameObject.FindWithTag(chosenLight.name);
        //camera = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
        //camera.target = problemPart;
    }

    // Update is called once per frame
    void Update()
    {
        if (problemPart == null) 
        {
            chosenLight.SetActive(false);
        }
    }
}
