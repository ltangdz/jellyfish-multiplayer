using UnityEngine;
using UnityEngine.UI;

public class WarningUI : MonoBehaviour
{
    [SerializeField] private Image warningImage;
    public Image WarningImage => warningImage;
    
    private void Start()
    {
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
