using UnityEngine;
using UnityEngine.EventSystems;

namespace RicochetTanks.Input.Mobile
{
    public sealed class MobileFireButton : MonoBehaviour, IPointerDownHandler
    {
        private bool _wasPressed;

        public bool ConsumePressed()
        {
            if (!_wasPressed)
            {
                return false;
            }

            _wasPressed = false;
            return true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _wasPressed = true;
        }
    }
}
