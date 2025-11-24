using UnityEngine;

public class CreditsLinks : MonoBehaviour
{
    private const string _AndreasLink = "https://www.linkedin.com/in/andreas-martins/";
    private const string _DanielLink = "https://www.linkedin.com/in/daniel-degott-a6622b2ab/";
    private const string _JoachimLink = "https://www.linkedin.com/in/joachim-legrand2/";
    private const string _RalifLink = "https://www.linkedin.com/in/ralif-tazutdinov/";
    private const string _RaphaelLink = "https://www.linkedin.com/in/rapha%C3%ABl-leray-7b71132aa/";
    private const string _ReauldLink = "https://www.linkedin.com/in/rapha%C3%ABl-leray-7b71132aa/";
    
    public void OpenLink(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }

    // Méthodes pratiques pour UnityButton si tu veux assigner directement
    public void OpenAndreas() => OpenLink(_AndreasLink);
    public void OpenDaniel() => OpenLink(_DanielLink);
    public void OpenJoachim() => OpenLink(_JoachimLink);
    public void OpenRalif() => OpenLink(_RalifLink);
    public void OpenRaphael() => OpenLink(_RaphaelLink);
    public void OpenRenauld() => OpenLink(_ReauldLink);
}
