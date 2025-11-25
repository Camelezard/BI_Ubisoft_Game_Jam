using System.Collections.Generic;
using UnityEngine;

public class ShopUiManager : MonoBehaviour
{
    [SerializeField] private List< GameObject > _ShopSection;
    [SerializeField] private GameObject _ActifSection = null;

    public void ChangeSection(GameObject pSection )
    {
        _ActifSection.SetActive(false);
        _ActifSection = pSection;
        _ActifSection.SetActive(true);
    }
}
