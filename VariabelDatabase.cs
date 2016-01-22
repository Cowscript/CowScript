using script.builder;
using script.help;
using script.variabel;
using System.Collections;
using System.Collections.Generic;

namespace script
{
    public class VariabelDatabase
    {
        //velcommen to the new VariabelDatabase :)

        private ArrayList types = new ArrayList();
        private Dictionary<string, CVar> container = new Dictionary<string, CVar>();
        private Dictionary<string, CVar> global = new Dictionary<string, CVar>();

        public ObjectVariabel Object { private set; get; }
        public ClassVariabel C { private set; get; }

        public VariabelDatabase()
        {
            types.Add("string");
            types.Add("int");
            types.Add("bool");
            types.Add("array");
            types.Add("function");
            types.Add("class");
        }

        public VariabelDatabase(Dictionary<string, CVar> global, ArrayList types)
        {
            this.global = new Dictionary<string, CVar>(global);
            this.types = new ArrayList(types);
        }

        public VariabelDatabase(Dictionary<string, CVar> global, ArrayList types, ObjectVariabel obj)
        {
            this.global = new Dictionary<string, CVar>(global);
            this.types = new ArrayList(types);
            Object = obj;
        }

        public VariabelDatabase(Dictionary<string, CVar> global, ArrayList types, ClassVariabel c)
        {
            this.global = new Dictionary<string, CVar>(global);
            this.types = new ArrayList(types);
            C = c;
        }

        public bool isType(string name)
        {
            return types.Contains(name);
        }
        

        public CVar push(string name, CVar variabel)
        {
            controleOveride(name);
            if (container.ContainsKey(name)) container.Remove(name);
            container.Add(name, variabel);
            return variabel;
        }

        public CVar get(string name)
        {
            if (global.ContainsKey(name))
                return global[name];

            if (container.ContainsKey(name))
                return container[name];

            throw new ScriptError("Unknown variabel: " + name, new Posision(0, 0));
        }

        public bool isExists(string name)
        {
            return global.ContainsKey(name) || container.ContainsKey(name);
        }

        public void pushFunction(Function function)
        {
            controleOveride(function.Name);
            global.Add(function.Name, new FunctionVariabel(function));
        }

        public void pushClass(Class c)
        {
            controleOveride(c.Name);
            types.Add(c.Name);//now is this class a types :)
            global.Add(c.Name, new ClassVariabel(c));
        }

        public VariabelDatabase createShadow()
        {
            return new VariabelDatabase(global, types);
        }

        public VariabelDatabase createShadow(ObjectVariabel obj)
        {
            return new VariabelDatabase(global, types, obj);
        }

        public VariabelDatabase createShadow(ClassVariabel obj)
        {
            return new VariabelDatabase(global, types, obj);
        }

        public void controleOveride(string name)
        {
            if (!global.ContainsKey(name))
                return;

            CVar var = global[name];
            throw new ScriptError("You can´t convert " + name + " becuse it is " + var.type(), new Posision(0, 0));
        }
    }
}
