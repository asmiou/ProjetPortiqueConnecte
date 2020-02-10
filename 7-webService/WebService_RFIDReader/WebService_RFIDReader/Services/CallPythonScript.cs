using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace WebService_RFIDReader.Services
{
    public class CallPythonScript
    {
        static ScriptEngine engine;

        static ScriptRuntimeSetup setup;

        static ScriptRuntime runtime;
        public CallPythonScript()
        {
            
            setup = Python.CreateRuntimeSetup(null);
            runtime = new ScriptRuntime(setup);
            //engine = Python.CreateEngine(runtime);
            //engine = Python.CreateEngine();
            engine = runtime.GetEngine("python");
        }

        public void buildDataSet(string rawDataPath)
        {
            try
            {
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/buildData.py");
                ScriptSource source = engine.CreateScriptSourceFromFile(path);
                ScriptScope scope = engine.Runtime.CreateScope();
                scope.SetVariable("rawData", rawDataPath);

                source.Execute(scope);
                //return scope.GetVariable("prefixPath");
            }
            catch(Exception e)
            {
                Console.WriteLine("Erreur: " + e.Message);
                throw e;
            }

            //return "cheminVersLeDataSetCrée";
        }

        public string predictByKnn(string trainSetPath, string testSetPath, string dataSetPath)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/knn.py");
            ScriptSource source = engine.CreateScriptSourceFromFile(path);
            ScriptScope scope = engine.Runtime.CreateScope();
            scope.SetVariable("trainSet", trainSetPath);
            scope.SetVariable("testSet", testSetPath);
            scope.SetVariable("dataSet", dataSetPath);

            //engine.ExecuteFile(path);
            source.Execute(scope);
            //scope.Engine.Runtime.Shutdown();

            return scope.GetVariable("predictPath");
           
            
            //return "chaminVersLesDonnéesPredites";
        }

        public string predictByKmeans()
        {
            return "chaminVersLesDonnéesPredites";
        }


    }
}