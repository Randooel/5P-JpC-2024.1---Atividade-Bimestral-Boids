using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{

    public float minSpeed = 2;
    public float maxSpeed = 5;

    public float perceptionRadius = 2.5f;
    public float avoidanceRadius = 1;

    public float maxSteerForce = 3;

    public float alignWeight = 1;
    public float cohesionWeight = 1;
    public float seperateWeight = 1;

    public float targetWeight = 1;

    Vector3 velocity;

    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;

    public int numPerceivedFlockmates;


    void Awake()
    {
        transform.Rotate(0,Random.Range(0,360),0);


        float startSpeed = (minSpeed + maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void UpdateBoid()
    {
        Vector3 acceleration = Vector3.zero;

        if (numPerceivedFlockmates != 0)
        {
            Vector3 trueCentrerOfFlockMates = centreOfFlockmates / numPerceivedFlockmates;

            Vector3 distanceFromFlockMatesCentrer = (trueCentrerOfFlockMates - transform.position);

            Vector3 alignmentForce = SteerTowards(avgFlockHeading) * alignWeight;
            Vector3 cohesionForce = SteerTowards(distanceFromFlockMatesCentrer) * cohesionWeight;
            Vector3 seperationForce = SteerTowards(avgAvoidanceHeading) * seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }else
        {
            acceleration = transform.forward;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;

        speed = Checkspeed(speed);
        dir = Checkborders(dir, speed);

        

        velocity = dir * speed;

        transform.position += velocity * Time.deltaTime;
        transform.forward = dir;
    }

    private Vector3 Checkborders(Vector3 direction, float speed)
    {
        Vector3 newPosition = transform.position += (direction * speed) * Time.deltaTime;

        if (newPosition.x > 15 && direction.x > 0)
            direction.x *= -1;

        if (newPosition.x < -15 && direction.x < 0)
            direction.x *= -1;

        if (newPosition.z > 10 && direction.z > 0)
            direction.z *= -1;

        if (newPosition.z < -10 && direction.z < 0)
            direction.z *= -1;

        if (newPosition.y > 12 && direction.y > 0)
            direction.y *= -1;

        if (newPosition.y < 0 && direction.y < 0)
            direction.y *= -1;

        return direction;
    }

    private float Checkspeed(float speed)
    {
        return Mathf.Clamp(speed, minSpeed, maxSpeed);
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 temp = vector.normalized * maxSpeed - velocity;
        return Vector3.ClampMagnitude(temp, maxSteerForce);
    }
}
