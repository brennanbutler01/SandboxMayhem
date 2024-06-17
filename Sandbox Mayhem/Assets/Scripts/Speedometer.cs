using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Speedometer : MonoBehaviour
{
    [SerializeField]
    private Image speedRingImage;
    [SerializeField]
    private Text speedDisplayText;

    [SerializeField]
    private CarController car;

    private float _displaySpeed;
    
    // Start is called before the first frame update
    private void Start()
    {
        speedRingImage.fillAmount = 0;
        speedDisplayText.text = "0";
        
        // make sure the image is type filled - else nothing happens when we speed up!
        if (speedRingImage.type == Image.Type.Filled) return;
        Debug.LogWarning("Speed ring will not render properly without the image being set to type=filled");

    }

    // Update is called once per frame
    private void Update()
    {
        // abs all speeds so we don't show negatives when going backwards
        _displaySpeed = Mathf.Abs(car.speed / car.maxSpeed);
        speedRingImage.fillAmount = _displaySpeed;
        speedDisplayText.text = Mathf.Abs(Mathf.Round(car.speed)).ToString(CultureInfo.CurrentCulture);
    }
}
