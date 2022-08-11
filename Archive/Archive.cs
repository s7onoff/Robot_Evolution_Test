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
    }
}
