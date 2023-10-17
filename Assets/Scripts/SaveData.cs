public abstract class SaveData
{
    public int Version { get; set; }

    public abstract SaveData VersionUp();
}

public class SaveDataV1 : SaveData
{
    public SaveDataV1()
    {
        Version = 1;
    }

    public int stageLev {  get; set; }
    public int swordLev {  get; set; }
    public int axeLev { get; set; }
    public int spearLev { get; set; }
    public int atkSpUpLev { get; set; }
    public int movSpUpLev { get; set; }

    public override SaveData VersionUp()
    {
        var data = new SaveDataV2();
        data.stageLev = stageLev;
        data.swordLev = swordLev;
        data.axeLev = axeLev;
        data.spearLev = spearLev;
        data.atkSpUpLev = atkSpUpLev;
        data.movSpUpLev= movSpUpLev;
        return data;
    }
}

public class SaveDataV2 : SaveData
{
    public SaveDataV2()
    {
        Version = 2;
    }

    public int stageLev { get; set; }
    public int swordLev { get; set; }
    public int axeLev { get; set; }
    public int spearLev { get; set; }
    public int atkSpUpLev { get; set; }
    public int movSpUpLev { get; set; }
    public int maxHpUp { get; set; }

    public override SaveData VersionUp()
    {
        return null;
    }
}