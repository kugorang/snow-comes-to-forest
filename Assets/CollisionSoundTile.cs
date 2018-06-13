using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSoundTile : MonoBehaviour
{
    public GameObject Light;
    public AudioSource Sound;
    public bool IsTurnOn { private get; set; }
    
    public void CollisionByCharacter()
    {
        if (IsTurnOn)
            return;
        
        Light.SetActive(true);
        Sound.Play();
        IsTurnOn = true;
    }
}
