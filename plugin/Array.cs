using System;
using script.builder;
using script.help;
using script.stack;
using script.variabel;

namespace script.plugin
{
    class Array : PluginInterface
    {
        public void open(VariabelDatabase database, EnegyData data)
        {
            //no support from V0.2
            Function count = new Function();
            count.Name = "count";
            count.agument.push("array", "array");
            count.call += Count_call;
            database.pushFunction(count, data);

            Function hasValue = new Function();
            hasValue.Name = "hasValue";
            hasValue.agument.push("array", "array");
            hasValue.agument.push("context");
            hasValue.call += HasValue_call;
            database.pushFunction(hasValue, data);

            Function hasKey = new Function();
            hasKey.Name = "hasKey";
            hasKey.agument.push("array", "array");
            hasKey.agument.push("context");
            hasKey.call += HasKey_call;
            database.pushFunction(hasKey, data);
        }

        private CVar Count_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return new IntVariabel(((ArrayVariabel)stack[0]).length());
        }

        private CVar HasValue_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            ArrayVariabel array = (ArrayVariabel)stack[0];

            foreach (string key in array.Keys())
            {
                if (array.get(key, pos, data, db).compare(stack[1], pos, data, db))
                    return new BooleanVariabel(true);
            }

            return new BooleanVariabel(false);
        }

        private CVar HasKey_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            ArrayVariabel array = (ArrayVariabel)stack[0];
            return new BooleanVariabel(array.keyExists(stack[1], pos, data, db));
        }
    }
}
