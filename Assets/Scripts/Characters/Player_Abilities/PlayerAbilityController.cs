using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityController : MonoBehaviour
{

    [SerializeField]
    private List<PlayerAbility> abilities;

    private int currentAbilityIndex;

    private void Start()
    {
        currentAbilityIndex = 0;
        if (abilities[0])
        {
            abilities[0].enabled = true;
        }
        for (int i = 1; i < abilities.Count; i++)
        {
            abilities[i].enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown(InputNames.Shift_Ability_Up))
        {
            CycleAbility(true);
        }
        else if (Input.GetButtonDown(InputNames.Shift_Ability_Down))
        {
            CycleAbility(false);
        }
    }

    private void CycleAbility(bool up)
    {
        abilities[currentAbilityIndex].enabled = false;
        if (up)
        {
            if (currentAbilityIndex < abilities.Count - 1)
            {
                currentAbilityIndex++;
            }
            else
            {
                currentAbilityIndex = 0;
            }
        }
        else
        {
            if (currentAbilityIndex == 0)
            {
                currentAbilityIndex = abilities.Count - 1;
            }
            else
            {
                currentAbilityIndex--;
            }
        }
        abilities[currentAbilityIndex].enabled = true;
    }
}
