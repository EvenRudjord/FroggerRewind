using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System.Collections;

public class GameManager : MonoBehaviour
{
    int stageCounter = 0;
    [Header("Timer Settings")]
    public float tid = 20;

    float startTid;
    public TMP_Text timerDisplay;
    public TMP_Text timerDisplay2;

    public GameObject lossPanel;
    public TMP_Text lossText;
    public TMP_Text stageCounterText;

    public bool startCountdown = true;

    void Start()
    {
        startTid = tid; // Store the initial timer value
        lossPanel.SetActive(false);
    }

    void Update()
    {
        timerDisplay.text = tid.ToString("0");
        if (!startCountdown) return;
        tid -= Time.deltaTime;
        // Update timer text

        if (tid <= 0)
        {
            tomForTid();
        }


    }

    void tomForTid()
    {
        startCountdown = false; // Stop the countdown
        tid = 0; // Ensure timer doesn't go negative
        Debug.Log("Tiden er tom!");
        lossText.text = "Time's Up";
        stageCounterText.text = "Treats Eaten: " + stageCounter.ToString();
        lossPanel.SetActive(true);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Die>().KillPlayer();
    }

    public void increaseStageCounter()
    {
        StageChanger stageChanger = GetComponent<StageChanger>();
        tid = startTid;
        startCountdown = false; // Stop the countdown when changing stage
        stageCounter++;
        if (stageCounter % 2 == 1)  // oddetall: 1, 3, 5...
        {
            stageChanger.StartStageVoid();
        }
        else // partall: 2, 4, 6...
        {
            stageChanger.startGlitchStage();
        }
    }

    public void ReduceTime()
    {

        tid -= 2;
        if (tid < 0) tid = 0;
        timerDisplay2.gameObject.SetActive(true);
        StartCoroutine(timeLossThingy(timerDisplay2.gameObject));
    }
    public IEnumerator timeLossThingy(GameObject timerDisplay2)
    {
        yield return new WaitForSeconds(1f);
        timerDisplay2.gameObject.SetActive(false);
    }
   

}
