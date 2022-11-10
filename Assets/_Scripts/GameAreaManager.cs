using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAreaManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log(collision.gameObject.name);

            Destroy(collision.gameObject);
            gameManager.desviatedMeteors++;
        }
    }

}
