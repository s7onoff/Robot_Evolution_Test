using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NetTopologySuite.Triangulate;
using RobotOM;
using static System.Net.WebRequestMethods;

namespace Robot_Evolution
{
    public static class RobotInteraction
    {
        public static readonly NLog.Logger RobotLogger = NLog.LogManager.GetLogger("Robot");

        public static RobotOM.RobotApplication RobotApplication = new RobotOM.RobotApplication();
        public static RobotProject Project = RobotApplication.Project;
        public static RobotStructure RobotStructure = Project.Structure;

        static Random random = new Random();
        public static void Start()
        {
            RobotLogger.Info("Started");
            Project.Open(InitialData.OriginalFile);
            CreateSections();
            Project.CalcEngine.AnalysisParams.IgnoreWarnings = true;
            Project.Structure.ResultsFreeze = false;
            Project.CalcEngine.AutoFreezeResults = false;
            Project.CalcEngine.GenerationParams.GenerateNodes_DiagonalBars = true;
        }

        public static void Finish()
        {
            RobotLogger.Info("Finished");
            Project.Close();
        }

        public static Result CalcResult()
        {
            RobotLogger.Info("Calculating");
            var result = new Result();
            var calculationStatus = Project.CalcEngine.CalculateEx(IRobotCalculationMode.I_CM_LOCAL);
            result.RobotCalculationStatus = calculationStatus;

            if (calculationStatus == IRobotCalculationStatus.I_CS_COMPLETED)
            {
                result.Deflection = RobotStructure.Results.Nodes.Displacements.Value(InitialData.DeflectionMonitoringRobotId, 2).UZ;
                
                var allNodes = RobotStructure.Nodes.GetAll();
                var allDeflections = new List<double>();

                for (int i = 1; i < allNodes.Count + 1; i++)
                {
                    var node = (IRobotNode)allNodes.Get(i);
                    if (!node.IsCalc)
                    {
                        var nodeNumber = node.Number;
                        allDeflections.Add(RobotStructure.Results.Nodes.Displacements.Value(nodeNumber, 2).UZ);
                    }
                }

                result.AverageDeflection = allDeflections.Average();
                
                result.Weight = RobotStructure.Results.Total.GetMass(1);
            }

            return result;
        }

        public static void SaveAs(Instance instance)
        {
            var filename = "_g_" + instance.GenerationID + "_i_" + instance.ID + ".rtd";
            RobotLogger.Info("File {filename} saved", filename);
            var path = Path.Combine(InitialData.WorkingDirectory, filename);
            Project.SaveAs(path);
        }

        public static void CheckIntegrity(Instance instance, string phase)
        {
            //TODO: after finding all bugs, turn this off to speed up algorythm
            RobotLogger.Info("Checking integrity for gen: {0}, instance: {1}. {2}", instance.GenerationID, instance.ID, phase);
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
                RobotLogger.Debug("beam read with number {0}", beam.Number);
                beamsCollection.Add(beam.Number);
            }
            if (originalBeams.Except(beamsCollection).Any())
            {
                RobotLogger.Error("Original beams are not the same!");
                RobotLogger.Info("Original beams: {0}", string.Join(", ", originalBeams));
                RobotLogger.Info("beamsCollection: {0}", string.Join(", ", beamsCollection));
                RobotLogger.Info("Diff: {0}", string.Join(", ", originalBeams.Except(beamsCollection.ToList())));
            }
            else
            {
                RobotLogger.Info("Ingtegrity ok");
            }
        }

        public static void CreateSections()
        {
            RobotLogger.Info("Creating sections");
            foreach (var section in Sections.SectionsToUse)
            {
                RobotLogger.Info("Section {0} | robotDBName: {1}", section.Name, section.NameInRobotDB);
                var beamSecLabel = RobotStructure.Labels.Create(IRobotLabelType.I_LT_BAR_SECTION, section.Name);
                var beamSection = beamSecLabel.Data;

                beamSection.ShapeType = IRobotBarSectionShapeType.I_BSST_USER_I_BISYM;

                Project.Preferences.SetCurrentDatabase(IRobotDatabaseType.I_DT_SECTIONS, "STO");

                beamSection.LoadFromDBase(section.NameInRobotDB);
                var labelServer = RobotStructure.Labels;
                labelServer.Store(beamSecLabel);
                RobotLogger.Info("Stored");
            }
        }

        #region MethodsToRead

        public static List<(int Number, double X, double Y)> ReadNodes()
        {
            RobotLogger.Info("Reading all nodes");
            var allNodes = RobotStructure.Nodes.GetAll();
            var nodesCollection = new List<(int Number, double X, double Y)>();

            for (int i = 1; i < allNodes.Count + 1; i++)
            {
                var node = (IRobotNode)allNodes.Get(i);
                if (!node.IsCalc)
                {
                    nodesCollection.Add((node.Number, node.X, node.Y));
                }
            }
            RobotLogger.Info("Nodes added: {0}", string.Join(", ", nodesCollection.Select(n => n.Number)));
            return nodesCollection;
        }

        public static List<(int Number, int Node1, int Node2, string SectionLabel)> ReadBeams()
        {
            RobotLogger.Info("Reading all beams");
            var allBeams = RobotStructure.Bars.GetAll();
            var beamsCollection = new List<(int Number, int Node1, int Node2, string SectionLabel)>();
            for (int i = 1; i < allBeams.Count + 1; i++)
            {
                var beam = (IRobotBar)allBeams.Get(i);
                RobotLogger.Debug("beam read with number {0}", beam.Number);
                beamsCollection.Add((beam.Number, beam.StartNode, beam.EndNode, (beam.GetLabel(IRobotLabelType.I_LT_BAR_SECTION)).Name));
            }
            RobotLogger.Info("Beams added: {0}", string.Join(", ", beamsCollection.Select(b => b.Number)));
            return beamsCollection;
        }

        public static List<(double x, double y)> ReadContour()
        {
            var allObjects = (RobotObjObjectCollection)RobotStructure.Objects.GetAll();

            var listOfPoints = new List<(double x, double y)>();

            for (int i = 1; i < allObjects.Count + 1; i++)
            {
                try
                {
                    var currentObject = (IRobotObjObject)allObjects.Get(i);
                    var currentPanelGeo = (RobotGeoContour)currentObject.Main.GetGeometry();
                    var segments = (RobotGeoSegmentCollection)currentPanelGeo.Segments;
                    for (int j = 1; j < segments.Count+1; j++)
                    {
                        var segment = segments.Get(j);
                        var x = segment.P1.X;
                        var y = segment.P1.Y;
                        listOfPoints.Add((x, y));
                    }
                    // to close the point ring
                    listOfPoints.Add(listOfPoints[0]);

                    // to read only first found contour
                    break;
                }
                catch
                {
                    continue;
                }
            }

            return listOfPoints;
        }

        #endregion MethodsToRead

        #region MethodsToWrite
        public static void DeleteMutations(Instance instance)
        {
            RobotLogger.Info("Deleting mutations");
            var nodesSelector = RobotStructure.Selections.Create(IRobotObjectType.I_OT_NODE);
            var mutatedNodes = string.Join(" ", instance.MutatedNodes.Select(node => node.RobotID));
            RobotLogger.Info("mutatedNodes: {0}", mutatedNodes);
            nodesSelector.FromText(mutatedNodes);
            RobotLogger.Info("Nodes to be deleted: {0}", nodesSelector.ToText());
            RobotStructure.Nodes.DeleteMany(nodesSelector);
            nodesSelector.Clear();

            var barsSelector = RobotStructure.Selections.Create(IRobotObjectType.I_OT_BAR);
            var mutatedBeams = string.Join(" ", instance.MutatedBeams.Select(beam => beam.RobotID));
            RobotLogger.Info("mutatedBeams: {0}", mutatedBeams);
            barsSelector.FromText(mutatedBeams);
            var d = barsSelector.ToText();
            RobotLogger.Info("Bars to be deleted: {0}", barsSelector.ToText());
            RobotStructure.Bars.DeleteMany(barsSelector);
            barsSelector.Clear();
        }

        public static void AddMutations(Instance instance)
        {
            RobotLogger.Info("Adding mutations");
            foreach (var node in instance.MutatedNodes)
            {
                int nn = random.Next(100, 600);
                node.RobotID = nn;
                var x = node.X;
                var y = node.Y;
                RobotLogger.Info("Adding node {0} ({1}|{2})", node.RobotID, node.X, node.Y);
                RobotStructure.Nodes.Create(nn, x, y, 5000.0);
            }

            foreach (var beam in instance.MutatedBeams)
            {
                beam.RobotID = random.Next(100, 600);
                RobotStructure.Bars.Create(beam.RobotID, beam.Node1.RobotID, beam.Node2.RobotID);
                RobotLogger.Info("Adding beam {0} ({1}|{2})", beam.RobotID, beam.Node1.RobotID, beam.Node2.RobotID);
                var bar = (IRobotBar)RobotStructure.Bars.Get(beam.RobotID);
                bar.SetSection(beam.Section.Name, false);
            }
        }

        #endregion MethodsToWrite

    }
}