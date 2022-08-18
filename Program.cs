using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Main");
        static void Main(string[] args)
        {
            InitialData.Start();
            Logging.SetLoggingConfiguration();
            Logger.Info("Evolution Started");

            var originalGeneration = new Generation();
            originalGeneration.GenerateOriginalGeneration();

            var initialGeneration = new Generation();
            initialGeneration.GenerateInitialGeneration();


            for (int i = 0; i < EvolutionParameters.NumberOfGenerations; i++)
            {
                var generation = new Generation();
                generation.GenerateRegularGeneration();
            }
                
            RobotInteraction.Finish();

            Console.WriteLine("Finished");

            Console.ReadLine();

            NLog.LogManager.Shutdown();

            
            // TODO: Serializing data
            // TODO: Continuing calculations after
            // TODO: Graphics visualization
        }
    }
}
