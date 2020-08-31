using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable {
    void hit(int damage, Attribute attackType);
    void heal(int value);
}