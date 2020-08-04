using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using sxlib;
using sxlib.Specialized;
using Microsoft.Win32;
using System.Windows.Forms;

namespace JellyDriver_SynapseX
{
    class Program
    {
        private static SxLibOffscreen SLib;
        public static void createSLib(string dir)
        {
            SLib = SxLib.InitializeOffscreen(dir);
            SLib.LoadEvent += (Ev, Param) =>
            {
                if (Ev == SxLibBase.SynLoadEvents.READY)
                {
                    Console.WriteLine("Loading complete! Now getting the script hub.");

                    //Register a handler for AttachEvent.
                    SLib.ScriptHubEvent += SLibOnScriptHubEvent;

                    //Attach Synapse X.
                    SLib.ScriptHub();

                    return;
                }
                Console.WriteLine("l|" + Ev);
            };
            SLib.AttachEvent += (Ev, Param) =>
            {
                Console.WriteLine("a|" + Ev);
            };
        }
        public static void load()
        {
            try
            {
                SLib.Load();
            }
            catch (Exception e)
            {
                Console.WriteLine("e|" + e.Message);
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = e.Message;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    createSLib(fbd.SelectedPath);
                    load();
                    File.WriteAllText("config.jdx", fbd.SelectedPath);

                }
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            string loc = "";
            if (File.Exists("config.jdx"))
            {
                loc = File.ReadAllText("config.jdx");
            }

            createSLib(loc);
            load();

            while (true)
            {
                try
                {
                    var k = Console.ReadLine();
                    if (k.StartsWith("i|"))
                    {
                        SLib.Attach();
                    }
                    if (k.StartsWith("s|"))
                    {
                        var txt = File.ReadAllText(k.Replace("s|", ""));
                        var s = txt.Split('|');
                        if (txt.StartsWith("SYNX_SHUB|"))
                        {
                            foreach (var Entry in sh)
                            {
                                if (Entry.Name.Replace("|","l") == s[1])
                                {
                                    Console.WriteLine($"esh|{Entry.Name.Replace("|", "l")}");
                                    Entry.Execute();
                                }
                            }
                        } else
                        {
                            Console.WriteLine($"e{k}");
                            SLib.Execute(txt);
                        }
                        
                    }
                } catch(Exception e)
                {
                    Console.WriteLine("e|" + e.Message);
                }
            }
        }
        private static List<SxLibBase.SynHubEntry> sh;
        private static void SLibOnScriptHubEvent(List<SxLibBase.SynHubEntry> Entries)
        {
            sh = Entries;
            foreach (var Entry in Entries)
            {
                Console.WriteLine($"sh|{Entry.Name.Replace("|","l")}|{Entry.Description.Replace("|", "l")}|{Entry.Picture.Replace("|", "l")}");
            }
        }
    }
}
