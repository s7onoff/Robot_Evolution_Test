using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RobotOM;

namespace Robot_Romb_Test_Archive
{
    class Rombic
    {
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
