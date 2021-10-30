using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Trapy Run/Level Database"), InlineEditor]
public class LevelDatabase : ScriptableObject
{
    public List<Level> LevelDB = new List<Level>();
}
