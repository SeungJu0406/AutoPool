using UnityEngine;
using UnityEngine.UI;

namespace AutoPool_Tool
{
    public class ObjectCountController : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private Slider _slider;
        [SerializeField] private SamplePoolSpawner1 _spawner;

        private void Start()
        {
            _slider.onValueChanged.AddListener(SetCount);
            _slider.value = 100;
        }


        private void SetCount(float value)
        {
            int count = (int)value;
            if (count < 0)
            {
                Debug.LogError("Count cannot be negative.");
                return;
            }
            _spawner.SetCount(count);
            UpdateText();
        }

        private void UpdateText()
        {
            _text.text = $"{_spawner.Count} / s";
        }
    }
}

