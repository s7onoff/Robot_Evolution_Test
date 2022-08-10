using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RobotOM;

namespace Robot_Romb_Test_Archive
{
    class Archive
    {
        static void TutorialBeam()
        {
            var RobotApp = new RobotOM.RobotApplication();

            RobotApp.Project.New(IRobotProjectType.I_PT_FRAME_2D);

            RobotApp.Project.Structure.Nodes.Create(1, 0, 0, 0);
            RobotApp.Project.Structure.Nodes.Create(2, 3, 0, 0);

            RobotApp.Project.Structure.Bars.Create(1, 1, 2);

            var fixedSupportLabel = RobotApp.Project.Structure.Labels.Create(IRobotLabelType.I_LT_SUPPORT, "Fixed");

            var supportData = fixedSupportLabel.Data;

            supportData.UX = 1;
            supportData.UY = 1;
            supportData.UZ = 1;
            supportData.RX = 1;
            supportData.RY = 1;
            supportData.RZ = 1;

            RobotApp.Project.Structure.Labels.Store(fixedSupportLabel);

            RobotApp.Project.Structure.Nodes.Get(1).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Fixed");
            RobotApp.Project.Structure.Nodes.Get(2).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Fixed");

            var beamSecLabel = RobotApp.Project.Structure.Labels.Create(IRobotLabelType.I_LT_BAR_SECTION, "BeamSec");
            var beamSection = beamSecLabel.Data;

            beamSection.ShapeType = IRobotBarSectionShapeType.I_BSST_CONCR_BEAM_RECT;

            var concrete = beamSection.concrete;
            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_BEAM_B, 0.5);
            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_BEAM_H, 0.5);
            beamSection.CalcNonstdGeometry();
            RobotApp.Project.Structure.Labels.Store(beamSecLabel);

            RobotApp.Project.Structure.Bars.Get(1).SetLabel(IRobotLabelType.I_LT_BAR_SECTION, "BeamSec");


            var caseSW = RobotApp.Project.Structure.Cases.CreateSimple(1, "SW", IRobotCaseNature.I_CN_PERMANENT, IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            caseSW.Records.New(IRobotLoadRecordType.I_LRT_DEAD);
            var LoadRec = caseSW.Records.Get(1);
            LoadRec.SetValue((short)IRobotDeadRecordValues.I_DRV_Z, -1);
            LoadRec.SetValue((short)IRobotDeadRecordValues.I_DRV_ENTIRE_STRUCTURE, 1.0);

            if (RobotApp.Project.CalcEngine.Calculate() != 0)
            {
                Console.WriteLine(RobotApp.Project.Structure.Results.Bars.Forces.Value(1, 1, 0.5).MY);
                Console.Read();
            }

            RobotApp.Project.SaveAs("C:\\Users\\Sesemenov\\Documents\\!tmp\\2022-08-08\\rtd.rtd");
            RobotApp.Project.Close();
        }


        static void Rombic_test()
        {
            var robotApplication = new RobotOM.RobotApplication();

            var proj = robotApplication.Project;
            var structure = proj.Structure;

            var dir = @"\\Fs-project\PS\01_PROJECTS_WD\2515_Lakhta_2_3\07_KM\02_CALC\03_FEA\00_Preliminary_calcs\01_Slab_Beams\API_playing";

            var rtdFile = Path.Combine(dir, "Rombic_beams_20220808_00.rtd");

            proj.Open(rtdFile);

            var node1 = (IRobotNode)structure.Nodes.Get(31);
            var node2 = (IRobotNode)structure.Nodes.Get(32);
            var nodeForControl = (IRobotNode)structure.Nodes.Get(16);
            var horizontalX = ((IRobotNode)structure.Nodes.Get(13)).X;

            var amplitude = Math.Abs(horizontalX - nodeForControl.X) / 3;

            var nodeCoordinates = new List<(double X1, double X2)>();
            var results = new List<(double X1, double X2, double displacement)>();
            var step = 0.05;


            for (double deltaX1 = -amplitude + step; deltaX1 < amplitude; deltaX1 += step)
            {
                for (double deltaX2 = deltaX1 + step; deltaX2 < 2 * amplitude; deltaX2 += step)
                {
                    nodeCoordinates.Add((horizontalX + deltaX1, horizontalX + deltaX2));
                }
            }

            var minX1 = 0.0;
            var minX2 = 0.0;
            var minDeflection = 0.0;

            for (int i = 0; i < nodeCoordinates.Count(); i++)
            {
                node1.X = nodeCoordinates[i].X1;
                node2.X = nodeCoordinates[i].X2;

                if (proj.CalcEngine.Calculate() != 0)
                {
                    var deflection = structure.Results.Nodes.Displacements.Value(16, 1).UZ;
                    results.Add((nodeCoordinates[i].X1, nodeCoordinates[i].X2, deflection));
                    if (Math.Abs(deflection) > Math.Abs(minDeflection))
                    {
                        minX1 = nodeCoordinates[i].X1;
                        minX2 = nodeCoordinates[i].X2;
                        minDeflection = deflection;
                    }

                }
            }

            var csv = new StringBuilder();

            for (int i = 0; i < results.Count(); i++)
            {
                var newLine = string.Format("{0}, {1}, {2}", results[i].X1, results[i].X2, results[i].displacement);
                csv.AppendLine(newLine);
            }

            File.WriteAllText(Path.Combine(dir, "results.csv"), csv.ToString());

            node1.X = minX1;
            node2.X = minX2;
            proj.Save();
            proj.Close();
        }
    }
}
