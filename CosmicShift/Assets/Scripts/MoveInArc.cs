using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInArc : MonoBehaviour
{
    public bool enable_script = false;
    public Transform glassCube;
    Vector3[] points = new Vector3[3];
    public ClickCounterDimensionChecker dimension_checker;
    public float count;
    // Start is called before the first frame update
    public void Start()
    {
        if(enable_script == true)
        {
            Debug.Log("Using dimension " + dimension_checker.current_dimension);
            //starting point
            points[0] = transform.position;
            //ending point
            points[2] = dimension_checker.dimensions[dimension_checker.current_dimension];
            //Point between starting and ending + height
            points[1] = points[0] + (points[2] - points[0]) / 2 + Vector3.up * 3.0f;
            count = 0.0f;
        }   
    }


    public void Update()
    {
        if(enable_script == true)
        {
            if (count < 1.0f)
            {
                count += 0.4f * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(points[0], points[1], count);
                Vector3 m2 = Vector3.Lerp(points[1], points[2], count);
                transform.position = Vector3.Lerp(m1, m2, count);
                transform.LookAt(glassCube);
            }
            
        }
    }

    public void Enable_Script()
    {
        enable_script = true;
    }
}
