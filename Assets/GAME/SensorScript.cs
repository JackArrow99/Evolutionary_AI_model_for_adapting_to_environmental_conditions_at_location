using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using System.Diagnostics;
using UnityEngine;

public class SensorScript : MonoBehaviour
{
    public bool isOverappingRoad;
    public bool isOverlappingSand;

    public Material material_Road;
    public Material material_NoRoad;
    public Material material_Sand;

    Renderer rend;

    // Update is called once per frame

    private void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material_NoRoad;
    }

    void Update()
    {
        ColorSwitch();
    }

    // *** COLOR SWITCH ***

    void ColorSwitch()
    {
        if (isOverlappingSand == true)
        {
            rend.sharedMaterial = material_Sand;
        }
        else
        {
            rend.sharedMaterial = material_Road;
        }
    }

    // ********************  Check Collisions  ********************

    void OnTriggerStay(Collider other)
    {
        

        if (other.gameObject.tag == "Road")
        {
            //print("Is overlapping Road!");
            isOverappingRoad = true;
        }

        if (other.gameObject.tag == "Sand")
        {
            //print("Is overlapping Sand!");
            isOverlappingSand = true;
        }
    }

    // OnTriggerEnter is disabled
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision enter");
        
        //if(other.gameObject.tag == "Road")
        {
        //    isOverappingRoad = true;
        }

    }


    void OnTriggerExit(Collider other)
    {
        //Debug.Log("Collision exit");

        if (other.gameObject.tag == "Road")
        {
            isOverappingRoad = false;
        }

        if (other.gameObject.tag == "Sand")
        {
            isOverlappingSand = false;
        }
    }
}
