//Moshiro ist nen hurensohn
using UnityEngine;
using UnityEngine.UI;
using nanoSDK;


namespace nanoSDK {
    public class nanoSDK_GradientScroll : MonoBehaviour
    {
        public Toggle myToggle;
        public Text excludeText;
        public Gradient myGradient;
        public float strobeDuration = 5f;
        public Text go;


        public void FixedUpdate()
        {
            //if (UITextRainbow)
            {
                if (myToggle.isOn)
                {
                    float t = Mathf.PingPong(Time.time / strobeDuration, 1f);
                    go.color = myGradient.Evaluate(t);
                }
                else
                {
                    go.color = Color.white;
                    excludeText.color = Color.black;
                }
            }
        }
    }
}
