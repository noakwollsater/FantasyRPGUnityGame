using UnityEngine;
using System.Collections;

public class CharacterRotation : MonoBehaviour
{
    public float rotationSpeed = 100f; // Adjust the speed of rotation
    public float resetSpeed = 5f; // Speed at which the character resets its rotation

    private bool isDragging = false;
    private float lastMouseX;
    private Quaternion initialRotation;

    public GameObject character;

    void Start()
    {
        initialRotation = transform.rotation; // Store the initial rotation
    }

    void Update()
    {
        if (character == null)
        {
            character = GameObject.Find("Sidekick Character");
        }

        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            lastMouseX = Input.mousePosition.x;
            StopAllCoroutines(); // Stop reset if user starts dragging again
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
            StartCoroutine(RotateBackToInitial()); // Start reset rotation
        }

        if (isDragging)
        {
            float deltaX = Input.mousePosition.x - lastMouseX;
            lastMouseX = Input.mousePosition.x;
            float rotationAmount = deltaX * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, -rotationAmount); // Rotate the character around the Y axis
        }
    }

    IEnumerator RotateBackToInitial()
    {
        while (Quaternion.Angle(transform.rotation, initialRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, resetSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = initialRotation; // Ensure exact alignment at the end
    }
}
