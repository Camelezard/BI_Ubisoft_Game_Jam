using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ItemDetailsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText, _descText, _moneyText;
    
    CanvasGroup _canvasGroup;
    
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        
        HouseShopButton.OnItemHover += OnItemHover;
        HouseShopButton.OnItemHoverEnd += OnItemHoverEnd;
    }
    
    private void OnItemHover(string pName, string pDesc, int pPrice)
    {
        _canvasGroup.alpha = 1f;
        _nameText.text = pName;
        _descText.text = pDesc;
        _moneyText.text = pPrice.ToString();
    }
    
    private void OnItemHoverEnd()
    {
        _canvasGroup.alpha = 0f;
    }
}
