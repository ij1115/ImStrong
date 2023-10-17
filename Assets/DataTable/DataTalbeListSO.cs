using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataTableList", menuName = "ScriptableObjects/DataTableList")]
public class DataTableListSO : ScriptableObject
{
    public TextAsset[] csvFiles;
}