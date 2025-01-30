using UnityEngine;

public class Blinking : MonoBehaviour
{
    public SkinnedMeshRenderer head;
    public GameObject player;
    public GameObject headObject;

    // Indices for blend shapes
    private int eyeBlinkUpperRightIndex;
    private int eyeBlinkUpperLeftIndex;

    // Blinking parameters
    public float blinkDuration = 0.6f; // Time to fully close or open the eyes
    public float blinkIntervalMin = 1.0f; // Minimum time between blinks
    public float blinkIntervalMax = 4.0f; // Maximum time between blinks

    private float blinkTimer;
    private bool isBlinking = false;
    private float blinkProgress = 0f;

    void Start()
    {
        // Get the blend shape indices for blinking
        eyeBlinkUpperRightIndex = head.sharedMesh.GetBlendShapeIndex("HEADBlends.eyeBlinkUpperRight");
        eyeBlinkUpperLeftIndex = head.sharedMesh.GetBlendShapeIndex("HEADBlends.eyeBlinkUpperLeft");

        if (eyeBlinkUpperRightIndex == -1 || eyeBlinkUpperLeftIndex == -1)
        {
            Debug.LogError("Blink blend shapes not found! Check the names in the SkinnedMeshRenderer.");
            enabled = false;
            return;
        }

        // Initialize the timer for the first blink
        blinkTimer = Random.Range(blinkIntervalMin, blinkIntervalMax);
    }

    void Update()
    {
        if (!isBlinking)
        {
            // Countdown to the next blink
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0f)
            {
                isBlinking = true;
                blinkProgress = 0f;
            }
        }
        else
        {
            // Handle blinking animation
            blinkProgress += Time.deltaTime / blinkDuration;

            if (blinkProgress <= 0.5f)
            {
                // Closing eyes
                float blendValue = Mathf.Lerp(0f, 100f, blinkProgress * 2f);
                SetBlinkBlendShape(blendValue);
            }
            else if (blinkProgress < 1f)
            {
                // Opening eyes
                float blendValue = Mathf.Lerp(100f, 0f, (blinkProgress - 0.5f) * 2f);
                SetBlinkBlendShape(blendValue);
            }
            else
            {
                // Blink finished
                isBlinking = false;
                blinkTimer = Random.Range(blinkIntervalMin, blinkIntervalMax);
                SetBlinkBlendShape(0f); // Ensure the eyes are fully open
            }
        }
    }

    private void SetBlinkBlendShape(float value)
    {
        head.SetBlendShapeWeight(eyeBlinkUpperRightIndex, value);
        head.SetBlendShapeWeight(eyeBlinkUpperLeftIndex, value);
    }
}