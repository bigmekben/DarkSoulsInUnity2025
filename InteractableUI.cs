using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SG
{
    public class InteractableUI : MonoBehaviour
    {
        public TextMeshProUGUI interactableText; // appears when player is in range, before player clicks button
        public TextMeshProUGUI itemText;
        public RawImage itemImage;
    }
}
