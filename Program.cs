﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Spatial;
using RobotOM;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Robot_Evolution
{
    #region enums

    public enum BoundaryArc
    {
        Inner,
        Outer
    }

    public enum BoundaryLine
    {
        Free1L,
        Free2L,
        Fixed1L,
        Fixed2L,
        Fixed3L,
        Free1R,
        Free2R,
        Fixed1R,
        Fixed2R,
        Fixed3R
    }

    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            
            var stopwatch = new Stopwatch();
            var rnd = new Random();

            var robotApplication = new RobotOM.RobotApplication();

            var structure = robotApplication.Project.Structure;
            var proj = robotApplication.Project;

            var dir = @"C:\Users\Sesemenov\Documents\!tmp\2022-08-11";

            var rtdFile = Path.Combine(dir, "Slab_Evo_00.rtd");
            var rndFile = Path.Combine(dir, "Slab_Evo_" + rnd.Next(1000).ToString() + ".rtd");

            proj.Open(rtdFile);

            stopwatch.Start();

            var nodes = structure.Nodes.GetAll();
            stopwatch.Stop();

            Console.WriteLine("Reading joints -> " + stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();

            stopwatch.Start();



            var listNodes = new List<IRobotNode>();
            var listNodeNumbers = new List<int>();

            for (int i = 1; i < (nodes.Count) + 1; i++)
            {
                var node = (IRobotNode)nodes.Get(i);
                if (!node.IsCalc)
                {
                    listNodes.Add(node);
                    listNodeNumbers.Add(node.Number);
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Reading all joints and numbers -> " + stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();


            stopwatch.Start();
            var barNum = structure.Bars.FreeNumber;
            stopwatch.Stop();
            Console.WriteLine("Reading bar free number -> " + stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();

            stopwatch.Start();
            var nodeNum1 = structure.Nodes.FreeNumber;
            structure.Nodes.Create(nodeNum1, rnd.NextDouble(), rnd.NextDouble(), 5000.0);
            var nodeNum2 = structure.Nodes.FreeNumber;
            structure.Nodes.Create(nodeNum2, rnd.NextDouble(), rnd.NextDouble(), 5000.0);
            stopwatch.Stop();
            Console.WriteLine("Creating 2 rnd nodes -> " + stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();

            stopwatch.Start();
            structure.Bars.Create(barNum, nodeNum1, nodeNum2);
            stopwatch.Stop();
            Console.WriteLine("Creating bar -> " + stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();

            stopwatch.Start();
            var labels = structure.Labels.GetAll();
            stopwatch.Stop();
            Console.WriteLine("Getting labels -> " + stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();

            stopwatch.Start();

            var beamSecLabel = structure.Labels.Create(IRobotLabelType.I_LT_BAR_SECTION, "I60B1");
            var beamSection = beamSecLabel.Data;

            beamSection.ShapeType = IRobotBarSectionShapeType.I_BSST_USER_I_BISYM;
            var bar11 = structure.Bars.Get(11);

            var a = proj.Preferences.GetCurrentDatabase(IRobotDatabaseType.I_DT_SECTIONS);
            proj.Preferences.SetCurrentDatabase(IRobotDatabaseType.I_DT_SECTIONS, "STO");
            beamSection.LoadFromDBase("60ДБ1");
            var labelServer = structure.Labels;
            labelServer.Store(beamSecLabel);

            bar11.SetLabel(IRobotLabelType.I_LT_BAR_SECTION, "I60B1");

            stopwatch.Stop();
            Console.WriteLine("Setting new section -> " + stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();


            stopwatch.Start();
            proj.SaveAs(rndFile);
            stopwatch.Stop();
            Console.WriteLine("SaveAs Time = " + stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();

            proj.Close();

            RobotInitialMethods.Start();
            Console.ReadLine();
        }
    }
}
