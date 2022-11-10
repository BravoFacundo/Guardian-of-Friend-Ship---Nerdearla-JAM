using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    [Header ("Debug")]
    [SerializeField] private float bodiesDistance;
    [SerializeField] private bool usingPhysicalBody;

    [Header ("Movement")]
    [SerializeField] private float physicalSpeed;
    private float horizontalInput = 0f;
    private float verticalInput = 0f;
    private Vector2 moveDirection;

    [Header("Astral Projection")]
    [SerializeField] private float astralSpeed;
    [SerializeField] private float permitedDistance;
    private bool canChangeBodies = true;  //Al ser bools opuestas todo el tiempo se pueden obviar y ser solo 1
    private bool goBackToBody = false;
    private DistanceJoint2D bodyJoint;
    [SerializeField] private GameObject newBodyObject;
    [SerializeField] private GameObject lastBodyObject;

    [Header("Grabbing Objects")]
    [SerializeField] public Collider2D currentCollision;
    [SerializeField] private GameObject grabbedObject;
    private bool canGrabObjects = true;   //Al ser bools opuestas todo el tiempo se pueden obviar y ser solo 1
    private bool grabbingObject = false;
    private SpringJoint2D grabJoint;

    [Header("References")]
    [SerializeField] public GameObject physicalGO; //physicalBody
    [SerializeField] private GameObject astralGO; //innerBody
    private GameObject cam;
    private Rigidbody2D physicalRB;
    private Rigidbody2D astralRB;
    private SpriteRenderer physicalSR;
    private SpriteRenderer astralSR;
    private GameObject arms;

    [Header("SoundRe ferences")]
    [SerializeField] private AudioClip grab;
    [SerializeField] private AudioClip bodyExit;
    [SerializeField] private AudioClip bodyEnter;
    [SerializeField] private AudioClip bodyLimit;
    private AudioSource aS;
    private bool limitSoundLock;

    void Start()
    {
        cam = Camera.main.gameObject;
        physicalRB = physicalGO.GetComponent<Rigidbody2D>();
        astralRB = astralGO.GetComponent<Rigidbody2D>();
        arms = astralGO.transform.GetChild(0).gameObject;
        physicalSR = physicalGO.GetComponent<SpriteRenderer>();
        astralSR = astralGO.GetComponent<SpriteRenderer>();
        aS = GetComponent<AudioSource>();
    }

    void Update()
    {
        //Body Move Input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Body Change Input
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1))
        {
            if (canChangeBodies)
            {
                usingPhysicalBody = false; canChangeBodies = false;

                astralRB.constraints = RigidbodyConstraints2D.None;
                astralRB.constraints = RigidbodyConstraints2D.FreezeRotation;
                astralGO.transform.localPosition = Vector2.zero;
                astralGO.GetComponent<Renderer>().enabled = true;
                astralGO.transform.parent = null;

                StartBodyJoint();
                SetCameraParent(astralGO);
                ControllBody(astralRB, 10, ForceMode2D.Impulse);

                aS.clip = bodyExit;
                aS.loop = false;
                aS.Play();
            }
        }
        else
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(1))
        {
            if (Input.GetMouseButtonDown(0))
            {
                arms.SetActive(true);                                
            }
            else
            if (Input.GetMouseButton(0))
            {
                if (currentCollision != null && canGrabObjects)
                {
                    canGrabObjects = false;
                    grabbingObject = true;

                    grabbedObject = currentCollision.gameObject;

                    StartObjectJoint();

                    //Si agarras el objeto nuevo -> Entonces el joint se hace con el objeto nuevo                   
                    if (newBodyObject != null)
                    {
                        lastBodyObject = physicalGO;
                        physicalGO = newBodyObject.gameObject;
                        physicalRB = newBodyObject.gameObject.GetComponent<Rigidbody2D>();

                        EndBodyJoint();
                        StartBodyJoint();
                    }
                }
            }
            else
            if (Input.GetMouseButtonUp(0))
            {
                arms.SetActive(false);
                
                EndObjectJoint();

                canGrabObjects = true;
                grabbingObject = false;

                //Si tenes objeto viejo -> Entonces el joint se hace con el objeto viejo
                if (lastBodyObject != null)
                {
                    newBodyObject = null;
                    physicalGO = lastBodyObject.gameObject;
                    physicalRB = lastBodyObject.gameObject.GetComponent<Rigidbody2D>();
                    EndBodyJoint();
                    StartBodyJoint();
                }
            }            
        }
        else
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(1))
        {
            if (!goBackToBody)
            {
                goBackToBody = true;
                astralGO.GetComponent<Collider2D>().enabled = false;

                aS.clip = bodyEnter;
                aS.loop = false;
                aS.Play();
            }

            EndObjectJoint();

            canGrabObjects = true;
            grabbingObject = false;
            arms.SetActive(false);
        }

        //Check Distance Between Bodies
        bodiesDistance = Vector2.Distance(physicalGO.transform.position, astralGO.transform.position);

        if (bodiesDistance > 4 && !limitSoundLock)
        {
            aS.clip = bodyLimit;
            aS.loop = true;
            aS.Play();
            limitSoundLock = true;
        }
        else
        if (bodiesDistance < 4 && limitSoundLock)
        {
            aS.Stop();
            limitSoundLock = false;
        }
    }

    private void FixedUpdate()
    {
        if (usingPhysicalBody)
            ControllBody(physicalRB, physicalSpeed, ForceMode2D.Force);
        else
            ControllBody(astralRB, astralSpeed, ForceMode2D.Force);

        if (goBackToBody)
        {
            if (bodyJoint != null)
            {
                bodyJoint.maxDistanceOnly = false;
                bodyJoint.distance = 0;
            }

            if (bodiesDistance < 0.5f)
            {
                SetCameraParent(physicalGO);

                astralGO.transform.parent = physicalGO.transform;
                astralGO.transform.localPosition = Vector2.zero;
                astralGO.GetComponent<Renderer>().enabled = false;
                astralGO.GetComponent<Collider2D>().enabled = true;
                astralRB.constraints = RigidbodyConstraints2D.FreezePosition;

                goBackToBody = false;
                EndBodyJoint();

                canChangeBodies = true;
                usingPhysicalBody = true;
            }
        }

    }
    private void ControllBody(Rigidbody2D rb, float speed, ForceMode2D forceMode)
    {
        //Move Body
        moveDirection = cam.transform.up * verticalInput + cam.transform.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * speed, forceMode);

        //Limit Speed
        Vector2 flatVel = new Vector2(rb.velocity.x, rb.velocity.y);
        if (flatVel.magnitude > speed)
        {
            Vector2 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector2(limitedVel.x, limitedVel.y);
        }

        //Limit Rotation Speed
        //if (rb.angularVelocity > speed) rb.angularVelocity = speed;

        //Limit Slippery Movement

        //Flip Sprite based on velocity direction
        if (rb.velocity.x > 0)
        {
            physicalSR.flipX = false;
            astralSR.flipX   = true;
            arms.transform.localScale = new Vector2( -1 , 1 );
        }
        else
        if (rb.velocity.x < 0)
        {
            physicalSR.flipX = true;
            astralSR.flipX   = false;
            arms.transform.localScale = new Vector2(1, 1);
        }
    }

    void StartBodyJoint()
    {
        bodyJoint = astralGO.AddComponent<DistanceJoint2D>();
        bodyJoint.connectedBody = physicalRB;
        bodyJoint.autoConfigureDistance = false;
        bodyJoint.maxDistanceOnly = true;
        bodyJoint.distance = permitedDistance;
    }
    void EndBodyJoint()
    {
        Destroy(bodyJoint);
        //if (astralGO.GetComponent<Joint2D>() != null) Destroy(jointBody);
    }
    void StartObjectJoint()
    {
        grabJoint = currentCollision.gameObject.AddComponent<SpringJoint2D>();
        grabJoint.connectedBody = astralRB;
        grabJoint.autoConfigureDistance = false;
        grabJoint.distance = 0;
        grabJoint.dampingRatio = 1;
        grabJoint.frequency = 0;
        grabJoint.connectedAnchor = new Vector2(0, 0.4f);

        aS.clip = grab;
        aS.loop = true;
        aS.Play();
    }
    void EndObjectJoint()
    {
        if (grabbedObject != null) 
            Destroy(grabbedObject.GetComponent<SpringJoint2D>());
        grabbedObject = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            currentCollision = collision;
        }
        
        else
        if (collision.gameObject.CompareTag("Body"))
        {
            currentCollision = collision;
            newBodyObject = collision.gameObject;
            
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            currentCollision = null;
        }
        
        else
        if (collision.gameObject.CompareTag("Body"))
        {
            currentCollision = null;            
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ship"))
        {
            currentCollision = collision.collider;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ship"))
        {
            currentCollision = collision.collider;
        }
    }


    private void SetCameraParent(GameObject parent)
    {
        cam.transform.parent = parent.transform;
        cam.transform.localPosition = new Vector3(0, 0, -10);
    }
}
