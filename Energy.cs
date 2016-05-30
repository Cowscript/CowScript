using script.builder;
using script.help;
using script.plugin;
using script.Task;
using script.token;
using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace script
{
    public class Energy : IDisposable
    {
        public const string VERSION = "V0.6";
        private EnegyData Data {  set;  get; }

        public VariabelDatabase VariabelDatabase { set; get; }
        public PluginContainer plugin = new PluginContainer();

        private ArrayList taskEvent = new ArrayList();
        private ArrayList taskEnd = new ArrayList();
        private TextReader reader;

        public Energy()
        {
            Data = new EnegyData()
            {
                Config = new ScriptConfig()
            };

            //set the impontans config so the script not can change it!
            Data.Config.append("tcp.enable",         "false", false);
            Data.Config.append("file.system.enable", "false", false);

            VariabelDatabase = new VariabelDatabase();
            StartItems.CreateStartItems(Data, VariabelDatabase);
        }

        public void setConfig(string name, string value)
        {
            setConfig(name, value, true);
        }

        public void setConfig(string name, string value, bool overide)
        {
            Data.Config.append(name, value, overide);
        }

        public void parse(string script)
        {
            parse((TextReader)new StringReader(script));
        }

        public void parse(StringReader reader)
        {
            parse((TextReader)reader);
        }

        public void parse(TextReader reader)
        {
            Interprenter.parse(new Token((this.reader = reader), Data, VariabelDatabase), Data, VariabelDatabase);
        }

        public void push(Function func)
        {
            VariabelDatabase.pushFunction(func, Data);
        }

        public void push(Class c)
        {
            VariabelDatabase.pushClass(c, Data);
        }

        public void addTaskEvent(ScriptTaskEvent e)
        {
            taskEvent.Add(e);
        }

        public void addEndEvent(ScriptEndEvent e)
        {
            taskEnd.Add(e);
        }

        public void Dispose()
        {
            if (reader != null)
                reader.Close();

            int i = 0;
            while (taskEvent.Count != 0)
            {
                if (i > taskEvent.Count)
                    i = 0;

                if (((ScriptTaskEvent)taskEvent[i]).isReady())
                {
                    //remove the event becuse it is ready to be removed :)
                    taskEvent.Remove(taskEvent[i]);
                }

                i++;
            }

            foreach (ScriptEndEvent e in taskEnd)
                e.callInEnd();
        }

        public RunningState getRunningStatus()
        {
            return Data.State;
        }

        public ScriptError getError()
        {
            if(Data.State != RunningState.Error)
            {
                return null;
            }

            return Data.ErrorData;
        }

        public void appendMethodToClass(Method method, Class c)
        {
            c.SetMethod(method, Data, VariabelDatabase, new Posision(0, 0));
        }

        public void LoadDLLFiles(string dir)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }

            foreach(string file in Directory.GetFiles(dir, "*.dll"))
            {
                HandleDLL(Assembly.LoadFile(file));
            }
        }

        public void Reaset()
        {
            VariabelDatabase = new VariabelDatabase();
            Data.setNormal();
        }

        private void HandleDLL(Assembly assembly)
        {
            foreach(System.Type type in assembly.GetTypes())
            {
                if(type.IsClass && !type.IsAbstract && type.IsPublic)
                {
                    foreach(System.Type type2 in type.FindInterfaces(new TypeFilter(ReturnTrue), null))
                    {
                        //first wee look if it is PluginInterface :)
                        if(type2 == typeof(PluginInterface))
                        {
                            Data.Plugin.push(Activator.CreateInstance(type) as PluginInterface);
                        }
                    }
                }
            }
        }

        private bool ReturnTrue(System.Type m, object filterCriteria)
        {
            return true;
        }
    }
}
