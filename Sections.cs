using System;

public static class Sections
{
    public static Section I20B = new Section("20B1", "20ДБ1");
    public static Section I40B = new Section("40B1", "40ДБ1");
    public static Section I60B = new Section("60B1", "60ДБ1");
    public static Section I70B = new Section("70B1", "70ДБ1");

}

public class Section
{
    public string Name { get; set; }
    public string NameInRobotDB { get; set; }

    public Section(string name, string nameDB)
    {
        Name = name;
        NameInRobotDB = nameDB;
    }
}
