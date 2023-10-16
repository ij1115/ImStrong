using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

public class SaveData : DataTable
{
    public class Data
    {
        public string key { get; set; }
        public int value { get; set; }
    }
    
    protected Dictionary<string, int> data = new Dictionary<string, int>();

    public SaveData()
    {
        path = SaveDirectory;
        Load();
    }

    public static string SaveDirectory
    {
        get
        {
            return "Save/SaveData";
        }
    }

    public static void Save(string SaveDirectory)
    {
        var csvStr = Resources.Load<TextAsset>(SaveDirectory);
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }
    }

    public override void Load()
    {
        
    }
}
