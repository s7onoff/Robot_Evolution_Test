﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotOM;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Robot_Evolution_archive
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

    class Program_test
    {
        static void __Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            var rnd = new Random();

            var robotApplication = new RobotOM.RobotApplication();

            var proj = robotApplication.Project;
            var structure = proj.Structure;

            var dir = @"C:\Users\Sesemenov\Documents\!tmp\2022-08-11";

            var rtdFile = Path.Combine(dir, "Rombic_beams_20220808_00.rtd");
            var rndFile = Path.Combine(dir, "Rombic_beams_20220808_" + rnd.Next(100).ToString() + ".rtd");

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
            // var labels = structure.Labels.GetAll();
            stopwatch.Stop();
            Console.WriteLine("Creating bar -> " + stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();


            stopwatch.Start();
            proj.SaveAs(rndFile);
            stopwatch.Stop();
            Console.WriteLine("SaveAs Time = " + stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();

             proj.Close();

        }
    }
}
