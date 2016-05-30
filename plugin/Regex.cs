using script.builder;
using System;
using script.Type;
using script.variabel;

namespace script.plugin
{
    class Regex : PluginInterface
    {

        public string Name { get { return "regex"; } }

        public void open(VariabelDatabase database, EnegyData data, Posision pos)
        {
            Class regex = new Class("Regex");

            Method rConstructor = new Method("");
            rConstructor.GetAgumentStack().push("string", "regex");
            rConstructor.SetBody(RConstructor_caller);
            regex.SetConstructor(rConstructor, data, database, pos);

            Pointer rg = new Pointer("regex");
            rg.SetLevel(ClassItemAccessLevel.Private);
            regex.SetPointer(rg, data, database, pos);

            Method match = new Method("match");
            match.GetAgumentStack().push("string", "str");
            match.SetBody(Match_caller);
            regex.SetMethod(match, data, database, pos);

            Method exec = new Method("exec");
            exec.GetAgumentStack().push("string", "str");
            exec.SetBody(Exec_caller);
            regex.SetMethod(exec, data, database, pos);

            Method getRegex = new Method("getRegex");
            getRegex.SetBody(GetRegex_caller);
            regex.SetMethod(getRegex, data, database, pos);

            Method updateRegex = new Method("updateRegex");
            updateRegex.GetAgumentStack().push("string", "regex");
            updateRegex.SetBody(UpdateRegex_caller);
            regex.SetMethod(updateRegex, data, database, pos);

            database.pushClass(regex, data);
        }

        private CVar UpdateRegex_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if(stack[0].toString(pos, data, db) == "")
            {
                data.setError(new ScriptError("Regex->updateRegex(string) a regex string can´t be empty", pos), db);
                return null;
            }

            TypeHandler.ToObjectVariabel(obj).appendToPointer("regex", stack[0]);
            return null;
        }

        private CVar GetRegex_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return TypeHandler.ToObjectVariabel(obj).get("regex");
        }

        private CVar Exec_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {

            System.Text.RegularExpressions.MatchCollection matchs = System.Text.RegularExpressions.Regex.Matches(stack[0].toString(pos, data, db), @TypeHandler.ToObjectVariabel(obj).get("regex").toString(pos, data, db));

            ArrayVariabel array = new ArrayVariabel(data, db, pos);

            foreach (System.Text.RegularExpressions.Match match in matchs)
            {
                ArrayVariabel subArray = new ArrayVariabel(data, db, pos);

                if (!match.Success)
                    continue;

                for (int i = 0; i < match.Groups.Count; i++)
                {
                    System.Text.RegularExpressions.Group group = match.Groups[i];

                    if (group.Success)
                        subArray.put(Types.toString(group.Value, data, db, pos), pos, data, db);
                }

                array.put(array.getNextID(data, db, pos), subArray, pos, data, db);
            }

            return array;
        }

        private CVar Match_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new BooleanVariabel(System.Text.RegularExpressions.Regex.IsMatch(stack[0].toString(pos, data, db), @TypeHandler.ToObjectVariabel(obj).get("regex").toString(pos, data, db)));
        }

        private CVar RConstructor_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if(!TypeHandler.ToObjectVariabel(obj).appendToPointer("regex", stack[0]))
            {
                data.setError(new ScriptError("Could not append " + stack[0].toString(pos, data, db) + " to regex object 'Regex'", pos), db);
                return new NullVariabel();
            }
            return new NullVariabel();
        }
    }
}
