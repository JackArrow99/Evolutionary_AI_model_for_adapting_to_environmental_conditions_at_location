using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using System;
using static TrackCheckpoints;

public class CarAgent : Agent
{
    public bool Active = false;
    
    public GameObject[] Sensors;

    public GameObject CarToControl;
    public PrometeoCarController Controller;
    public Transform carColliderTransform;      // set manually!

    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private Vector3 startPosition;

    public int checkpoints;

    // Heuristics

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int gas_reverse_action = 0;
        int left_right_action = 0;

        // W/S //
 
        if (Input.GetKey(KeyCode.W)) gas_reverse_action = 1;
        //if (Input.GetKey(KeyCode.S)) gas_reverse_action = 2;
        

        // A/D //

        if (Input.GetKey(KeyCode.D)) left_right_action = 1;
        if (Input.GetKey(KeyCode.A)) left_right_action = 2;


        // Space //
        // = nothing here =

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        discreteActions[0] = gas_reverse_action;
        discreteActions[1] = left_right_action;

    }

    //


    private void Start()
    {
        SpeedRewardInvoke();

        //Controller = CarToControl.GetComponent<PrometeoCarController>(); // already set manually (perhaps?)

        // subscribe to events:
        trackCheckpoints.OnCarCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        trackCheckpoints.OnCarWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;

    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, TrackCheckpointsEventArgs e)
    {
        
        
        if (e.eventCarTransform == carColliderTransform)
        {
            //  + Reward

            //AddReward(+1f);

            Vector3 checkpointForward = trackCheckpoints.GetNextCheckpoint(carColliderTransform).transform.forward;
            float directionDotNext = Vector3.Dot(transform.forward, checkpointForward);

            //AddReward( 1f * directionDotNext );

            if (Controller.carSpeed > 30) AddReward(2f * directionDotNext);
            else AddReward(1f * directionDotNext);

            checkpoints++;

            

        }
    }


    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, TrackCheckpointsEventArgs e)
    {
        if (e.eventCarTransform == carColliderTransform)
        {
            //  very small Reward

            //AddReward(-1f);
            //EndEpisode();
        }
    }



    // *** Checkpoint System ***



    // Begin Episode

    public override void OnEpisodeBegin()
    {
        checkpoints = 0;
        
        // Reset position
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;

        // Set moving speed to zero
        Controller.Stop();

        // Reset checkpoints
        trackCheckpoints.ResetCheckpoints(carColliderTransform);
        

    }

    // ***

    // Sensors
    public override void CollectObservations(VectorSensor sensor)
    {
            // Track sensors (raycast) are collect automatically (perhaps?)
        
        // *** Landscape sensors ***

        for (int i = 0; i < Sensors.Length; i++)
        {
            //var sens = Sensors[i].GetComponent<SensorScript>();

            //sensor.AddObservation(sens.isOverlappingSand);

        }




        // *** Other sensors ***


            // next checkpoint direction (should be forward)
        
        Vector3 checkpointForward = trackCheckpoints.GetNextCheckpoint(carColliderTransform).transform.forward;
        float directionDotNext = Vector3.Dot(transform.forward, checkpointForward);

        sensor.AddObservation(directionDotNext);
        //Debug.Log(directionDotNext);

        Vector3 checkpointForwardAfterNext = trackCheckpoints.GetAfterNextCheckpoint(carColliderTransform).transform.forward;
        float directionDotAfterNext = Vector3.Dot(transform.forward, checkpointForwardAfterNext);

        sensor.AddObservation(directionDotAfterNext);
        //Debug.Log(directionDotAfterNext);

        // car speed

        sensor.AddObservation(Controller.carSpeed);
        //Debug.Log(Controller.carSpeed);

    }

    // Actions

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Параметры для выбора действий Агентом

        float gas_reverse = 0f;

        float left_right = 0f;


        // just don't use break. it's quite useless
        bool stop = false;

        switch (actions.DiscreteActions[0])
        {
            case 0: gas_reverse = 0f; break;
            case 1: gas_reverse = +1f; break;
            //case 2: gas_reverse = -1f; break;

        }

        switch (actions.DiscreteActions[1])
        {
            case 0: left_right = 0f; break;
            case 1: left_right = +1f; break;
            case 2: left_right = -1f; break;

        }


        if (Active == true) Controller.AgentControls(gas_reverse, left_right, stop);

        
        // Немного поучим его ездить вперед, потом отключим
        //if (gas_reverse < 0) AddReward(-0.01f);
        //if (gas_reverse > 0) AddReward(0.01f);

    }



    // *** Reward ***

    private void SpeedRewardInvoke()
    {
        SpeedReward();
        Invoke(nameof(SpeedRewardInvoke), 1);
    }
    private void SpeedReward()
    {
        //Debug.Log("SpeedReward");


        //if (Controller.carSpeed < 0) AddReward(-0.001f);
        //if (Controller.carSpeed > 0) AddReward(0.001f* Controller.carSpeed);

        if (Controller.carSpeed > 30f) AddReward(0.01f);

        //if (Controller.carSpeed > 30f) AddReward(Controller.carSpeed / 500f);

        //if (Controller.carSpeed > 60f) AddReward(Controller.carSpeed / 250f);

    }

    private void Death()
    {
        Invoke(nameof(EndEpisode), 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        // DEATH WALL
        if (other.gameObject.tag == "DeathWall")
        {
            Debug.Log("Death Wall collision");

            //SetReward(GetCumulativeReward() / 2);

            if (Controller.carSpeed > 60) AddReward(-3f);
            else AddReward(-1f);

            Debug.Log("=============");
            Debug.Log("Total checkpoints: ");
            Debug.Log(checkpoints);
            Debug.Log("=============");
            Death();
        }

        // SAND
        if (other.gameObject.tag == "Sand")
        {
            //Debug.Log("Sand collision");

            AddReward(-0.25f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // DEATH WALL
        if (other.gameObject.tag == "DeathWall")
        {

            AddReward(-0.01f);
        }

        // SAND
        if (other.gameObject.tag == "Sand")
        {
            //Debug.Log("Sand...");

            AddReward(-0.01f);
        }
    }


}
