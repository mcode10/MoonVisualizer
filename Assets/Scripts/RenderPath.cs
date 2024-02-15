using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderPath : MonoBehaviour
{
    LineRenderer lineRenderer;  //to hold the line Renderer
    UnityEngine.AI.NavMeshAgent agent;  //to hold the agent of this gameObject
    UnityEngine.AI.NavMeshPath path;  //to hold the agent of this gameObject
    Transform transform;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>(); //get the line renderer
        transform = GetComponent<Transform>(); //get the line renderer
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>(); //get the agent

        lineRenderer.positionCount = 0;

        Gradient gradient = new Gradient();
        Color startColor = new Color(33, 223, 188);
        Color endColor = new Color(255, 255, 255);
        float alpha = 1.0f;
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0f), new GradientColorKey(endColor, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    void Update()
    {
        path = agent.path;
        DrawPath(path);

        Gradient gradient = new Gradient();
        Color startColor = new Color(33, 223, 188);
        Color endColor = new Color(255, 255, 255);
        float alpha = 1.0f;
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0f), new GradientColorKey(endColor, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    void DrawPath(UnityEngine.AI.NavMeshPath path)
    {
        lineRenderer.positionCount = agent.path.corners.Length;
        lineRenderer.SetPosition(0, transform.position);

        if (path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(path.corners[i].x, path.corners[i].y, path.corners[i].z);
            lineRenderer.SetPosition(i, pointPosition);
        }
    }
}
