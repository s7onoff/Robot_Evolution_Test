using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RobotOM;
using static System.Net.WebRequestMethods;

namespace Robot_Evolution
{
    public static class RobotInteraction
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Robot logging");

        public static RobotOM.RobotApplication RobotApplication = new RobotOM.RobotApplication();
        public static RobotProject Project = RobotApplication.Project;
        public static RobotStructure RobotStructure = Project.Structure;

        static Random random = new Random();
        public static void Start()
        {
            Logger.Info("Started");
            Project.Open(InitialData.OriginalFile);
            CreateSections();
            Project.CalcEngine.AnalysisParams.IgnoreWarnings = true;
            Project.Structure.ResultsFreeze = false;
            Project.CalcEngine.AutoFreezeResults = false;
            Project.CalcEngine.GenerationParams.GenerateNodes_DiagonalBars = true;
        }

        public static void Finish()
        {
            Logger.Info("Finished");
            Project.Close();
        }

        public static Result CalcResult()
        {
            Logger.Info("Calculating");
            var result = new Result();
            var calculationStatus = Project.CalcEngine.CalculateEx(IRobotCalculationMode.I_CM_LOCAL);
            result.RobotCalculationStatus = calculationStatus;

            if (calculationStatus == IRobotCalculationStatus.I_CS_COMPLETED)
            {
                result.Deflection = RobotStructure.Results.Nodes.Displacements.Value(InitialData.DeflectionMonitoringRobotId, 2).UZ;
                result.Weight = RobotStructure.Results.Total.GetMass(1);
            }

            return result;
        }

        public static void SaveAs(Instance instance)
        {
            var filename = "_g_" + instance.GenerationID + "_i_" + instance.ID + ".rtd";
            Logger.Info("File {filename} saved", filename);
            var path = Path.Combine(InitialData.WorkingDirectory, filename);
            Project.SaveAs(path);
        }

        public static void CheckIntegrity(Instance instance, string phase)
        {
            //TODO: after finding all bugs, turn this off to speed up algorythm
            Logger.Info("Checking integrity for gen: {0}, instance: {1}. {2}", instance.GenerationID, instance.ID, phase);
            // needed to control bugs
            // var filename = "_g_" + instance.GenerationID + "_i_" + instance.ID + "_" + phase + ".rtd";
            // var path = Path.Combine(InitialData.WorkingDirectory, filename);
            // Project.SaveAs(path);
            var originalBeams = Instance.OriginalBeams.Select(b => b.RobotID);
            var allBeams = RobotStructure.Bars.GetAll();
            var beamsCollection = new List<int>();
            for (int i = 1; i < (allBeams.Count) + 1; i++)
            {
                var beam = (IRobotBar)allBeams.Get(i);
                Logger.Debug("beam read with number {0}", beam.Number);
                beamsCollection.Add(beam.Number);
            }
            if (!originalBeams.Except(beamsCollection).Any())
            {
                Logger.Error("Original beams are not the same!");
                Logger.Info("Original beams: {0}", string.Join(", ", originalBeams));
                Logger.Info("beamsCollection: {0}", string.Join(", ", beamsCollection));
            }
        }

        public static void CreateSections()
        {
            Logger.Info("Creating sections");
            foreach (var section in Sections.SectionsToUse)
            {
                Logger.Info("Section {0} | robotDBName: {1}", section.Name, section.NameInRobotDB);
                var beamSecLabel = RobotStructure.Labels.Create(IRobotLabelType.I_LT_BAR_SECTION, section.Name);
                var beamSection = beamSecLabel.Data;

                beamSection.ShapeType = IRobotBarSectionShapeType.I_BSST_USER_I_BISYM;

                Project.Preferences.SetCurrentDatabase(IRobotDatabaseType.I_DT_SECTIONS, "STO");

                beamSection.LoadFromDBase(section.NameInRobotDB);
                var labelServer = RobotStructure.Labels;
                labelServer.Store(beamSecLabel);
                Logger.Info("Stored");
            }
        }

        #region MethodsToRead

        public static List<(int Number, double X, double Y)> ReadNodes()
        {
            Logger.Info("Reading all nodes");
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
            Logger.Info("Nodes added: {0}", string.Join(", ", nodesCollection.Select(n => n.Number)));
            return nodesCollection;
        }

        public static List<(int Number, int Node1, int Node2, string SectionLabel)> ReadBeams()
        {
            Logger.Info("Reading all beams");
            var allBeams = RobotStructure.Bars.GetAll();
            var beamsCollection = new List<(int Number, int Node1, int Node2, string SectionLabel)>();
            for (int i = 1; i < (allBeams.Count) + 1; i++)
            {
                var beam = (IRobotBar)allBeams.Get(i);
                Logger.Debug("beam read with number {0}", beam.Number);
                beamsCollection.Add((beam.Number, beam.StartNode, beam.EndNode, (beam.GetLabel(IRobotLabelType.I_LT_BAR_SECTION)).Name));
            }
            Logger.Info("Beams added: {0}", string.Join(", ", beamsCollection.Select(b => b.Number)));
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
            Logger.Info("Deleting mutations");
            var nodesSelector = RobotStructure.Selections.Create(IRobotObjectType.I_OT_NODE);
            var mutatedNodes = string.Join(" ", instance.MutatedNodes.Select(node => node.RobotID));
            Logger.Info("mutatedNodes: {0}", mutatedNodes);
            nodesSelector.FromText(mutatedNodes);
            Logger.Info("Nodes to be deleted: {0}", nodesSelector.ToText());
            RobotStructure.Nodes.DeleteMany(nodesSelector);
            nodesSelector.Clear();

            var barsSelector = RobotStructure.Selections.Create(IRobotObjectType.I_OT_BAR);
            var mutatedBeams = string.Join(" ", instance.MutatedBeams.Select(beam => beam.RobotID));
            Logger.Info("mutatedBeams: {0}", mutatedBeams);
            barsSelector.FromText(mutatedBeams);
            var d = barsSelector.ToText();
            Logger.Info("Bars to be deleted: {0}", barsSelector.ToText());
            RobotStructure.Bars.DeleteMany(barsSelector);
            barsSelector.Clear();
        }

        public static void AddMutations(Instance instance)
        {
            Logger.Info("Adding mutations");
            foreach (var node in instance.MutatedNodes)
            {
                int nn = random.Next(100, 300);
                node.RobotID = nn;
                var x = node.X;
                var y = node.Y;
                Logger.Info("Adding node {0} ({1}|{2})", node.RobotID, node.X, node.Y);
                RobotStructure.Nodes.Create(nn, x, y, 5000.0);
            }

            foreach (var beam in instance.MutatedBeams)
            {
                beam.RobotID = random.Next(100, 300);
                RobotStructure.Bars.Create(beam.RobotID, beam.Node1.RobotID, beam.Node2.RobotID);
                Logger.Info("Adding beam {0} ({1}|{2})", beam.RobotID, beam.Node1.RobotID, beam.Node2.RobotID);
                var bar = (IRobotBar)RobotStructure.Bars.Get(beam.RobotID);
                bar.SetSection(beam.Section.Name, false);
            }
        }

        #endregion MethodsToWrite

    }
}