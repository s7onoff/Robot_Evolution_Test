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
        static void Main(string[] args)
        {
            InitialData.Start();
            Logging.SetLoggingConfiguration();
            Logging.Logger.Info("Evolution Started");

            var originalGeneration = new Generation();
            originalGeneration.generateOriginalGeneration();

            var initialGeneration = new Generation();
            initialGeneration.generateInitialGeneration();


            for (int i = 0; i < EvolutionParameters.NumberOfGenerations; i++)
            {
                var generation = new Generation();
                generation.generateRegularGeneration();
            }
                
            RobotInteraction.Finish();

            Console.ReadLine();

            // TODO: Logging
            // TODO: Serializing data
            // TODO: Continuing calculations after
            // TODO: Graphics visualization
        }
    }
}
