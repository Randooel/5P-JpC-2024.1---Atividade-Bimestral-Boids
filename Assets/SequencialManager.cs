using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class SequencialManager : MonoBehaviour
{
    Boid[] boids;

    void Start()
    {
        boids = FindObjectsOfType<Boid>();
    }

    private void Update()
    {
        if (boids != null)
        {
            for (int i = 0; i < boids.Length; i++)
            {
                boids[i].numPerceivedFlockmates = 0;
                boids[i].avgFlockHeading = Vector3.zero;
                boids[i].centreOfFlockmates = Vector3.zero;
                boids[i].avgAvoidanceHeading = Vector3.zero;

                for (int j = 0; j < boids.Length; j++)
                {
                    if (i != j)
                    {
                        Boid neighborBoid = boids[j];
                        Vector3 distance = neighborBoid.transform.position - boids[j].transform.position;

                        if (distance.magnitude < boids[i].perceptionRadius)
                        {
                            boids[i].numPerceivedFlockmates += 1;
                            boids[i].avgFlockHeading += neighborBoid.transform.forward;
                            boids[i].centreOfFlockmates += neighborBoid.transform.position;

                            if (distance.magnitude < boids[i].avoidanceRadius)
                            {
                                boids[i].avgAvoidanceHeading -= distance / distance.magnitude;
                            }
                        }
                    }
                }
                boids[i].UpdateBoid();
            }
        }
    }
}
