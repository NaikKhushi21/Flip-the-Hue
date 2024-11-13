using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float initialOffset = 0f;
    public float bufferDistance = 0.5f; 

    private Vector3 targetPosition;
    private float initialY;

    void Start()
    {
        initialY = transform.position.y;
        
        transform.position = new Vector3(
            pointA.position.x + initialOffset,
            initialY,
            transform.position.z
        );
        
        targetPosition = new Vector3(pointB.position.x, initialY, transform.position.z);
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
            targetPosition = targetPosition.x == pointA.position.x 
                ? new Vector3(pointB.position.x, initialY, transform.position.z) 
                : new Vector3(pointA.position.x, initialY, transform.position.z);
        }
    }
}
