using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class namahageAnimToCode : MonoBehaviour
{

    private Namahage nam;
    private EnemyDamage enemyDamage;
    private Transform player;

    private void Start()
    {
        nam = transform.parent.GetComponent<Namahage>();
        enemyDamage = transform.parent.GetComponent<EnemyDamage>();
        player = PlayerController.Instance.GetPlayer().transform;
    }

    public void BucketAttack()
    {
        float angle = Mathf.Atan2((player.position - transform.position).y, (player.position - transform.position).x) * Mathf.Rad2Deg;
        List<GameObject> projectile = new List<GameObject>();
        projectile.Add( Instantiate(nam.projectile, transform.position, Quaternion.Euler(Vector3.forward * angle)));
        projectile.Add( Instantiate(nam.projectile, transform.position, Quaternion.Euler(Vector3.forward * (angle + 15))));
        projectile.Add( Instantiate(nam.projectile, transform.position, Quaternion.Euler(Vector3.forward * (angle - 15))));

        foreach (GameObject pro in projectile)
        {
            pro.GetComponent<NamahageProjectile>().startForce = nam.power;
        }


        nam.AfterAtttack();
    }

    public void Stab()
    {
        enemyDamage.Dmg();
    }
}