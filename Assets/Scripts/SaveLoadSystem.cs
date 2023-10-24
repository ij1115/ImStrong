using UnityEngine;
using System.IO;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Linq;
using SaveDataVC = SaveDataV3;

public class SaveDataV1Map : ClassMap<SaveDataV1>
{
    public SaveDataV1Map()
    {
        Map(m => m.Version).Name("Version");
        Map(m => m.stageLev).Name("stageLev");
        Map(m => m.swordLev).Name("swordLev");
        Map(m => m.axeLev).Name("axeLev");
        Map(m => m.spearLev).Name("spearLev");
        Map(m => m.atkSpUpLev).Name("atkSpUpLev");
        Map(m => m.movSpUpLev).Name("movSpUpLev");
    }
}

public class SaveDataV2Map : ClassMap<SaveDataV2>
{
    public SaveDataV2Map()
    {
        Map(m => m.Version).Name("Version");
        Map(m => m.stageLev).Name("stageLev");
        Map(m => m.swordLev).Name("swordLev");
        Map(m => m.axeLev).Name("axeLev");
        Map(m => m.spearLev).Name("spearLev");
        Map(m => m.atkSpUpLev).Name("atkSpUpLev");
        Map(m => m.movSpUpLev).Name("movSpUpLev");
        Map(m => m.maxHpUp).Name("maxHpUp");
    }
}

public class SaveDataV3Map : ClassMap<SaveDataV3>
{
    public SaveDataV3Map()
    {
        Map(m => m.Version).Name("Version");
        Map(m => m.stageLev).Name("stageLev");
        Map(m => m.swordLev).Name("swordLev");
        Map(m => m.axeLev).Name("axeLev");
        Map(m => m.spearLev).Name("spearLev");
        Map(m => m.atkSpUpLev).Name("atkSpUpLev");
        Map(m => m.movSpUpLev).Name("movSpUpLev");
        Map(m => m.maxHpUp).Name("maxHpUp");
        Map(m => m.name).Name("name");
    }
}

public static class SaveLoadSystem
{
    public static int SaveDataVersion { get; } = 3;

    public static string SavePath
    {
        get
        {
            return $"{Application.persistentDataPath}/SaveData.csv";
        }
    }

    public static void Save(SaveDataVC data)
    {
        if(data == null)
        {
            Debug.LogError("save data Lost");
            return;
        }

        using (var writer = new StreamWriter(SavePath))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.Context.RegisterClassMap<SaveDataV3Map>();
            csv.WriteHeader<SaveDataVC>();
            csv.NextRecord();
            csv.WriteRecord(data);
        }
    }

    public static SaveDataVC Load()
    {
        if(!File.Exists(SavePath))
        {
            var savedata = new SaveDataVC 
            { name = null, stageLev = 1, maxHpUp = 0, swordLev = 0,
                axeLev = 0, spearLev = 0, atkSpUpLev = 0, movSpUpLev = 0 };
            Save(savedata);
        }

        SaveData data = null;
        int version = GetFileVersion();

        using(var reader = new StreamReader(SavePath))
        using(var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            switch(version)
            {
                case 1:
                    csv.Context.RegisterClassMap<SaveDataV1Map>();
                    data = csv.GetRecords<SaveDataV1>().FirstOrDefault();
                    break;
                case 2:
                    csv.Context.RegisterClassMap<SaveDataV2Map>();
                    data = csv.GetRecords<SaveDataV2>().FirstOrDefault();
                    break;
                case 3:
                    csv.Context.RegisterClassMap<SaveDataV3Map>();
                    data = csv.GetRecords<SaveDataV3>().FirstOrDefault();
                    break;
            }

            while (data.Version < SaveDataVersion)
            {
                data = data.VersionUp();
                version++;
            }

            return (SaveDataVC)data;
        }
    }

    private static int GetFileVersion()
    {
        using (var reader = new StreamReader(SavePath))
        {
            reader.ReadLine();
            string number = reader.ReadLine();
            string[] columns = number.Split(',');
            if(columns.Length >0)
            {
                if (int.TryParse(columns[0], out int version))
                {
                    return version;
                }
            }
        }
        return 0;
    }
}