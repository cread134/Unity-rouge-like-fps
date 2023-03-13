using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility 
{
    void InitializeAbility(GameObject playerHead);

    void StartAbility();

    void UseAbility();

    void EndAbility();

    void SecondInteration();
}
