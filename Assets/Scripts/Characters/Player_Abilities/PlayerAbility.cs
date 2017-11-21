using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{

    [SerializeField]
    private bool unlocked;

    public bool IsUnlocked()
    {
        return unlocked;
    }
}
