using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void OnClickCharacterPick(int characterIndex)
    {
        if (PlayerInfo.PI != null)
        {
            PlayerInfo.PI.mySelectedColour = characterIndex;
            PlayerPrefs.SetInt("MyCharacter", characterIndex);
        }
    }
}
