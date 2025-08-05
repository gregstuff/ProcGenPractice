using UnityEngine;

[CreateAssetMenu(fileName = "DungeonOutputConfig", menuName = "Dungeon/OutputConfig")]
public class DungeonOutputConfigSO : ScriptableObject
{
    [SerializeField] private Renderer _levelLayoutDisplay; 
    public Renderer LevelLayoutDisplay => _levelLayoutDisplay;
}