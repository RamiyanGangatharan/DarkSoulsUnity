using System;
using UnityEngine;

namespace DarkSouls
{
    public class CharacterManager : MonoBehaviour
    {
        protected virtual void Awake() { DontDestroyOnLoad(this); }
        protected virtual void Update()
        {

        }
    }
}
