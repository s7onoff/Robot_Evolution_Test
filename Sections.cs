using System;
using System.Collections.Generic;

public static class Sections
{
    public static Section I20B = new Section("20B1", "20ДБ1");
    public static Section I40B = new Section("40B1", "40ДБ1");
    public static Section I60B = new Section("60B1", "60ДБ1");
    public static Section I70B = new Section("70B1", "70ДБ1");

    public static List<Section> SectionsToUse = 
        new List<Section>() { 
            I20B,
            I40B, 
            I60B, 
            I70B
        };

    internal static Section CreateSection(string sectionLabel)
    {
        var newSection = new Section(sectionLabel, sectionLabel);
        SectionsToUse.Add(newSection);
        return newSection;
    }
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
