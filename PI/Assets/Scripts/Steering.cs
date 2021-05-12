using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{

    public bool isLeader = false;
    public Steering leader;
    public GameObject[] flock;
    public float keepDistance = 2f;

    public Vector3 velocity;
    private Quaternion rotateDirection;
    private Vector3 direction;

    public List<Node> mapLocations;
    public MonoBehaviour target;
    public MonoBehaviour detectedEnemy;
    public Node destinationNode;
    public Node endNode;
    public List<Node> path;

    public Navigator navigator;
    System.Random random = new System.Random();

    public Node currentNode;

    public float mass;
    public float rotationSpeed = 3.0f;

    public bool isFleeing = false;
    private void Start()
    {
        if (isLeader == true) MakePath(endNode);
        else { target = leader; }
        flock = GameObject.FindGameObjectsWithTag("AI");
    }

    void FixedUpdate()
    {
        Movement();
    }

    void NextNode()
    {
        currentNode = (Node)target;
        if (path.Count > 0)
        {
            target = (MonoBehaviour)path[path.Count - 1];
            path.RemoveAt(this.path.Count - 1);
        }
    }

    void MakePath(Node endNode)
    {
        path.Clear();
        destinationNode = endNode;
        Debug.Log(currentNode);
        path = navigator.CalculatePath(currentNode, endNode);
        path.Add(currentNode);
        NextNode();
    }


    void Movement()
    {
        LookAt();

        velocity.z = 5f;
        if (isLeader == true)
        {
            if (isFleeing == false && Vector3.Distance(transform.position, target.transform.position) > 1.5f || isFleeing == true && Vector3.Distance(transform.position, target.transform.position) < 50)
            {
                transform.Translate(velocity * Time.deltaTime);
            }
            else { NextNode(); }
        }
        else
        {
            foreach (GameObject AI in flock)
            {
                if (AI != gameObject)
                {
                    float distance = Vector3.Distance(AI.transform.position, this.transform.position);
                    if (distance <= keepDistance)
                    {
                        this.transform.position += (transform.position - AI.transform.position)* Time.deltaTime;;
                        transform.Translate(-velocity * Time.deltaTime);
                    }

                    else if (Vector3.Distance(transform.position, AI.transform.position) > keepDistance + 0.5f || isFleeing == true && Vector3.Distance(transform.position, AI.transform.position) < 50)
                    {
                        transform.Translate(velocity * Time.deltaTime);
                    }
                }
            }
        }
    }

    void LookAt()
    {
        direction = (target.transform.position - transform.position).normalized;
        rotateDirection = Quaternion.LookRotation(isFleeing ? -direction : direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateDirection, Time.deltaTime * rotationSpeed);
    }
}