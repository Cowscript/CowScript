using script.builder;
using script.help;
using script.variabel;
using System.Collections;
using System.Collections.Generic;

namespace script
{
    public class VariabelDatabase
    {
        private ArrayList types = new ArrayList();
        private Dictionary<string, VariabelDatabaseData> container = new Dictionary<string, VariabelDatabaseData>();
        private VariabelDatabase befor = null;

        public VariabelDatabase()
        {
            types.Add("string");
            types.Add("int");
            types.Add("bool");
            types.Add("array");
            types.Add("function");
            types.Add("class");
        }

        public VariabelDatabase(VariabelDatabase befor, ArrayList type)
        {
            this.befor = befor;
            types = type;
        }

        public bool isType(string name)
        {
            return types.Contains(name);
        }
        

        public CVar push(string name, CVar variabel)
        {
            controleOveride(name);
            if (container.ContainsKey(name)) container.Remove(name);
            container.Add(name, new VariabelDatabaseData()
            {
                IsFunction = false,
                isClass = false,
                Context = variabel
            });
            return variabel;
        }

        public CVar get(string name)
        {
            return get(name, false);
        }

        public CVar get(string name, bool befor)
        {
            if (!befor)
            {
                //wee control in this scope 
                if (container.ContainsKey(name))
                    return container[name].Context;
                else if(this.befor != null)
                {
                    return this.befor.get(name, true);
                }
                else
                {
                    throw new ScriptError("Unknown variabel: " + name, new Posision(0, 0));
                }
            }
            else
            {
                if (container.ContainsKey(name))
                {
                    if (container[name].IsFunction || container[name].isClass)
                        return container[name].Context;
                    else throw new ScriptError("Unknown variabel: " + name, new Posision(0, 0));
                }else if(this.befor != null)
                {
                    return this.befor.get(name, true);
                }
                else
                {
                    throw new ScriptError("3Unknown variabel: " + name, new Posision(0, 0));
                }
            }
        }

        public bool isExists(string name)
        {
            return isExists(name, false);
        }

        public bool isExists(string name, bool befor)
        {
            if (!befor)
            {
                if (container.ContainsKey(name))
                {
                    return true;
                }
                else if(this.befor != null)
                {
                    return this.befor.isExists(name, true);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if(container.ContainsKey(name) && (container[name].IsFunction || container[name].isClass))
                {
                    return true;
                }else if(this.befor != null)
                {
                    return this.befor.isExists(name, true);
                }
                else
                {
                    return false;
                }
            }
        }

        public void pushFunction(Function function)
        {
            controleOveride(function.Name);
            if (container.ContainsKey(function.Name)) container.Remove(function.Name);
            container.Add(function.Name, new VariabelDatabaseData()
            {
                Context = new FunctionVariabel(function),
                IsFunction = true,
                isClass = false
            });
        }

        public void pushClass(Class c)
        {
            types.Add(c.Name);//now is this class a types :)
            controleOveride(c.Name);
            if (container.ContainsKey(c.Name)) container.Remove(c.Name);

            container.Add(c.Name, new VariabelDatabaseData()
            {
                Context = new ClassVariabel(c),
                IsFunction = false,
                isClass = true
            });
        }

        public VariabelDatabase createShadow()
        {
            return new VariabelDatabase(this, types);
        }

        public void controleOveride(string name)
        {
            if (container.ContainsKey(name))
            {
                if (container[name].IsFunction)
                {
                    throw new ScriptError("Cant overide " + name + " becuse it is a function", new Posision(0, 0));
                }else if (container[name].isClass)
                {
                    throw new ScriptError("Cant overide " + name + " becuse it is a class", new Posision(0, 0));
                }
            }

            if (befor != null)
                befor.controleOveride(name);
        }
    }
}
