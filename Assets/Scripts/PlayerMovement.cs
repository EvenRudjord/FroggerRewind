using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public bool isJumping = false;
    public LineRenderer lineRenderer;
    public Animator animator;
    public GameObject distanceRenderer;
    public GameObject nextPositionObject;

    public float jumpDuration = 1f;
    public float dragLimit = 6f; // Limit the drag distance to 6 units
    public int sideLimit = 7;
    public float distanceScale = 5;

    private Vector3 startMousePosition;
    private Vector3 endMousePosition;
    Vector3 mouseWorldPosition;
    Die dieScript;

    void Start()
    {
        dieScript = GetComponent<Die>();
        nextPositionObject.GetComponent<SpriteRenderer>().enabled = false; // Hide the next position object
       // distanceRenderer.transform.localScale = new Vector3(distanceScale * 2, distanceScale * 2, 1f); // Set the distance renderer scale
        GetComponent<MovementRecorder>().AddTrack(transform.position); // Record the initial position
    }
    void Update()
{
    mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mouseWorldPosition.z = 0f;

    if (Input.GetMouseButtonDown(0))
        {
            animator.SetBool("isCharging", true);
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0));
            startMousePosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);
            nextPositionObject.GetComponent<SpriteRenderer>().enabled = true;
        }

    if (Input.GetMouseButton(0))
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(1, new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0));

        // ✅ Calculate drag vector (from where you first clicked to current mouse pos)
        Vector3 dragVector = mouseWorldPosition - startMousePosition;

            // ✅ Clamp drag length
            
            if (dragVector.magnitude > dragLimit)
            {
                dragVector = dragVector.normalized * dragLimit;
                SetLineColor(Color.red);
               
                distanceRenderer.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                distanceRenderer.GetComponent<SpriteRenderer>().enabled = false;
                SetLineColor(Color.white);

            }

        // ✅ Position preview object on the **opposite** side of the player
        setNextPositionObject(transform.position - dragVector);
    }

    if (Input.GetMouseButtonUp(0))
    {
        Vector3 dragVector = mouseWorldPosition - startMousePosition;
        distanceRenderer.GetComponent<SpriteRenderer>().enabled = false;

            // ✅ Clamp again on release
            if (dragVector.magnitude > dragLimit)
            {
                dragVector = dragVector.normalized * dragLimit;
            }
            
        // ✅ Clear line renderer
            SetLineColor(Color.white);


        // ✅ Jump in the **opposite** direction of drag
            Jump(transform.position - dragVector);
    }
}

    void SetLineColor(Color color)
    {
        lineRenderer.startColor = new Color(1f, 1f, 1f, 0f);
        lineRenderer.endColor = new Color(1f, 1f, 1f, 0f);
    }
    public void Jump(Vector3 targetPosition)
    {
        
        if (isJumping || GetComponent<MovementRecorder>().isPlaybacking) return; // Prevent multiple jumps

        
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().startCountdown = true;
        isJumping = true;

        animator.SetFloat("JumpTime", 1/jumpDuration);
        animator.SetBool("isJumping", true);
        animator.SetBool("isCharging", false);
        nextPositionObject.GetComponent<SpriteRenderer>().enabled = false;
        endMousePosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);
        lineRenderer.positionCount = 0;
        StartCoroutine(MoveToPosition(setNextPositionObject(targetPosition)));
    }

    Vector3 setNextPositionObject(Vector3 position)
    {
        // Snap position to the nearest whole number grid (1x1)
        Vector3 snappedPosition = new Vector3(
            Mathf.Round(position.x),
            Mathf.Round(position.y),
            Mathf.Round(position.z)
        );

        if(position.x > sideLimit)
        {
            snappedPosition.x = sideLimit; // Limit the X position to a maximum of 6
        }
        if(position.x < -sideLimit)
        {
            snappedPosition.x = -sideLimit; // Limit the Y position to a minimum of -6
        }

        nextPositionObject.transform.position = snappedPosition;
        return snappedPosition;
    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / jumpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;  // wait for next frame
        }

        // Ensure player ends exactly at target position
        transform.position = targetPosition;
        animator.SetBool("isJumping", false);
        isJumping = false;
        dieScript.CheckPlayerDeath();
        
        GetComponent<MovementRecorder>().AddTrack(transform.position); // Record the position after jump
    }
}
