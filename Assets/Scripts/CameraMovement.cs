using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform origin;
    [SerializeField] private float rotationSpeed = 10.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (origin == null)
        {
            Debug.LogWarning("ERROR: origin not set for cameramovement script.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");

        if (horInput != 0)
        {
            transform.RotateAround(origin.position, Vector3.up, -horInput * rotationSpeed * Time.deltaTime);
        }

        if (vertInput != 0)
        {
            transform.RotateAround(origin.position, transform.right, vertInput * rotationSpeed * Time.deltaTime);
        }
    }
}
