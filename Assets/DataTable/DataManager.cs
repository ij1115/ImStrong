using System.Collections.Generic;
using UnityEngine;

public static class DataManager
{
    private static Dictionary<System.Type, DataTable> tables = new Dictionary<System.Type, DataTable>();

    static DataManager()
    {
        tables.Clear();

        var itemTable = new ItemTable();
        tables.Add(typeof(ItemTable), itemTable);
    }

    public static T GetTable<T>() where T : DataTable
    {
        var id = typeof(T);
        if (!tables.ContainsKey(id))
        {
            return null;
        }

        return tables[id] as T;
    }
}