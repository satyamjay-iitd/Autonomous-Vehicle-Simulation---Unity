using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathScript : MonoBehaviour
{
    public Transform[] path;
    public float speed = 5.0f;
    public float reachDist = 10.0f;
    public int currentPoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    // void Update() {
    //     Vector3 pos = transform.position;
    //     Vector3 dir = path[currentPoint].position - pos;
    //     pos += dir * Time.deltaTime * speed;
    //     if(dir.magnitude<=reachDist)
    //     {
    //         currentPoint++;
    //     }
    //     if(currentPoint>=path.Length)
    //     {
    //         currentPoint = 0;
    //     }
    // }
    // private void OnDrawGizmos()
    // {
    //     if(path.Length>0)
    //     {
    //         for(int i=0;i<path.Length;i++)
    //         {
    //             if(path[i]!=null)
    //             {
    //                 Gizmos.DrawSphere(path[i].position, reachDist);
    //             }
    //         }
    //     }
    // }
}
