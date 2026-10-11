using UnityEngine;

public class Die : MonoBehaviour
{
    public LayerMask safeLayer;
    public void CheckPlayerDeath()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f, safeLayer);

        if (hits.Length == 0)
        {
            KillPlayer();
        }

        else if (hits.Length > 0)
        {
            if (hits[0].gameObject.layer == LayerMask.NameToLayer("WinPad"))
            {
                Win();
            }
        }
    }

    public void KillPlayer()
    {
        //GameObject.FindGameObjectWithTag("GameManager").GetComponent<SceneManage>().ReloadScene();
        Debug.Log("You died!");
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ReduceTime();
        GetComponent<PlayerMovement>().isJumping = false; 
        Vector3 targetPosition = GetComponent<MovementRecorder>().GetAndRemoveLastTrack(); // Remove last track to avoid death during playback
        print("Target Position: " + targetPosition);
        GetComponent<PlayerMovement>().Jump(targetPosition);
    }

    void Win()
    {
        Debug.Log("You won!");
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().increaseStageCounter();
        transform.position = new Vector3(0, -7.0f, 0); // Reset position
        GetComponent<MovementRecorder>().PlayBackMovement();
    }
}
