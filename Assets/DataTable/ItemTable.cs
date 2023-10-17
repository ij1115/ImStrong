using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

public class ItemTable : DataTable
{
    public class Data
    {
        public int ITEM_NUMBER { get; set; }
        public string NAME { get; set; }
        public float VALUE { get; set; }
    }


    protected Dictionary<int, (string, float)> dic = new Dictionary<int, (string, float)>();


    public ItemTable()
    {
        path = "Table/ItemTable";
        Load();
    }

    public override void Load()
    {
        var csvStr = Resources.Load<TextAsset>(path);
        using (TextReader reader = new StringReader(csvStr.text))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            var records = csv.GetRecords<Data>();

            foreach (var record in records)
            {
                dic.Add(record.ITEM_NUMBER, (record.NAME, record.VALUE));
            }
        }
    }

    public (string, float) GetValue(int id)
    {
        if(!dic.ContainsKey(id))
        {
            return (null, 0.0f);
        }
        return dic[id];
    }
}