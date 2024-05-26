using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class HelpPopupGifWebGLApology : MonoBehaviour
{
    [SerializeField] [Required] private GameObject webGLApologyContainer;
    [SerializeField] [Required] private RawImage videoImage;
    
    private void Start()
    {
        bool inWebGL = Application.platform == RuntimePlatform.WebGLPlayer;
        
        webGLApologyContainer.gameObject.SetActive(inWebGL);
        videoImage.gameObject.SetActive(!inWebGL);
    }
}
