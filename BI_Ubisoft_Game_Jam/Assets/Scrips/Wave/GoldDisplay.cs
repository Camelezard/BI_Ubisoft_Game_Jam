using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class GoldDisplay : MonoBehaviour
{
   [SerializeField] private TMP_Text _goldText;
   
   [Header("Settings")]
   [SerializeField] private AnimationCurve _transparencyCurve;
   [SerializeField] private AnimationCurve _movementCurve;
   [SerializeField] private float _distance = 100f;
   [SerializeField] private float _time = 1f;
   
   private CanvasGroup _canvasGroup;
   
   public void StartAnim(int pGoldAmount)
   {
       _canvasGroup = GetComponent<CanvasGroup>();
       _goldText.text = $"+{pGoldAmount}";
       
       StartCoroutine(AnimationCoroutine());
   }
   
   private IEnumerator AnimationCoroutine()
    {
        float lElapsedTime = 0f;
        
        _canvasGroup.alpha = 0f;
        Vector3 lStartPos = transform.position;
        Vector3 lEndPos = lStartPos + Vector3.up * _distance;
        
        while (lElapsedTime < _time)
        {
            lElapsedTime += Time.unscaledDeltaTime;
            _canvasGroup.alpha = _transparencyCurve.Evaluate(lElapsedTime / _time);
            transform.position = Vector3.Lerp(lStartPos, lEndPos, _movementCurve.Evaluate(lElapsedTime / _time));
            
            yield return new WaitForEndOfFrame();
        }
        
        yield return null;
    }
}
