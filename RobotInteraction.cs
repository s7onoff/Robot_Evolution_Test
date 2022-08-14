using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RobotOM;
using static System.Net.WebRequestMethods;

namespace Robot_Evolution
{
    public static class RobotInteraction
    {
        public static RobotOM.RobotApplication RobotApplication = new RobotOM.RobotApplication();
        public static RobotProject Project = RobotApplication.Project;
        public static RobotStructure RobotStructure = Project.Structure;

        static Random random = new Random();
        public static void Start()
        {
            Project.Open(InitialData.OriginalFile);
            CreateSections();
            Project.CalcEngine.AnalysisParams.IgnoreWarnings = true;
            Project.Structure.ResultsFreeze = false;
            Project.CalcEngine.AutoFreezeResults = false;
            
        }

        public static void Finish()
        {
            Project.Close();
        }

        public static Result CalcResult()
        {
            var result = new Result();
            if (Project.CalcEngine.Calculate() != 0)
            {
                result.Deflection = RobotStructure.Results.Nodes.Displacements.Value(InitialData.DeflectionMonitoringRobotId, 2).UZ;
                result.Weight = RobotStructure.Results.Total.GetMass(1);
            }
            return result;
        }

        public static void SaveAs(Instance instance)
        {
            var filename = "_g_" + instance.generationId + "_i_" + instance.id + ".rtd";
            var path = Path.Combine(InitialData.WorkingDirectory, filename);
            Project.SaveAs(path);
        }

        public static void CreateSections()
        {
            foreach (var section in Sections.SectionsToUse)
            {
                var beamSecLabel = RobotStructure.Labels.Create(IRobotLabelType.I_LT_BAR_SECTION, section.Name);
                var beamSection = beamSecLabel.Data;

                beamSection.ShapeType = IRobotBarSectionShapeType.I_BSST_USER_I_BISYM;

                Project.Preferences.SetCurrentDatabase(IRobotDatabaseType.I_DT_SECTIONS, "STO");

                beamSection.LoadFromDBase(section.NameInRobotDB);
                var labelServer = RobotStructure.Labels;
                labelServer.Store(beamSecLabel);
            }
        }

        #region MethodsToRead

        public static List<(int Number, double X, double Y)> ReadNodes()
        {
            var allNodes = RobotStructure.Nodes.GetAll();
            var nodesCollection = new List<(int Number, double X, double Y)>();

            for (int i = 1; i < (allNodes.Count) + 1; i++)
            {
                var node = (IRobotNode)allNodes.Get(i);
                if (!node.IsCalc)
                {
                    nodesCollection.Add((node.Number, node.X, node.Y));
                }
            }
            return nodesCollection;
        }

        public static List<(int Number, int Node1, int Node2, string SectionLabel)> ReadBeams()
        {
            var allBeams = RobotStructure.Bars.GetAll();

            var listOfBeams = new List<IRobotNode>();
            var listOfBeamNumbers = new List<int>();
            var beamsCollection = new List<(int Number, int Node1, int Node2, string SectionLabel)>();

            for (int i = 1; i < (allBeams.Count) + 1; i++)
            {
                var beam = (IRobotBar)allBeams.Get(i);
                beamsCollection.Add((beam.Number, beam.StartNode, beam.EndNode, (beam.GetLabel(IRobotLabelType.I_LT_BAR_SECTION)).Name));
            }
            return beamsCollection;
        }

        public static void ReadContour()
        {
            //TODO: Maybe realize
            //var contour = RobotStructure.
        }

        #endregion MethodsToRead

        #region MethodsToWrite

        public static void DeleteMutations(Instance instance)
        {
            var nodesSelector = RobotStructure.Selections.Create(IRobotObjectType.I_OT_NODE);
            string nodesRobotNumbers = "";
            foreach (var node in instance.MutatedNodes)
            {
                nodesRobotNumbers += node.RobotID.ToString();
                nodesRobotNumbers += " ";
            }
            nodesSelector.AddText(nodesRobotNumbers);
            RobotStructure.Nodes.DeleteMany(nodesSelector);

            var barsSelector = RobotStructure.Selections.Create(IRobotObjectType.I_OT_BAR);
            string beamsRobotNumbers = "";
            foreach (var beam in instance.MutatedBeams)
            {
                beamsRobotNumbers += beam.RobotID.ToString();
                beamsRobotNumbers += " ";
            }
            barsSelector.AddText(beamsRobotNumbers);
            RobotStructure.Bars.DeleteMany(nodesSelector);
        }

        public static void AddMutations(Instance instance)
        {
            foreach (var node in instance.MutatedNodes)
            {
                // var n = RobotStructure.Nodes.FreeNumber;
                int nn = random.Next(100, 200);
                node.RobotID = nn;
                var x = node.X;
                var y = node.Y;
                RobotStructure.Nodes.Create(nn, x, y, 5000.0);
            }

            foreach (var beam in instance.MutatedBeams)
            {
                var n = RobotStructure.Bars.FreeNumber;
                beam.RobotID = n;
                RobotStructure.Bars.Create(n, beam.Node1.RobotID, beam.Node2.RobotID);
                var bar = (IRobotBar)RobotStructure.Bars.Get(n);
                bar.SetSection(beam.Section.Name, false);
            }
        }

        #endregion MethodsToWrite

    }
}