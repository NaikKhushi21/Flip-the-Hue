using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    public Transform pointA; 
    public Transform pointB; 
    public float speed = 2f;

    private Vector3 targetPosition; 
    private float initialY; 

    void Start()
    {
        initialY = transform.position.y;
        
        transform.position = new Vector3(pointA.position.x, initialY, transform.position.z);
        
        targetPosition = new Vector3(pointB.position.x, initialY, transform.position.z);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPosition, 
            speed * Time.deltaTime
        );

        if (transform.position.x == targetPosition.x)
        {
            targetPosition = targetPosition.x == pointA.position.x 
                ? new Vector3(pointB.position.x, initialY, transform.position.z) 
                : new Vector3(pointA.position.x, initialY, transform.position.z);
        }
    }
}
