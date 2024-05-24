using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class TextEffect : MonoBehaviour
{
    [FormerlySerializedAs("textComponent")] [SerializeField] [Required] protected TMP_Text TextComponent;
    [SerializeField] private float speed = 1;
    
    private void Update()
    {
        TextComponent.ForceMeshUpdate();
        
        Mesh mesh = TextComponent.mesh;
        TMP_TextInfo textInfo = TextComponent.textInfo;
        Vector3[] vertices = TextComponent.mesh.vertices;
        
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            
            int index = charInfo.vertexIndex;
            
            Vector3 offset = AnimationOffset(speed * Time.time + i);
            
            for (int j = 0; j < 4; j++) { vertices[index + j] += offset; }
        }
        
        mesh.vertices = vertices;
        TextComponent.canvasRenderer.SetMesh(mesh);
    }
    
    protected abstract Vector2 AnimationOffset(float time);
}