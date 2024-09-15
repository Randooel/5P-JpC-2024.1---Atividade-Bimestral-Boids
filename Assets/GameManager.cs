using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    const int threadGroupSize = 10;

    public ComputeShader compute;
    Boid[] boids;
    
    void Start()
    {
        boids = FindObjectsOfType<Boid>();
    }

    void Update()
    {
        if (boids != null)
        {
            int numBoids = boids.Length;
            BoidData[] boidData = new BoidData[numBoids];

            for (int i = 0; i < numBoids; i++)
            {
                boidData[i].position = boids[i].transform.position;
                boidData[i].direction = boids[i].transform.forward;
            }

            var boidBuffer = new ComputeBuffer(numBoids, sizeof(float) * 3 * 5 + sizeof(int));
            boidBuffer.SetData(boidData);

            compute.SetBuffer(0, "boids", boidBuffer);
            compute.SetInt("numBoids", numBoids);
            compute.SetFloat("viewRadius", boids[0].perceptionRadius);
            compute.SetFloat("avoidRadius", boids[0].avoidanceRadius);

            int threadGroups = Mathf.CeilToInt((float)boids.Length / threadGroupSize);

            compute.Dispatch(0, threadGroups, 1, 1);
            
            boidBuffer.GetData(boidData);

            for (int i = 0; i < boids.Length; i++)
            {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                boids[i].UpdateBoid();
            }

            boidBuffer.Release();
        }
    }

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

    }
}
