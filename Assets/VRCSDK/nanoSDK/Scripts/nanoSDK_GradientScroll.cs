//Moshiro ist nen hurensohn
using UnityEngine;
using UnityEngine.UI;
using nanoSDK;
using System.Collections.Generic;

namespace nanoSDK
{
    public class nanoSDK_GradientScroll : MonoBehaviour
    {
        public Toggle myToggle;
        public Text doBlackText;
        public Gradient myGradient;
        public float strobeDuration = 5f;
        public Text rainbowText;


        public void FixedUpdate()
        {
            try
            {
                if (doBlackText || rainbowText || myToggle == null)
                    //Ignore

                if (myToggle.isOn)
                {
                    float t = Mathf.PingPong(Time.time / strobeDuration, 1f);
                    rainbowText.color = myGradient.Evaluate(t);
                }
                else
                {
                    rainbowText.color = Color.white;
                    doBlackText.color = Color.black;

                }
            }
            catch (System.NullReferenceException)
            {
                //ignore
            }
        }
    }
}
