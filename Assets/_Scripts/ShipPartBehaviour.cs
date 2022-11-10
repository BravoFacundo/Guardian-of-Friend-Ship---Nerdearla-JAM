using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPartBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;
    private int totalHitPoints = 3;
    [SerializeField] private int currentHitPoints = 3;
    private bool canBroke = true;
    [SerializeField] private GameObject bombPrefab;
    private AudioSource aS;
    [SerializeField] private AudioClip shipExplode;

    [SerializeField] private Transform spawnPivot;
    [SerializeField] private Transform directionPivot;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        aS = GetComponent<AudioSource>();

        if (transform.name != "ShipPart0")
        {
            Vector2 shipDirection = directionPivot.position - spawnPivot.position;
            shipDirection = shipDirection.normalized;
            rb.AddForce(shipDirection * 6f, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ship"))
        {
            //Debug.Log(collision.transform.name);
            if (FindParentWithName(collision.gameObject, "Ship"))
                gameObject.transform.parent = FindParentWithName(collision.gameObject, "Ship").transform;

            //Debug.Log("Freeze");
            //Destroy(gameObject.GetComponent<Rigidbody2D>()); 
            rb.constraints = RigidbodyConstraints2D.FreezeAll; //Destroy(gameObject.GetComponent<Rigidbody2D>());
        }
        if (collision.transform.CompareTag("Obstacle"))
        {
            //Debug.Log("Hit");
            currentHitPoints--;

            collision.transform.tag = "Ship";
            collision.gameObject.GetComponent<AsteroidBehaviour>().Explode();
            collision.gameObject.layer = LayerMask.NameToLayer("NoneColitions");
            Instantiate(bombPrefab, collision.transform);
            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -50;
            
            if (currentHitPoints <= 0 && canBroke)
            {
                canBroke = false;
                //Debug.Log("Broke");
                List<GameObject> parentNullificator = new List<GameObject>();
                aS.clip = shipExplode;
                aS.Play();
             

                Vector2 collisionAngle = collision.transform.position - transform.position;

                var child1 = transform.GetChild(0);
                var child2 = transform.GetChild(1);

                if (transform.parent.childCount == 1)
                {
                    GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                    gameManager.Loose();
                }
                gameObject.GetComponent<ShipPartBehaviour>().enabled = false;

                foreach (Transform child in child1)
                {
                    //Debug.Log(child.name);
                    GameObject newDebris = child.gameObject;
                    newDebris.layer = LayerMask.NameToLayer("NoneColitions");
                    parentNullificator.Add(newDebris);

                    Rigidbody2D newRB = newDebris.AddComponent<Rigidbody2D>();
                    newRB.gravityScale = 0;

                    newRB.AddForce(new Vector2(Random.Range(-0.9f, 0.9f), Random.Range(-0.9f, 0.9f)) 
                        - collisionAngle.normalized, ForceMode2D.Impulse);
                    newRB.AddTorque(Random.Range(-1.9f, 1.9f), ForceMode2D.Impulse);
                    Instantiate(bombPrefab, newDebris.transform);
                }                
                foreach (Transform child in child2)
                {
                    //Debug.Log(child.name);
                    GameObject newDebris = child.gameObject;
                    newDebris.layer = LayerMask.NameToLayer("NoneColitions");
                    parentNullificator.Add(newDebris);

                    Rigidbody2D newRB = newDebris.AddComponent<Rigidbody2D>();
                    newRB.gravityScale = 0;

                    newRB.AddForce(new Vector2(Random.Range(-0.9f, 0.9f), Random.Range(-0.9f, 0.9f)) 
                        - collisionAngle.normalized, ForceMode2D.Impulse);
                    newRB.AddTorque(Random.Range(-1.9f, 1.9f), ForceMode2D.Impulse);
                    Instantiate(bombPrefab, newDebris.transform);
                }
                GameObject bombFather = new GameObject();
                for (int i = 0; i < parentNullificator.Count; i++)
                {
                    GameObject newDebris = parentNullificator[i];
                    newDebris.GetComponent<SpriteRenderer>().sortingOrder = -50;
                    newDebris.transform.parent = bombFather.transform;
                }
            }
        }
    }

    private GameObject FindParentWithName(GameObject child, string name)
    {
        Transform t = child.transform;
        while (t.parent != null)
        {
            if (t.parent.name == name)
            {
                return t.parent.gameObject;
            }
            t = t.parent.transform;
        }
        return null;
    }

}
