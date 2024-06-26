using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeTriggerer : MonoBehaviour
{
    private float distanceToKnife;
    private bool isAlreadyCloseToKnife;
    private int knifeTriggerRange = 10;
    KnifeTrigger[] knifeTriggerList = null;
    private Dictionary<string, bool> knifeActiveDict = new Dictionary<string, bool>();
    // Start is called before the first frame update
    void Start()
    {
        knifeTriggerList = FindObjectsOfType(typeof(KnifeTrigger)) as KnifeTrigger[];
        knifeActiveDict.Add("knife1", false);
        knifeActiveDict.Add("knife2", false);
        knifeActiveDict.Add("knife3", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        foreach (KnifeTrigger knifeTrigger in knifeTriggerList)
        {
            if (knifeTrigger != null)
            {
                distanceToKnife = Vector3.Distance(transform.position, knifeTrigger.gameObject.transform.position);
                if (distanceToKnife < knifeTriggerRange && !knifeActiveDict[knifeTrigger.gameObject.name])
                {
                    EventManager.TriggerEvent<KnifeTriggerEvent, string, bool>(knifeTrigger.gameObject.name, true);
                    knifeActiveDict[knifeTrigger.gameObject.name] = true;
                }
                else if (distanceToKnife >= knifeTriggerRange && knifeActiveDict[knifeTrigger.gameObject.name])
                {
                    EventManager.TriggerEvent<KnifeTriggerEvent, string, bool>(knifeTrigger.gameObject.name, false);
                    knifeActiveDict[knifeTrigger.gameObject.name] = false;
                }
            }
        }
    }
}
