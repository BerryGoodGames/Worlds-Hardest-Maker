using UnityEngine;

[CreateAssetMenu(fileName = "SupabaseConfig", menuName = "ScriptableObjects/SupabaseConfig")]
public class SupabaseConfig : ScriptableObject
{ // do NOT put values hard coded in here, fill in scriptable object
    public string ProjectUrl;
    public string AnonKey;
}