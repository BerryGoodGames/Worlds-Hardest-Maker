using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class KeyFactory
{
    private readonly IObjectResolver diContainer;
    
    private KeyController grayKeyPrefab;
    private KeyController redKeyPrefab;
    private KeyController blueKeyPrefab;
    private KeyController greenKeyPrefab;
    private KeyController yellowKeyPrefab;

    private Transform keyContainer;

    public KeyFactory(IObjectResolver diContainer)
    {
        this.diContainer = diContainer;
    }

    public void Initialize(KeyController grayKeyPrefab,
        KeyController redKeyPrefab,
        KeyController blueKeyPrefab,
        KeyController greenKeyPrefab,
        KeyController yellowKeyPrefab,
        Transform keyContainer)
    {
        this.grayKeyPrefab = grayKeyPrefab;
        this.redKeyPrefab = redKeyPrefab;
        this.blueKeyPrefab = blueKeyPrefab;
        this.greenKeyPrefab = greenKeyPrefab;
        this.yellowKeyPrefab = yellowKeyPrefab;
        this.keyContainer = keyContainer;
    }
    
    public KeyController Create(Vector2 position, ISheet sheet, KeyColor keyColor)
    {
        KeyController key = Object.Instantiate(
            GetPrefabKey(keyColor),
            position, Quaternion.identity,
            sheet is AnchorSheet anchorSheet ? anchorSheet.Container : keyContainer
        );
        
        diContainer.InjectGameObject(key.gameObject);
        
        return key;
    }
    
    private KeyController GetPrefabKey(KeyColor color)
    {
        // TODO: apply OCP to key colors
        Dictionary<KeyColor, KeyController> prefabs = new()
        {
            { KeyColor.Gray, grayKeyPrefab },
            { KeyColor.Red, redKeyPrefab },
            { KeyColor.Blue, blueKeyPrefab },
            { KeyColor.Green, greenKeyPrefab },
            { KeyColor.Yellow, yellowKeyPrefab },
        };
        
        return prefabs[color];
    }
}