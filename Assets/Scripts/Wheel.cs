using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Wheel : MonoBehaviour, IObserver
{
    [Header("WheelDesign")]
    [SerializeField] int numberOfSlices = 0;
    [SerializeField] GameObject sliceObject;
    [SerializeField] Color[] colors;
    [SerializeField] WheelContent[] contents; // Array of WheelContent ScriptableObjects

    [Header("WheelSpecifications")]
    [SerializeField] float initialSpinSpeed = 360f;
    [SerializeField] float deceleration = 30f;

    float currentSpinSpeed;
    bool isSpinning;
    bool isSkipping;
    float skipTime;
    float lastAngle;

    [Header("Output")]
    [SerializeField] TextMeshProUGUI output;
    float timer = 0;
    private WheelContent currentReward;

    private void Start()
    {
        GenerateWheel();
        GameManager.Instance.RegisterWheel(this);
        GameManager.Instance.RegisterObserver(this);
        lastAngle = transform.eulerAngles.z;  // Initialize lastAngle
    }

    private void GenerateWheel()
    {
        for (int i = 0; i < numberOfSlices; i++)
        {
            GameObject slice = Instantiate(sliceObject, transform);
            float sliceSize = 1f / numberOfSlices;
            float sliceRotation = (360f / numberOfSlices) * (i + 1);
            Image sliceImg = slice.GetComponent<Image>();
            slice.transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, (180f * sliceSize) - 90f);
            sliceImg.fillAmount = sliceSize;
            sliceImg.color = colors[i % colors.Length];
            TextMeshProUGUI contentText = slice.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            Image contentIcon = slice.transform.GetChild(0).GetComponentInChildren<Image>();
            contentText.text = contents[i % contents.Length].contentName;
            contentIcon.sprite = contents[i % contents.Length].icon;
            slice.transform.localRotation = Quaternion.Euler(0, 0, -sliceRotation);
        }
    }

    public void Spin()
    {
        currentSpinSpeed = initialSpinSpeed + Random.Range((initialSpinSpeed / 4) * -1, (initialSpinSpeed / 4));
        isSpinning = true;
        isSkipping = false;
        AudioManager.Instance.PlaySFX("Spin");
    }

    private void CheckReward()
    {
        for (int i = 0; i < numberOfSlices; i++)
        {
            float startRotation = (360 / numberOfSlices) * i;
            float endRotation = (360 / numberOfSlices) * (i + 1);
            if (transform.eulerAngles.z > startRotation && transform.eulerAngles.z <= endRotation)
            {
                currentReward = contents[i % contents.Length];
                FindObjectOfType<UIManager>().UpdateOutput(currentReward.contentName);
            }
        }
    }

    private void CheckAndPlaySound()
    {
        float anglePerSlice = 360f / numberOfSlices;
        float currentAngle = transform.eulerAngles.z;

        // Check if we passed a slice boundary
        if (Mathf.Abs(currentAngle - lastAngle) >= anglePerSlice)
        {
            lastAngle = currentAngle;
            AudioManager.Instance.PlaySFX("Tick");
        }
    }

    public void StartSkip()
    {
        isSkipping = true;
        skipTime = Random.Range(0.1f,0.5f);  // Set the skip duration
        currentSpinSpeed = initialSpinSpeed * 2;  // Increase the spin speed
        AudioManager.Instance.PlaySFX("Skip");
    }

    public bool IsSpinning()
    {
        return isSpinning;
    }

    public string GetCurrentReward()
    {
        return currentReward.contentName;
    }

    private void Update()
    {
        if (isSpinning)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                CheckReward();
                timer = 0;
            }

            transform.Rotate(new Vector3(0, 0, currentSpinSpeed * Time.deltaTime));

            CheckAndPlaySound();

            if (isSkipping)
            {
                skipTime -= Time.deltaTime;
                if (skipTime <= 0)
                {
                    isSkipping = false;
                    isSpinning = false;
                    currentSpinSpeed = 0;
                    CheckReward();
                    FindObjectOfType<UIManager>().ShowClaimPopup(currentReward); // Show the claim popup
                    AudioManager.Instance.PlaySFX("Reward");
                }
            }
            else
            {
                currentSpinSpeed -= deceleration * Time.deltaTime;
                if (currentSpinSpeed <= 0)
                {
                    currentSpinSpeed = 0;
                    CheckReward();
                    isSpinning = false;
                    FindObjectOfType<UIManager>().ShowClaimPopup(currentReward); // Show the claim popup
                    AudioManager.Instance.PlaySFX("Reward");
                }
            }
        }
    }

    public void OnNotify(string message)
    {
        if (message == "Spin")
        {
            Spin();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemoveObserver(this);
        }
    }
}
