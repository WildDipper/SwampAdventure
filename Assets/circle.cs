using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class circle : MonoBehaviour


{
    //Assign a GameObject in the Inspector to rotate around

    /*   public GameObject target;

       public float OrbitSpeed = 500f;

       Vector3 axis = new Vector3(0, 10, 0);

       void Update()

       {

           // Spin the object around the target at 20 degrees/second.

           transform.RotateAround(target.transform.position, axis, OrbitSpeed * Time.deltaTime);
          // transform.RotateAround(point, axis, Time.deltaTime * 10);


       } */

    // Add this script to Cube(2)
    [Header("Add your turret")]
    public GameObject Turret;//to get the position in worldspace to which this gameObject will rotate around.

    [Header("The axis by which it will rotate around")]
    public Vector3 axis;//by which axis it will rotate. x,y or z.

    [Header("Angle covered per update")]
    public float angle; //or the speed of rotation.

    public float upperLimit, lowerLimit, delay; // upperLimit & lowerLimit: heighest & lowest height; 
    private float height, prevHeight, time; //height:height it is trying to reach(randomly generated); prevHeight:stores last value of height;delay in radomness; 

    // Update is called once per frame
    void Update()
    {
        //Gets the position of your 'Turret' and rotates this gameObject around it by the 'axis' provided at speed 'angle' in degrees per update 
        transform.RotateAround(Turret.transform.position, axis.normalized, angle);
        time += Time.deltaTime;
        //Sets value of 'height' randomly within 'upperLimit' & 'lowerLimit' after delay 
        if (time > delay)
        {
            prevHeight = height;
            height = Random.Range(lowerLimit, upperLimit);
            time = 0;
        }
        //$$anonymous$$athf.Lerp changes height from 'prevHeight' to 'height' gradually (smooth transition)  
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(prevHeight, height, time), transform.position.z);
    }
}
