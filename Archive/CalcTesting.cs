using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotOM;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace CalcTesting
{

    class CalcTesting
    {
        static void __Main(string[] args)
        {
            
            var robotApplication = new RobotOM.RobotApplication();
            var proj = robotApplication.Project;
            var robotStructure = proj.Structure;

            var dir = @"C:\Users\Sesemenov\Documents\!tmp\2022-08-15\";

            var rtdFile = Path.Combine(dir, "Slab_Slice_00.rtd");

            proj.Open(rtdFile);

            double deflection;
            var calc = proj.CalcEngine.CalculateEx(IRobotCalculationMode.I_CM_LOCAL);
            if (calc == IRobotCalculationStatus.I_CS_COMPLETED)
            {
                deflection = robotStructure.Results.Nodes.Displacements.Value(16, 2).UZ; // meters
                // var weight = robotStructure.Results.Total.GetMass(1);
            }

            proj.Close();

        }
    }
}
