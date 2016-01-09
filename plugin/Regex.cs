using script.builder;
using script.stack;
using script.variabel;

namespace script.plugin
{
    class Regex : PluginInterface
    {
        public void open(VariabelDatabase database)
        {
            Class regex = new Class("Regex");

            ClassMethods rConstructor = (ClassMethods)regex.createMethods();
            rConstructor.caller += RConstructor_caller;
            AgumentStack rConstructorStack = new AgumentStack();
            rConstructorStack.push("string", "regex");
            rConstructor.Aguments = rConstructorStack;
            rConstructor.createConstructor();

            //wee need a pointer there is called str :)
            regex.addVariabel("regex", false, false);//str is not static and is not public :)

            ClassMethods match = (ClassMethods)regex.createMethods();
            match.setName("match");
            match.setAccess(true);
            match.caller += Match_caller;
            AgumentStack matchStack = new AgumentStack();
            matchStack.push("string", "str");
            match.Aguments = matchStack;
            match.create();

            ClassMethods exec = (ClassMethods)regex.createMethods();
            exec.setName("exec");
            exec.setAccess(true);
            exec.caller += Exec_caller;
            AgumentStack execStack = new AgumentStack();
            execStack.push("string", "str");
            exec.Aguments = execStack;
            exec.create();

            database.pushClass(regex);
        }

        private CVar Exec_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {

            System.Text.RegularExpressions.MatchCollection matchs = System.Text.RegularExpressions.Regex.Matches(stack.pop().toString(new Posision(0, 0)), @obj.get("regex").toString(new Posision(0, 0)));

            ArrayVariabel array = new ArrayVariabel();

            foreach(System.Text.RegularExpressions.Match match in matchs)
            {
                ArrayVariabel subArray = new ArrayVariabel();

                if (!match.Success)
                    continue;

                for(int i = 0; i < match.Groups.Count; i++)
                {
                    System.Text.RegularExpressions.Group group = match.Groups[i];

                    if (group.Success)
                        subArray.put(new IntVariabel(subArray.getNextID()), new StringVariabel(group.Value), new Posision(0, 0));
                }

                array.put(new IntVariabel(array.getNextID()), subArray, new Posision(0, 0));
            }

            return array;
        }

        private CVar Match_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            return new BooleanVariabel(System.Text.RegularExpressions.Regex.IsMatch(stack.pop().toString(new Posision(0, 0)), @obj.get("regex").toString(new Posision(0, 0))));
        }

        private CVar RConstructor_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            obj.appendToPointer("regex", stack.pop());
            return new NullVariabel();
        }
    }
}
