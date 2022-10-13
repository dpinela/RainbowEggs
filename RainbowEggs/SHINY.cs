using UnityEngine;
using UnityEngine.UI;

namespace RainbowEggs
{
    internal class SHINY : MonoBehaviour
    {
        private float hue = 0;
        private Text text;

        private const float PeriodSecs = 4.0f;

        public void Awake()
        {
            text = gameObject.GetComponent<Text>();
        }

        public void Update()
        {
            text.color = Color.HSVToRGB(hue, 0.4f, 1);
            hue += Time.deltaTime / PeriodSecs;
            if (hue > 1)
            {
                hue -= 1;
            }
        }
    }
}