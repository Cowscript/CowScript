using script.builder;
using script.stack;
using script.Type;
using script.variabel;

namespace script.plugin
{
    class Regex : PluginInterface
    {
        public void open(VariabelDatabase database, EnegyData data, Posision pos)
        {
            Class regex = new Class("Regex");

            Method rConstructor = new Method("");
            rConstructor.GetAgumentStack().push("string", "regex");
            rConstructor.SetBody(RConstructor_caller);
            regex.SetMethod(rConstructor, data, database, pos);

            Pointer rg = new Pointer("regex");
            rg.SetPrivate();
            regex.SetPointer(rg, data, database, pos);

            Method match = new Method("match");
            match.GetAgumentStack().push("string", "str");
            match.SetBody(Match_caller);
            regex.SetMethod(match, data, database, pos);

            Method exec = new Method("exec");
            exec.GetAgumentStack().push("string", "str");
            exec.SetBody(Exec_caller);
            regex.SetMethod(exec, data, database, pos);

            database.pushClass(regex, data);
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
            TypeHandler.ToObjectVariabel(obj).appendToPointer("regex", stack[0]);
            return new NullVariabel();
        }
    }
}
