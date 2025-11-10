using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] private float baseOrderTime = 60f;
    public float remainingTime;
    private bool isCounting = false;
    private bool alertTriggered = false;
    private bool isDelivered = false;

    private const float AlertLimitTime = 10f;

    void Start()
    {
        Setup();
        StartCountdown();
    }

    public void Setup()
    {
        remainingTime = baseOrderTime;
    }

    void Update()
    {
        if (!isCounting || isDelivered) return;

        remainingTime -= Time.deltaTime;

        if (!alertTriggered && remainingTime <= AlertLimitTime)
        {
            alertTriggered = true;
        }

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isCounting = false;
        }
    }

    public void StartCountdown()
    {
        isCounting = true;   
    }
    public void StopCountdown()
    {
        isCounting = false;
    }
        public void ResumeCountdown()
    {
        isCounting = true;
    }

    public void SetDelivered()
    {
        isDelivered = true;
        StopCountdown();
        //GameObject.Instantiate.PlayGameManager.AddScore();
    }
}