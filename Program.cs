using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Win32;
using System.Windows.Forms;
using System.Reflection;

namespace JellyDriver_SynapseX
{
    class Prog
    {
        private static sxlib.Specialized.SxLibOffscreen SLib;
        public static void createSLib(string dir)
        {
            SLib = sxlib.SxLib.InitializeOffscreen(dir);
            SLib.LoadEvent += (Ev, Param) =>
            {
                Console.WriteLine("l|" + Ev);
                if (Ev == sxlib.Specialized.SxLibBase.SynLoadEvents.READY)
                {

                    //Register a handler for AttachEvent.
                    SLib.ScriptHubEvent += SLibOnScriptHubEvent;

                    //Attach Synapse X.
                    SLib.ScriptHub();

                }
                
            };
            SLib.AttachEvent += (Ev, Param) =>
            {
                Console.WriteLine("a|" + Ev);
            };
        }
        

        [STAThread]
        public void Man(string[] args)
        {
            
            if (!File.Exists(Application.StartupPath + @"\auth\options.bin"))
            {
                MessageBox.Show("I am not in the Synapse folder. Oh no.");
                Application.Exit();
                return;
            }
            createSLib(Application.StartupPath);
            SLib.Load();

            string s = "";
            while (true)
            {
                try
                {
                    int ke = Console.Read();
                    if (ke == 13) { continue; }
                    if (ke == 10)
                    {
                        string k = "";
                        k = s;
                        s = "";
                        if (k.StartsWith("i|"))
                        {
                            SLib.Attach();
                        }
                        if (k.StartsWith("s|"))
                        {
                            var txt = File.ReadAllText(k.Replace("s|", ""));
                            var sp = txt.Split('|');
                            if (txt.StartsWith("SYNX_SHUB|"))
                            {
                                foreach (var Entry in sh)
                                {
                                    if (Entry.Name.Replace("|", "l") == sp[1])
                                    {
                                        Console.WriteLine($"esh|{Entry.Name.Replace("|", "l")}");
                                        Entry.Execute();
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"e{k}");
                                SLib.Execute(txt);
                            }
                            
                        }
                    }
                    else
                    {
                        s += (char)ke;
                    }
                } catch(Exception e)
                {
                    Console.WriteLine("e|" + e.Message);
                }
            }
        }


        private static List<sxlib.Specialized.SxLibBase.SynHubEntry> sh;
        private static void SLibOnScriptHubEvent(List<sxlib.Specialized.SxLibBase.SynHubEntry> Entries)
        {
            sh = Entries;
            foreach (var Entry in Entries)
            {
                Console.WriteLine($"sh|{Entry.Name.Replace("|","l")}|{Entry.Description.Replace("|", "l")}|{Entry.Picture.Replace("|", "l")}");
            }
        }
    }
    class Program
    {
        public static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
            Prog poggers = new Prog();
            poggers.Man(args);
        }
        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            //This handler is called only when the common language runtime tries to bind to the assembly and fails.

            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            string applicationDirectory = Path.GetDirectoryName(executingAssembly.Location);

            string[] fields = args.Name.Split(',');
            string assemblyName = fields[0];
            string assemblyCulture;
            if (fields.Length < 2)
                assemblyCulture = null;
            else
                assemblyCulture = fields[2].Substring(fields[2].IndexOf('=') + 1);


            string assemblyFileName = assemblyName + ".dll";
            string assemblyPath;

            if (assemblyName.EndsWith(".resources"))
            {
                // Specific resources are located in app subdirectories
                string resourceDirectory = Path.Combine(applicationDirectory, assemblyCulture);

                assemblyPath = Path.Combine(resourceDirectory, assemblyFileName);
            } else
            {
                assemblyPath = Path.Combine(applicationDirectory, "bin","sxlib", assemblyFileName);
            }



            if (File.Exists(assemblyPath))
            {
                //Load the assembly from the specified path.                    
                Assembly loadingAssembly = Assembly.LoadFrom(assemblyPath);

                //Return the loaded assembly.
                return loadingAssembly;
            } else
            {
                return null;
            }
        }
    }
}
