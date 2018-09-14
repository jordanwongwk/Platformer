using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class ButtonHandler : MonoBehaviour
    {

        public string Name;

        void OnEnable()
        {

        }

        public void SetDownState()
        {
            CrossPlatformInputManager.SetButtonDown(Name);
            ButtonIsPressed();
        }


        public void SetUpState()
        {
            CrossPlatformInputManager.SetButtonUp(Name);
            ButtonIsReleased();
        }


        public void SetAxisPositiveState()
        {
            CrossPlatformInputManager.SetAxisPositive(Name);
            ButtonIsPressed();
        }


        public void SetAxisNeutralState()
        {
            CrossPlatformInputManager.SetAxisZero(Name);
            ButtonIsReleased();
        }


        public void SetAxisNegativeState()
        {
            CrossPlatformInputManager.SetAxisNegative(Name);
            ButtonIsPressed();
        }

        void ButtonIsPressed()
        {
            GetComponent<Image>().color = Color.grey;
        }

        void ButtonIsReleased()
        {
            GetComponent<Image>().color = Color.white;
        }

        public void Update()
        {

        }
    }
}
