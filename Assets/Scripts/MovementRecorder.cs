using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MovementRecorder : MonoBehaviour
{
    List<Vector3> movementHistory = new List<Vector3>();
    public float jumpDuration = 0.1f, waitDuration = 0.2f;// Duration for playback
    public bool isPlaybacking = false; // Flag to check if playback is in progress
    public void AddTrack(Vector3 position)
    {
        movementHistory.Add(position);
    }

    public void PlayBackMovement()
    {
        isPlaybacking = true; // Set jumping state to true to not die during playback
        StartCoroutine(LerpCameraSmoothSpeed(30f, 2f));  // 📈 smoothly go from 5 → 30
        StartCoroutine(PlaybackCoroutine());
    }

    public Vector3 GetAndRemoveLastTrack()
    {
        if (movementHistory.Count == 0) return Vector3.zero; // Return zero if no history

        Vector3 lastPosition = movementHistory[movementHistory.Count - 1];
        movementHistory.RemoveAt(movementHistory.Count - 1); // Remove the last position
        return lastPosition; // Return the last position
    }

    IEnumerator PlaybackCoroutine()
    {
        // Loop backwards through the movement history
        for (int i = movementHistory.Count - 1; i >= 1; i--)
        {
            Vector3 startPosition = movementHistory[i];
            Vector3 targetPosition = movementHistory[i - 1]; // move toward the previous point

            float elapsedTime = 0f;

            while (elapsedTime < jumpDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / jumpDuration);

                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null; // wait for next frame
            }

            // Snap exactly to the target at the end to avoid float errors
            transform.position = targetPosition;
            yield return new WaitForSeconds(waitDuration); // wait before next move
        }

        // ✅ Snap to very first recorded position, just in case
        if (movementHistory.Count > 0)
            transform.position = movementHistory[0];

        // ✅ Clear history when done
        movementHistory.Clear();
        StartCoroutine(LerpCameraSmoothSpeed(5f, 0.5f));  // 📉 smoothly go from 30 → 5

        AddTrack(transform.position); // Record the final position after playback
        isPlaybacking = false; 
    }

IEnumerator LerpCameraSmoothSpeed(float targetSpeed, float duration)
{
    CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
    float startSpeed = camFollow.smoothSpeed;
    float elapsed = 0f;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        camFollow.smoothSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
        yield return null;
    }

    camFollow.smoothSpeed = targetSpeed; // ✅ Snap exactly to target at end
}


}
