using UnityEngine;

public class Chaser : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetTransform = PlayerController.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(targetTransform.position);
    }
}
