using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Spatial;
using RobotOM;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Robot_Evolution_archive
{
    class SpeedTesting
    {
        static void ArchiveMain(string[] args)
        {

            var robotApplication = new RobotOM.RobotApplication();

            var proj = robotApplication.Project;
            var structure = proj.Structure;

            var dir = @"C:\Users\Sesemenov\Documents\!tmp\2022-08-11";

            var rtdFile = Path.Combine(dir, "Rombic_beams_20220808_00.rtd");

            proj.Open(rtdFile);


            var stopwatch = new Stopwatch();

            for (int _ = 0; _ < 10; _++)
            {
                stopwatch.Start();

                var nodes = structure.Nodes.GetAll();

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
                Console.WriteLine("casting -> " + stopwatch.ElapsedMilliseconds);

                stopwatch.Reset();

                stopwatch.Start();

                var nodes2 = structure.Nodes.GetAll();

                var listNodes2 = new List<IRobotNode>();
                var listNodeNumbers2 = new List<int>();

                for (int i = 1; i < (nodes2.Count) + 1; i++)
                {
                    var node = nodes2.Get(i);
                    if (!node.IsCalc)
                    {
                        listNodes2.Add(node);
                        listNodeNumbers2.Add(node.Number);
                    }
                }

                stopwatch.Stop();
                Console.WriteLine("no casting -> " + stopwatch.ElapsedMilliseconds);

                stopwatch.Reset();


                stopwatch.Start();

                var roboNodes = (RobotNodeCollection)structure.Nodes.GetAll();

                var listRoboNodes = new List<IRobotNode>();
                var listRoboNodeNumbers = new List<int>();

                for (int i = 1; i < (roboNodes.Count) + 1; i++)
                {
                    var node = (IRobotNode)roboNodes.Get(i);
                    if (!node.IsCalc)
                    {
                        listRoboNodes.Add(node);
                        listRoboNodeNumbers.Add(node.Number);
                    }
                }

                stopwatch.Stop();
                Console.WriteLine("Robo casting -> " + stopwatch.ElapsedMilliseconds);

                stopwatch.Reset();


                stopwatch.Start();

                var roboNodes2 = (RobotNodeCollection)structure.Nodes.GetAll();

                var listRoboNodes2 = new List<IRobotNode>();
                var listRoboNodeNumbers2 = new List<int>();

                for (int i = 1; i < (roboNodes2.Count) + 1; i++)
                {
                    var node = roboNodes2.Get(i);
                    if (!node.IsCalc)
                    {
                        listRoboNodes2.Add(node);
                        listRoboNodeNumbers2.Add(node.Number);
                    }
                }

                stopwatch.Stop();
                Console.WriteLine("Robo no casting -> " + stopwatch.ElapsedMilliseconds);

                stopwatch.Reset();
            }

            for (int _ = 0; _ < 10; _++)
            {

                stopwatch.Start();
                proj.Save();
                stopwatch.Stop();
                Console.WriteLine("Save Time = " + stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();



                stopwatch.Start();
                proj.SaveAs(Path.Combine(dir, "Rombic_beams_20220808_01.rtd"));
                stopwatch.Stop();
                Console.WriteLine("SaveAs Time = " + stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();
            }


            proj.Close();
        }
    }
}
