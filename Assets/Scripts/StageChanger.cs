using System.Collections.Generic;
using UnityEngine;

public class StageChanger : MonoBehaviour
{


    #region stageVoid
    Dictionary<Vector2Int, GameObject> lillypads = new Dictionary<Vector2Int, GameObject>();

    float hoppeDistance;

    public GameObject lossPanel;
    public GameObject startPad, goalPad;
    [SerializeField, Range(0f, 1f)] float voidChance = 0.2f;

    public GameObject[] voidsGO;

    public int antallVoids = 10;
    float grenseHoyreVenstre = 6, grenseOpp = 20;

    #endregion

    #region stageColor

    //Har farge for nå, men må gjøre det på en annen måte, som med glitching
    public Color glichedColor;
    [SerializeField, Range(0f, 1f)] float colorChance = 0.4f;

    #endregion

    #region stageCamera
    public Camera mainCamera;
    public float cameraScale = 2f;
    public float cameraRotation = 20f;

    #endregion

    #region startStart

    public GameObject lillyPadPrefab;
    public int lillypadsAnt = 40;
    public int lillyPadLimitVertical = 22;
    public int lillyPadSideLimit = 5;

    #endregion

    #region stageGlitch
    [SerializeField, Range(0f, 1f)] float glitchChance = 0.2f;

    #endregion

    void Start()
    {
        StartStartStage();
    }

    #region stageStartFunctions
    public void StartStartStage()
    {
        lossPanel.SetActive(false);
        hoppeDistance = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().dragLimit;

        SpawnLillyPadsStart();
    }

    void SpawnLillyPadsStart()
    {
        bool validSetup = false;
        int attempts = 0;

        while (!validSetup && attempts < 5000)
        {
            attempts++;

            // 🔄 delete old random pads before retrying
            foreach (GameObject lp in GameObject.FindGameObjectsWithTag("LillyPad"))
            {
                if (lp != startPad && lp != goalPad)   // don’t destroy the fixed pads
                    Destroy(lp);
            }
            lillypads.Clear();

            // 🏗 spawn random pads (excluding start & goal)
            for (int i = 0; i < lillypadsAnt; i++)
            {
                // halve the range, then multiply by 2 so we keep the same world limit
                int randX = Random.Range(-lillyPadSideLimit / 2, (lillyPadSideLimit / 2) + 1) * 2;
                int randY = Random.Range(0, (lillyPadLimitVertical / 2) + 1) * 2;

                Vector3 spawnPos = new Vector3(randX, randY, 0f);

                GameObject pad = Instantiate(lillyPadPrefab, spawnPos, Quaternion.identity);

                Vector2Int position = new Vector2Int(randX, randY);

                if (!lillypads.ContainsKey(position))
                {
                    lillypads[position] = pad;
                } else
                {
                    Destroy(pad); // Avoid duplicates
                    i--; // Retry this iteration
                }
            }


            // ✅ include start & goal pads in the pad list
            lillypads[new Vector2Int(Mathf.RoundToInt(startPad.transform.position.x),
                                     Mathf.RoundToInt(startPad.transform.position.y))] = startPad;

            lillypads[new Vector2Int(Mathf.RoundToInt(goalPad.transform.position.x),
                                     Mathf.RoundToInt(goalPad.transform.position.y))] = goalPad;

            List<GameObject> allPads = new List<GameObject>(lillypads.Values);

            // ✅ check if there’s a path between startPad and goalPad
            if (PathExistsStart(startPad, goalPad, allPads, hoppeDistance))
            {
                Debug.Log($"✅ Valid lillypad layout found after {attempts} tries");
                validSetup = true;
            }
            else
            {
                Debug.Log("❌ No valid path — retrying...");
            }
        }

        if (!validSetup)
        {
            Debug.LogError("🚨 Could not generate a valid lillypad layout after 5000 tries!");
        }
    }

    bool PathExistsStart(GameObject start, GameObject goal, List<GameObject> allPads, float maxJumpDistance)
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            GameObject current = queue.Dequeue();

            if (current == goal)
                return true;

            List<GameObject> neighbors = GetNeighborsStart(current, allPads, maxJumpDistance);

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return false;
    }

    List<GameObject> GetNeighborsStart(GameObject currentLillyPad, List<GameObject> allLillyPads, float maxDistance)
    {
        List<GameObject> neighbors = new List<GameObject>();
        Vector2 currentPosition = currentLillyPad.transform.position;

        foreach (GameObject pad in allLillyPads)
        {
            if (pad == currentLillyPad) continue;

            float distance = Vector2.Distance(currentPosition, pad.transform.position);
            if (distance <= maxDistance)
            {
                neighbors.Add(pad);
            }
        }
        return neighbors;
    }

    #endregion

    #region stageCameraFunctions

    public void StartCamera()
    {
        mainCamera = Camera.main;
        mainCamera.orthographicSize = cameraScale;
        mainCamera.transform.rotation = Quaternion.Euler(0, 0, cameraRotation);
    }

    #endregion

    #region stageColorFunctions

    public void StartColorStage()
    {
        GameObject[] lillyPads = GameObject.FindGameObjectsWithTag("LillyPad");
        foreach (GameObject pad in lillyPads)
        {
            if (Random.value < colorChance) // 40% sjanse
            {
                //Forandre denne fra color til glitch sprite
                if (pad != startPad && pad != goalPad && pad.layer != LayerMask.NameToLayer("Glitch")) // Ikke start/mål
                {
                    pad.GetComponent<SpriteRenderer>().color = glichedColor;
                    //pad.layer = LayerMask.NameToLayer("Glitch");
                }

            }
        }
    }

    #endregion

    #region stageVoidFunctions
    public void StartStageVoid()
    {
        hoppeDistance = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().dragLimit;
        // Find all lillypads in the scene and add them to the dictionary

        foreach (GameObject lillypad in GameObject.FindGameObjectsWithTag("LillyPad"))
        {

            Vector2Int position = new Vector2Int(Mathf.RoundToInt(lillypad.transform.position.x), Mathf.RoundToInt(lillypad.transform.position.y));
            if (!lillypads.ContainsKey(position))
            {
                lillypads[position] = lillypad;
            }
        }

        MakeVoidPads();
    }

    void MakeVoidPads()
    {
        List<GameObject> allPads = new List<GameObject>(lillypads.Values);
        bool validSetup = false;
        int attempts = 0;

        List<GameObject> allVoids = new List<GameObject>();

        while (!validSetup && attempts < 10000)
        {

            foreach (var voidGO in allVoids)
            {
                Destroy(voidGO);
            }

            allVoids.Clear();

            attempts++;

            // ✅ Resett alle pads til normal før nytt forsøk
            foreach (var pad in allPads)
            {
                if (pad != goalPad && pad.tag != "Void")
                {
                    pad.layer = LayerMask.NameToLayer("Lillypad");
                    pad.GetComponent<Animator>().SetBool("Void", false);
                    pad.GetComponent<SpriteRenderer>().color = Color.white;
                }

            }


            for (int i = 0; i < antallVoids; i++)
            {
                GameObject voidInstance = Instantiate(voidsGO[Random.Range(0, voidsGO.Length)], new Vector3(Random.Range(-lillyPadSideLimit, lillyPadSideLimit), Random.Range(0f, lillyPadLimitVertical - 5), 0), Quaternion.identity);
                allVoids.Add(voidInstance);
                BoxCollider2D collider = voidInstance.GetComponent<BoxCollider2D>();


                // Calculate the world-space center and size for overlap check
                Vector2 center = (Vector2)voidInstance.transform.position + collider.offset;
                Vector2 size = collider.size;
                float angle = voidInstance.transform.eulerAngles.z;

                // Get all overlapping colliders in that box area
                Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle);

                foreach (Collider2D hit in hits)
                {
                    // Check if hit is a lilypad
                    if (hit.CompareTag("LillyPad"))
                    {
                        // Set the lilypad's layer to Void (or do whatever you want)
                        hit.gameObject.layer = LayerMask.NameToLayer("Void");
                        hit.gameObject.GetComponent<Animator>().SetBool("Void", true);
                        //hit.gameObject.GetComponent<SpriteRenderer>().color = Color.black; // Set color to black
                    }
                }
            }

            // ✅ Sjekk om sti finnes fra start til mål
            if (PathExists(startPad, goalPad, allPads, hoppeDistance))
            {
                validSetup = true;
                Debug.Log(" Gyldig bane funnet!");
                
                foreach (var pad in allPads)
                {
                    if (pad.layer == LayerMask.NameToLayer("Void"))
                    {
                        pad.tag = "Void"; // Set tag to Void for all void pads
                    }

                }
            }
            else
            {
                Debug.Log(" Ingen vei – prøver igjen...");
            }
        }

        if (!validSetup)
        {
            Debug.LogError(" Fant ikke gyldig oppsett etter 50 forsøk!");
        }
    }

    List<GameObject> GetNeighbors(GameObject currentLillyPad, List<GameObject> allLillyPads, float maxDistance)
    {
        List<GameObject> neighbors = new List<GameObject>();
        Vector2 currentPosition = currentLillyPad.transform.position;

        foreach (GameObject pad in allLillyPads)
        {
            if (pad == currentLillyPad) continue; // Skip the current lillypad
            if (pad.layer == LayerMask.NameToLayer("Void")) continue; // Ensure it's a lillypad

            float distance = Vector2.Distance(currentPosition, pad.transform.position);
            //Debug.Log($"{currentLillyPad.name} -> {pad.name} distanse: {distance}");
            if (distance <= maxDistance)
            {
                neighbors.Add(pad);
            }
        }


        return neighbors;
    }

    bool PathExists(GameObject start, GameObject goal, List<GameObject> allPads, float maxJumpDistance)
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            GameObject current = queue.Dequeue();

            if (current == goal)
                return true;

            List<GameObject> neighbors = GetNeighbors(current, allPads, maxJumpDistance);

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return false;
    }

    #endregion

    #region stageGlitchFunctions
    public void startGlitchStage()
    {
        GameObject[] lillyPads = GameObject.FindGameObjectsWithTag("LillyPad");
        foreach (GameObject pad in lillyPads)
        {
            if (Random.value < glitchChance) // 20% sjanse
            {
                if (pad != startPad && pad != goalPad && pad.layer != LayerMask.NameToLayer("Void")) // Ikke start/mål
                {
                    pad.GetComponent<GlichedPad>().glitchDuration = Random.Range(2f, 3f); // Random glitch duration
                    pad.GetComponent<GlichedPad>().StartGlitch();
                }

            }
        }
    }

    #endregion
}
