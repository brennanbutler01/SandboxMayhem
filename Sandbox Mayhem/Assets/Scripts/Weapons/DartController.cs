using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class DartController : MonoBehaviour
    {
        private float speed = 40f;
        private Vector3 target = Vector3.forward;
        private Rigidbody rigidBody;
        private bool stuck = false;

        public float defaultSpeed = 40f;
        public Material fadeMaterialBarrel;
        public Material fadeMaterialTip;
        public Transform barrel;
        public Transform tip;
        
        public void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            barrel = transform.Find("Barrel");
            tip = transform.Find("Tip");
        }

        public void Update()
        {
            if (!stuck)
            {
                transform.position += Time.deltaTime * speed * target;
            }
        }

        public void SetTarget(Vector3 newTarget)
        {
            target = newTarget;
        }

        public void SetSpeed(float newSpeed)
        {
            speed = defaultSpeed + newSpeed;
        }

        public void RemoveDart()
        {
            stuck = false;
            target = -transform.up;
            speed = 1.5f;
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut(float duration = 0.75f)
        {
            float elapsed = 0;
            Renderer barrelRenderer = barrel.GetComponent<Renderer>();
            barrelRenderer.material = fadeMaterialBarrel;
            
            Renderer tipRenderer = tip.GetComponent<Renderer>();
            tipRenderer.material = fadeMaterialTip;

            while (elapsed < duration)
            {
                Color currentColorBody = fadeMaterialBarrel.color;
                Color currentColorTip = fadeMaterialTip.color;

                float alpha = Mathf.Lerp(1, 0, elapsed / duration);
                barrelRenderer.material.color = new Color(currentColorBody.r, currentColorBody.g, currentColorBody.b, alpha);
                tipRenderer.material.color = new Color(currentColorTip.r, currentColorTip.g, currentColorTip.b, alpha);

                elapsed += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                transform.SetParent(other.collider.transform);
                rigidBody.detectCollisions = false;
                rigidBody.isKinematic = true;
                stuck = true;
                CarController carController = other.gameObject.GetComponent<CarController>();
                carController.ApplyDartSpeedPenalty(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}