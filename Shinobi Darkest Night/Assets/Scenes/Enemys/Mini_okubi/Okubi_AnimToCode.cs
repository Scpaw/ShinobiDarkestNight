using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Okubi_AnimToCode : MonoBehaviour
{
    public void HairAttack()
    {
        Debug.Log("Do hair attack");
        transform.parent.GetComponent<EnemyDamage>().attackAnim = true;
    }
}
