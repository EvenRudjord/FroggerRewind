using UnityEngine;

public class NoParent : MonoBehaviour
{
    void Start()
    {
        transform.SetParent(null);
    }
}
