using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class namahageAnimToCode : MonoBehaviour
{

    private Namahage nam;
    private EnemyDamage enemyDamage;

    private void Start()
    {
        nam = transform.parent.GetComponent<Namahage>();
        enemyDamage = transform.parent.GetComponent<EnemyDamage>();
    }

    public void BucketAttack()
    {
        Debug.Log("after bucket");
        nam.AfterAtttack();
    }

    public void Stab()
    {
        enemyDamage.Dmg();
    }
}
