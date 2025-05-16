using UnityEngine;

namespace edeastudio.Utils
{
    public class eMonoBehaviour : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private bool openCloseEvents;
        [SerializeField, HideInInspector]
        private bool openCloseWindow;
        [SerializeField, HideInInspector]
        private int selectedToolbar;
    }
}