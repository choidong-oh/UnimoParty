using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerAvatarSetup : MonoBehaviourPun
{
    [PunRPC]
    public void RPC_SetupPlayer(int charIndex, int shipIndex)
    {
        StartCoroutine(SetupPlayerRoutine(charIndex, shipIndex));
    }

    IEnumerator SetupPlayerRoutine(int charIndex, int shipIndex)
    {
        yield return new WaitForSeconds(0.1f);

        Transform characterPos = transform.Find("XR Origin (XR Rig)/CharacterPos");
        Transform xrOrigin = transform.Find("XR Origin (XR Rig)");
        Transform shipPos = null;

        if (xrOrigin != null && xrOrigin.childCount > 3)
        {
            Transform spaceShip = xrOrigin.GetChild(3);
            shipPos = spaceShip.GetChild(1);
        }

        GameObject[] characters = Resources.LoadAll<GameObject>("Characters");
        GameObject[] ships = Resources.LoadAll<GameObject>("Prefabs");

        if (characterPos && charIndex < characters.Length)
        {
            GameObject charObj = Instantiate(characters[charIndex], characterPos.position, Quaternion.identity, characterPos);
            charObj.transform.localPosition = Vector3.zero;
        }

        if (shipPos && shipIndex < ships.Length)
        {
            GameObject shipObj = Instantiate(ships[shipIndex], shipPos.position, Quaternion.identity, shipPos);
            shipObj.transform.localPosition = Vector3.zero;
        }
    }
}
