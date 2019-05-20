using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace MatlabInterop
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var bioDoseService = new BioDoseService();

            // Initialize the ESAPI application object
            using (var app = Application.CreateApplication(null, null))
            {
                // Open a patient and get a plan
                var patient = app.OpenPatientById("123456789");
                var course = patient.Courses.First(c => c.Id == "LIVER");
                var plan = course.PlanSetups.First(p => p.Id == "LIVER_5Fx");

                // Configure to get doses in absolute units
                plan.DoseValuePresentation = DoseValuePresentation.Absolute;

                var dose = plan.Dose;
                for (int z = 0; z < dose.ZSize; z++)
                {
                    Console.WriteLine($"Exporting plane {z}");

                    var dosePlane = GetDosePlane(z, dose);
                    var bioDosePlane = bioDoseService.Calculate(dosePlane, 5, 2.5);
                    ExportToCsv(z, bioDosePlane);
                }
            }
        }

        public static double[,] GetDosePlane(int z, Dose dose)
        {
            var voxels = new int[dose.XSize, dose.YSize];
            dose.GetVoxels(z, voxels);

            var dosePlane = new double[dose.XSize, dose.YSize];
            for (int x = 0; x < dose.XSize; x++)
                for (int y = 0; y < dose.YSize; y++)
                    dosePlane[x, y] = dose.VoxelToDoseValue(voxels[x, y]).Dose;

            return dosePlane;
        }

        private static void ExportToCsv(int z, double[,] dose)
        {
            var output = new StringBuilder();
            for (int y = 0; y < dose.GetLength(1); y++)
            {
                // Create the output line (doses separated by commas)
                var doseValues = new List<double>();
                for (int x = 0; x < dose.GetLength(0); x++)
                    doseValues.Add(dose[x, y]);
                output.AppendLine(string.Join(",", doseValues));
            }

            // Output the dose plane to a CSV file
            File.WriteAllText($@"C:\Temp\Dose\{z}.csv", output.ToString());
        }
    }
}
