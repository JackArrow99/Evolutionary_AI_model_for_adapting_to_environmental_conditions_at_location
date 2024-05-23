using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrackCheckpoints : MonoBehaviour
{
    public class TrackCheckpointsEventArgs : EventArgs
    {
        public Transform eventCarTransform;
    }

    public event EventHandler<TrackCheckpointsEventArgs> OnCarCorrectCheckpoint;
    public event EventHandler<TrackCheckpointsEventArgs> OnCarWrongCheckpoint;

    [SerializeField] private List<Transform> carTransformList;

    private List<CheckpointSingle> checkpointSingleList;
    private List<int> nextCheckpointSingleIndexList;

    private void Awake()
    {
        Transform CheckpointsTransform = transform.Find("Checkpoints");


        checkpointSingleList = new List<CheckpointSingle>();
        foreach(Transform checkpointSingleTransform in CheckpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();


            checkpointSingle.SetTrackCheckpoints(this);
            
            checkpointSingleList.Add(checkpointSingle);
        }



        nextCheckpointSingleIndexList = new List<int>();
        foreach(Transform carTransform in carTransformList)
        {
            nextCheckpointSingleIndexList.Add(0);
        }

    }



    public void ResetCheckpoints(Transform carTransform)
    {
        nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)] = 0;
        //Debug.Log(nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)]);
    }

    public CheckpointSingle GetNextCheckpoint(Transform carTransform)
    {
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];

        return (checkpointSingleList[nextCheckpointSingleIndex]);

    }

    public CheckpointSingle GetAfterNextCheckpoint(Transform carTransform)
    {
        
        int nextCheckpointSingleIndex = (nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)] + 1) % checkpointSingleList.Count;

        return (checkpointSingleList[nextCheckpointSingleIndex]);
    }


    public void CarEntersCheckpoint(CheckpointSingle checkpointSingle, Transform carTransform)
    {
        //Debug.Log(carTransform);

        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];




        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            // Correct
            //Debug.Log("Correct! :)");

            if (checkpointSingleList.IndexOf(checkpointSingle) == checkpointSingleList.Count)
            {
                Debug.Log("*** Circle finished ***");
            }


            OnCarCorrectCheckpoint?.Invoke(this, new TrackCheckpointsEventArgs { eventCarTransform = carTransform });

            nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)] 
                = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;

            
        }
        else
        {
            // Wrong
            Debug.Log("Wrong! >:(");
            OnCarWrongCheckpoint?.Invoke(this, new TrackCheckpointsEventArgs { eventCarTransform = carTransform });
        }
    }

}
