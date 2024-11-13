using UnityEngine;

public class VerticalMovingTrap : MonoBehaviour
{
    public Transform pointA;  
    public Transform pointB;  
    public float speed = 2f; 
    public float initialOffset = 0f;  
    public float bufferDistance = 0.5f;  

    private Vector3 targetPosition;
    private float initialX;

    void Start()
    {
        initialX = transform.position.x;
        
        transform.position = new Vector3(
            initialX,
            pointA.position.y + initialOffset,
            transform.position.z
        );
        
        targetPosition = new Vector3(initialX, pointB.position.y, transform.position.z);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPosition, 
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < bufferDistance)
        {
            targetPosition = targetPosition.y == pointA.position.y 
                ? new Vector3(initialX, pointB.position.y, transform.position.z) 
                : new Vector3(initialX, pointA.position.y, transform.position.z);
        }
    }
}
