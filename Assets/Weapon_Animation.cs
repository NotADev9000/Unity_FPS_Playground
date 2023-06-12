using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Animation : MonoBehaviour
{
    [SerializeField] Player_Movement player;

    public void PlayFootstepSound(int foot)
    {
        player.PlayFootstepSound(foot);
    }
}
