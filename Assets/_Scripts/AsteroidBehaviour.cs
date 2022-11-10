using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour
{
    [SerializeField] public Transform playerTransform;
    [SerializeField] public GameObject miniAsteroidsPrefab;
    [SerializeField] private float force = 1;
    private Rigidbody2D rb;
    private AudioSource aS;
    [SerializeField] private AudioClip explodeSFX;
    
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        aS = GetComponent<AudioSource>();
        NewImpulse();
    }

    public void NewImpulse()
    {
        Vector2 playerDirection = playerTransform.position - transform.position;
        playerDirection = playerDirection.normalized;
        rb.AddForce(playerDirection * force, ForceMode2D.Impulse);
    }

    public void Explode()
    {
        //Luego agregar los pequeñitos
        //Debug.Log("Explota");
        aS.clip = explodeSFX;
        aS.Play();

        int minis = Random.Range(3, 5);
        GetComponent<SpriteRenderer>().enabled = false;
        for (int i = 0; i < minis; i++)
        {
            GameObject newMini = Instantiate(miniAsteroidsPrefab, transform);
            newMini.transform.localPosition = Vector2.zero;
            newMini.layer = LayerMask.NameToLayer("NoneColitions");
            Rigidbody2D newRB = newMini.AddComponent<Rigidbody2D>();
            newRB.gravityScale = 0;
            CircleCollider2D newBC = newMini.AddComponent<CircleCollider2D>();
            newRB.AddForce(new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), ForceMode2D.Impulse);
            newRB.AddTorque(Random.Range(-1.9f, 1.9f), ForceMode2D.Impulse);
        }
    }
}
