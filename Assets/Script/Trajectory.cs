using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    public GameObject bullet;
    public Transform target;
    public Transform shootPoint;
    private bool isDrawingPath = false;
    private Vector3 velocity;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("LauncherProjectile");
            LauncherProjectile();
        }

        if (isDrawingPath)
        {
            DrawPath(velocity);
        }
    }

    void LauncherProjectile()
    {
        Vector3 distance = target.position - shootPoint.position;
        float distance_x = distance.x;
        float distance_y = distance.y;

        float velocityValue = 30f;  // 예: 10 m/s

        // 발사 각도 계산
        float launchAngle = CalculateLaunchAngle(distance_x, distance_y, velocityValue);
        Debug.Log($"Calculated Launch Angle: {launchAngle} degrees");

        // 발사체 발사
        velocity = GetVelocityVector(launchAngle, velocityValue);

        if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y))
        {
            Debug.LogError("Calculated velocity vector is invalid!");
            return;
        }

        Rigidbody obj = Instantiate(bullet, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        obj.velocity = velocity;

        isDrawingPath = true;
    }

    float CalculateLaunchAngle(float distance_x, float distance_y, float velocity)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        float part1 = Mathf.Pow(velocity, 2);
        float part2 = Mathf.Pow(velocity, 4) - gravity * (gravity * Mathf.Pow(distance_x, 2) + 2 * distance_y * Mathf.Pow(velocity, 2));

        if (part2 < 0)
        {
            Debug.LogError("No valid solution for the launch angle (negative discriminant).");
            return 45f;  // 기본 각도나 예외 처리
        }

        float sqrtPart2 = Mathf.Sqrt(part2);
        float angle1 = Mathf.Atan2(part1 - sqrtPart2, gravity * distance_x);
        float angle2 = Mathf.Atan2(part1 + sqrtPart2, gravity * distance_x);

        float angleInDegrees1 = angle1 * Mathf.Rad2Deg;
        float angleInDegrees2 = angle2 * Mathf.Rad2Deg;

        Debug.Log($"Calculated angles: {angleInDegrees1}°, {angleInDegrees2}°");

        return Mathf.Min(angleInDegrees1, angleInDegrees2);
    }

    Vector3 GetVelocityVector(float angle, float velocity)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        float velocityX = velocity * Mathf.Cos(angleRad);
        float velocityY = velocity * Mathf.Sin(angleRad);

        Debug.Log($"Calculated velocity vector: ({velocityX}, {velocityY})");

        return new Vector3(velocityX, velocityY, 0);
    }

    void DrawPath(Vector3 velocity)
    {
        Vector3 previousDrawPoint = transform.position;
        int resolution = 30;
        for (int i = 1; i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution * 1f;

            Vector3 displacement = velocity * simulationTime + Vector3.up * Physics.gravity.y * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = transform.position + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }
    }
}