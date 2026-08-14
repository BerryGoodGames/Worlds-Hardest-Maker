using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class HelpPopupGifWebGLApology : MonoBehaviour
{
    [SerializeField] [MustBeAssigned] private GameObject webGLApologyContainer;
    [SerializeField] [MustBeAssigned] private RawImage videoImage;
    
    private void Start()
    {
        bool inWebGL = Application.platform == RuntimePlatform.WebGLPlayer;
        
        webGLApologyContainer.gameObject.SetActive(inWebGL);
        videoImage.gameObject.SetActive(!inWebGL);
    }
}