using UnityEngine;
using UnityEngine.UI;

namespace MoraeGames.Library.View
{
    public class SliderView : MonoBehaviour
    {
        #region Field

        Slider sliderUI;

        #endregion

        #region Unity Lifecycle

        private void Awake() => sliderUI = GetComponent<Slider>();

        #endregion

        #region Public Methods

        public void UpdateUI(float value)
        {
            sliderUI.value = value;
            if (sliderUI.value <= 0) sliderUI.fillRect.gameObject.SetActive(false);
            else sliderUI.fillRect.gameObject.SetActive(true);
        }

        #endregion
    }
}