using UnityEngine;

public class NextPositionSprite : MonoBehaviour
{
    public Sprite targetLillyPadSprite;
    public Sprite defaultSprite;

    void Update()
    {
        Collider2D col = Physics2D.OverlapBox(transform.position, new Vector2(0.1f, 0.1f), 0f, LayerMask.GetMask("Lillypad"));

        if(col != null && col.gameObject.layer == LayerMask.NameToLayer("Lillypad"))
        {
            GetComponent<SpriteRenderer>().sprite = targetLillyPadSprite; // Set to target sprite
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = defaultSprite; // Set to default sprite
        }
    }
}
