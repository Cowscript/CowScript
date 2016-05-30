using script.builder;
using script.Type;
using script.variabel;

namespace script.plugin
{
    class Array : PluginInterface
    {
        public string Name { get { return "array"; } }

        //no support from V0.3
        public void open(VariabelDatabase database, EnegyData data, Posision pos)
        {
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
