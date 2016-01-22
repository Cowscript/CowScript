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

            ClassMethods rConstructor = new ClassMethods(regex, null);
            rConstructor.caller += RConstructor_caller;
            rConstructor.Aguments.push("string", "regex");
            rConstructor.createConstructor();

            //wee need a pointer there is called str :)
            regex.addVariabel("regex", false, false);//str is not static and is not public :)

            ClassMethods match = new ClassMethods(regex, "match");
            match.caller += Match_caller;
            match.Aguments.push("string", "str");
            match.create();

            ClassMethods exec = new ClassMethods(regex, "exec");
            exec.caller += Exec_caller;
            exec.Aguments.push("string", "str");
            exec.create();

            database.pushClass(regex);
        }

        private CVar Exec_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {

            System.Text.RegularExpressions.MatchCollection matchs = System.Text.RegularExpressions.Regex.Matches(stack[0].toString(pos, data, db), @obj.get("regex").toString(pos, data, db));

            ArrayVariabel array = new ArrayVariabel();

            foreach (System.Text.RegularExpressions.Match match in matchs)
            {
                ArrayVariabel subArray = new ArrayVariabel();

                if (!match.Success)
                    continue;

                for (int i = 0; i < match.Groups.Count; i++)
                {
                    System.Text.RegularExpressions.Group group = match.Groups[i];

                    if (group.Success)
                        subArray.put(new StringVariabel(group.Value), pos, data, db);
                }

                array.put(array.getNextID(), subArray, pos, data, db);
            }

            return array;
        }

        private CVar Match_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new BooleanVariabel(System.Text.RegularExpressions.Regex.IsMatch(stack[0].toString(pos, data, db), @obj.get("regex").toString(pos, data, db)));
        }

        private CVar RConstructor_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            obj.appendToPointer("regex", stack[0]);
            return new NullVariabel();
        }
    }
}
