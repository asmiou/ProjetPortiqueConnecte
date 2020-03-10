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
    public class RunPythonScript_Old
    {
        static ScriptEngine engine;

        static ScriptRuntimeSetup setup;

        static ScriptRuntime runtime;
        public RunPythonScript_Old()
        {
            
            setup = Python.CreateRuntimeSetup(null);
            runtime = new ScriptRuntime(setup);
            //engine = Python.CreateEngine(runtime);
            //engine = Python.CreateEngine();
            engine = runtime.GetEngine("python");
        }

        public void hello()
        {
            try
            {
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/hello.py");
                ScriptSource source = engine.CreateScriptSourceFromFile(path);
                //ScriptScope scope = engine.Runtime.CreateScope();
                //scope.SetVariable("rawData", rawDataPath);

                source.Execute();
                //return scope.GetVariable("prefixPath");
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur: " + e.Message);
                throw e;
            }

            //return "cheminVersLeDataSetCrée";
        }

        public Boolean buildDataSet(string rawDataPath, string token)
        {
            try
            {
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/buildDataSet.py");
                ScriptSource source = engine.CreateScriptSourceFromFile(path);
                ScriptScope scope = engine.Runtime.CreateScope();
                scope.SetVariable("rawData", rawDataPath);

                scope.SetVariable("token", token);

                source.Execute(scope);
                return scope.GetVariable("executed");
            }
            catch(Exception e)
            {
                Console.WriteLine("Erreur: " + e.Message);
                throw e;
            }

        
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