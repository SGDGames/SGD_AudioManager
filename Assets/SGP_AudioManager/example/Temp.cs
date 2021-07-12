using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGP_Util;
public class Temp : MonoBehaviour
{
    public string[] key;
    public int Multicount = 1;

    public void Load()
    {
        for (var i = 0; i < key.Length; i++) SGP_AudioManager.i.PoolAudio(key[i], null, 1, false, false, Multicount);
    }

    public void Play()
    {
        SGP_AudioManager.i.PlayAudio(key[Random.Range(0, key.Length)]);
    }
}