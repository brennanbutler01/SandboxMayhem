using UnityEngine;
using UnityEngine.Events;

public class KnifeTrigger : MonoBehaviour
{
    private Animator anim;
    public bool knifeIsActive = false;
    private int numberOfPlayersInRange = 0;
    private UnityAction<string, bool> knifeTriggerEventListener;
    public AudioEventManager audioEventManager;
    private void Awake()
    {
        anim = GetComponent<Animator>();

        knifeTriggerEventListener = new UnityAction<string, bool>(knifeTriggerEventHandler);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        EventManager.StartListening<KnifeTriggerEvent, string, bool>(knifeTriggerEventHandler);
    }

    private void OnDisable()
    {

        EventManager.StopListening<KnifeTriggerEvent, string, bool>(knifeTriggerEventHandler);
    }

    void knifeTriggerEventHandler(string knifeName, bool knifeShouldBeActive)
    {
        if (gameObject.name.Equals(knifeName) && knifeShouldBeActive)
        {
            numberOfPlayersInRange++;
            anim.Play("Knife Fall");
            anim.SetBool("isRotating", true);
            audioEventManager.playStopKnifeSwingAudtio(true);
        }
        else if (gameObject.name.Equals(knifeName) && !knifeShouldBeActive)
        {
            numberOfPlayersInRange--;
            if (numberOfPlayersInRange == 0)
            {
                anim.SetBool("isRotating", false);
                audioEventManager.playStopKnifeSwingAudtio(false);
            }
        }
    }
}
