using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class CoinSpawner : MonoBehaviour
{
    private Vector2 StartingPoint;
     
    public Vector2 Gap = new Vector2(0,0);
    public int CoinCount;
  
    public GameObject CoinPrefab;
  
    // Start is called before the first frame update
    void Start()
    {
        StartingPoint = gameObject.transform.position;
 
        for(int i=0; i<CoinCount; i++)
        {
            Instantiate(CoinPrefab, StartingPoint + (Gap*i), Quaternion.identity);
        }
        Destroy(this);
    }
} 