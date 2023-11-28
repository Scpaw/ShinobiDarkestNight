using UnityEngine;

public class NamahageProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public float startForce = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * startForce,ForceMode2D.Impulse);
    }


    private void Hit()
    {
        PlayerController.Instance.gameObject.GetComponent<PlayerHealth>().AddDamage(10);
        Destroy(gameObject, 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerController.Instance.gameObject)
        {
            Hit();
        }
        else if( collision.gameObject.layer == 9)
        {
            Destroy(gameObject,0.2f);
        }
    }
}
