using UnityEngine;

public abstract class Skill : MonoBehaviour
{   
    public abstract void UseSkill();

    public abstract bool IsOnCooldown();
}