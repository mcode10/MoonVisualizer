using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multipleCameras : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera overHeadCamera;
    public Camera firstpersonCamera;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShowCameras();
    }
    public void ShowCameras() 
    {
        overHeadCamera.enabled = true;
        firstpersonCamera.enabled = true;
    }
}
