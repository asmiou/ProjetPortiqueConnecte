using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Microsoft.Scripting.Hosting;

namespace WebService_RFIDReader.Services
{
    public class RunPythonScript
    {
        static ProcessStartInfo psi;

        static ScriptEngine engine;

        static ScriptRuntimeSetup setup;

        static ScriptRuntime runtime;

        public RunPythonScript()
        {
            psi = new ProcessStartInfo();
            psi.FileName = @"E:\ProgramFiles\Anaconda3\python.exe";
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
        }

        public Boolean buildDataSet(string rawDataPath, string token)
        {
            var script = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/buildDataSet.py");
            psi.Arguments = $"\"{script}\" \"{rawDataPath}\" \"{token}\"";

            var errors = "";
            var results = "";

            using (var process = Process.Start(psi))
            {
                errors = process.StandardError.ReadToEnd();
                results = process.StandardOutput.ReadToEnd();
            }

            Debug.WriteLine("Errors : ");
            Debug.WriteLine(errors);
            Debug.WriteLine("");
            Debug.WriteLine("Resultats");
            Debug.WriteLine(results);

            return errors.Length>0?false:true;
        }

        public Boolean predictData(string dataSetPath, string token)
        {
            var script = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/predict.py");
            psi.Arguments = $"\"{script}\" \"{dataSetPath}\" \"{token}\"";

            var errors = "";
            var results = "";

            using (var process = Process.Start(psi))
            {
                errors = process.StandardError.ReadToEnd();
                results = process.StandardOutput.ReadToEnd();
            }

            Debug.WriteLine("Errors : ");
            Debug.WriteLine(errors);
            Debug.WriteLine("");
            Debug.WriteLine("Resultats");
            Debug.WriteLine(results);

            return errors.Length > 0 ? false : true;
        }
    }
}