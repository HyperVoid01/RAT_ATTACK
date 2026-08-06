using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private TMP_Text orderText;

    public static HUDManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public void EnableInteractionText(string text)
    {
        if (Input.GetMouseButton(0)) // Ignores enabling when holding items
            return;
        
        interactionText.text = text;
        interactionText.gameObject.SetActive(true);
    }

    public void DisableInteractionText()
    {
        interactionText.gameObject.SetActive(false);
    }

    public void AddOrderText(string text)
    {
        orderText.text += "\n" + text;
    }
}
