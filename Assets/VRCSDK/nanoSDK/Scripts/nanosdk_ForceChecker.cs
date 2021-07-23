/* Working on maybe..
using System.IO;
using UnityEngine;

public class nanosdk_ForceChecker : MonoBehaviour
{
    public ParticleSystem trademarked;
    private void Start()
    {

        string path = "Assets\\VRCSDK\\nanoSDK\\Configs\\sdkProvider.txt";        

        if (!File.Exists(path))
        {
            while (true)
            {
                trademarked.gameObject.SetActive(true);
                ForceCheckingSystem();
            }
        }

    }

    private void ForceCheckingSystem()
    {
        Debug.LogError("FUCK YOU");
    }
}
*/
