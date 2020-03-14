using System;
using System.Diagnostics;
using Microsoft.Scripting.Hosting;

namespace WebService_RFIDReader.Services
{
    public class RunPythonScript
    {
        public ProcessStartInfo psi;

        public RunPythonScript(){}

        public Boolean exectScript(string assetFolder, string token)
        {
            psi = new ProcessStartInfo();
            psi.FileName = @"E:\ProgramFiles\Anaconda3\python.exe";
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            var script = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/script.py");
            psi.Arguments = $"\"{script}\" \"{assetFolder}\" \"{token}\"";

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

            return errors.Length > 0 ? throw new Exception("Une erreur s'est produite SCRIPT PYTHON \n"+errors) : true;
        }
    }
}