using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
namespace NSJ_EasyPoolKit
{

    [DefaultExecutionOrder(-999)]
    public class AutoEventSystemConfigurator : MonoBehaviour
    {
        private void Awake()
        {
            EventSystem eventSystem = EventSystem.current;

            if (eventSystem == null)
            {
                eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            }

#if ENABLE_INPUT_SYSTEM
            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                Debug.Log("[AutoEventSystem] Attached: InputSystemUIInputModule");
            }
#else
        if (eventSystem.GetComponent<StandaloneInputModule>() == null)
        {
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            Debug.Log("[AutoEventSystem] Attached: StandaloneInputModule");
        }
#endif
        }
    }
}
