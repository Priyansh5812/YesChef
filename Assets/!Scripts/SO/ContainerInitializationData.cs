using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "ContainerInitializationData", menuName = "Scriptable Objects/ContainerInitializationData")]
public class ContainerInitializationData : ScriptableObject
{
    public List<KitchenItem> initItems;
}
