using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Speedometer : MonoBehaviour
{
    public Image speedRingImage;
    public Text speedDisplayText;
    public CarController car;

    private float _displaySpeed;
    private const float MaxDisplaySpeed = 220f;
    
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
        _displaySpeed = Mathf.Abs(car.speed / MaxDisplaySpeed);
        speedRingImage.fillAmount = _displaySpeed;
        speedDisplayText.text = Mathf.Abs(Mathf.Round(car.speed)).ToString(CultureInfo.CurrentCulture);
    }
}
