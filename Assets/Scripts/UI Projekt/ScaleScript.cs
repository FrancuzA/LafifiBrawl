using UnityEngine;
using UnityEngine.UI;

public class ScaleScript : MonoBehaviour
{
    private Slider _slider;
    public CanvasScaler canvasScaler;

    public void ChangeScale(float newScale)
    {
        var scale = _slider.value;
        canvasScaler.scaleFactor = scale;
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        _slider = GetComponent<Slider>();
    }
}
