using UnityEngine;
using System.Collections;

public class GlichedPad : MonoBehaviour
{
    [HideInInspector] public float glitchDuration;
    public Animator anim;
    SpriteRenderer spriteRenderer;

public void StartGlitch()
{
    spriteRenderer = GetComponent<SpriteRenderer>();
    StartCoroutine(GlitchEffect());
}

IEnumerator GlitchEffect()
{
    // 🎲 Random delay only the very first time
    if (firstGlitch)
    {
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        firstGlitch = false;
    }

        // ✨ Start glitch
        anim.SetBool("Glitching", true);
    //spriteRenderer.enabled = false;

    float timer = 0f;
    while (timer < glitchDuration)
    {
        // 🔍 Check for player while glitched
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Vector2 center = (Vector2)transform.position + collider.offset;
            Vector2 size = collider.size;
            float angle = transform.eulerAngles.z;

            Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle);

            foreach (Collider2D hit in hits)
            {
                    if (hit.CompareTag("Player"))
                    {
                        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isJumping == false &&
                        GameObject.FindGameObjectWithTag("Player").GetComponent<MovementRecorder>().isPlaybacking == false)
                        {
                            GameObject.FindGameObjectWithTag("Player").GetComponent<MovementRecorder>().GetAndRemoveLastTrack();
                            GameObject.FindGameObjectWithTag("Player").GetComponent<Die>().KillPlayer();
                        }
                }
            }
        }

        timer += Time.deltaTime;
        yield return null; // ⏳ wait for next frame
    }

    // 🌟 End glitch
    //spriteRenderer.enabled = true;
        anim.SetBool("Glitching", false);
    yield return new WaitForSeconds(glitchDuration);

    // 🔁 Start again
    StartCoroutine(GlitchEffect());
}

private bool firstGlitch = true;
}
