using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class LoadPlayerSetting : MonoBehaviour
{
    [SerializeField] private ActionBasedControllerManager ABCM;
    [SerializeField] private ActionBasedSnapTurnProvider snapTurn;
    [SerializeField] private ActionBasedContinuousTurnProvider smoothTurn;
    void Start()
    {
        ABCM = GameObject.Find("Camera Offset").transform.GetChild(5).GetComponent<ActionBasedControllerManager>();
        GameObject turn = GameObject.Find("Turn");
        snapTurn = turn.GetComponent<ActionBasedSnapTurnProvider>();
        smoothTurn = turn.GetComponent<ActionBasedContinuousTurnProvider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
