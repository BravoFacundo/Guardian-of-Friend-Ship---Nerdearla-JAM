using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarManager : MonoBehaviour
{
    [SerializeField] private GameObject radarObject;
    [SerializeField] private GameObject arrowPrefab;
    private GameManager gameManager;
    //COmunicacion con game manager
    //Por cada asteroide spawneado crear un nuevo prefab (Cada cierto tiempo)
    
    void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        
    }

    public void CreateArrowForTarget(GameObject target)
    {
        GameObject newArrow = Instantiate(arrowPrefab);
        newArrow.transform.parent = radarObject.transform;
        newArrow.GetComponent<RotatePivotTowardsObject>().target = target.transform;
    }
}
