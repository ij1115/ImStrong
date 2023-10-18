using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SOItem : ScriptableObject
{
    [System.Serializable]
    public struct Item
    {
        public GameObject obj;
        public int weight;
    }

    public List<Item> items = new List<Item>();

    protected GameObject DropItem()
    {
        int sum = 0;

        foreach(var item in items)
        {
            sum += item.weight;
        }

        var rnd = Random.Range(0, sum);

        for(int i = 0; i< items.Count; i++)
        {
            var item = items[i];
            if(item.weight >rnd)
            {
                return items[i].obj;
            }
            else
            {
                rnd -= item.weight;
            }
        }
        return null;
    }

    public void itemDrop(Vector3 pos)
    {
        var item = DropItem();
        if (item == null)
            return;

        var iObj = Instantiate(item, pos, Quaternion.identity);
    }
}
