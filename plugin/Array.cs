using System;
using script.builder;
using script.help;
using script.stack;
using script.variabel;

namespace script.plugin
{
    class Array : PluginInterface
    {
        public void open(VariabelDatabase database)
        {
            Function count = new Function();
            count.Name = "count";
            count.agument.push("array", "array");
            count.call = new ArrayCount();
            database.pushFunction(count);

            Function hasValue = new Function();
            hasValue.Name = "hasValue";
            hasValue.agument.push("array", "array");
            hasValue.agument.push("context");
            hasValue.call = new HasValue();
            database.pushFunction(hasValue);

            Function hasKey = new Function();
            hasKey.Name = "hasKey";
            hasKey.agument.push("array", "array");
            hasKey.agument.push("context");
            hasKey.call = new HasKey();
            database.pushFunction(hasKey);
        }
    }

    class HasKey : CallInterface
    {
        public CVar call(CallAgumentStack stack, EnegyData data)
        {
            ArrayVariabel array = (ArrayVariabel)stack.pop();
            return new BooleanVariabel(array.keyExists(stack.pop(), new Posision(0,0)));
        }
    }

    class HasValue : CallInterface
    {
        public CVar call(CallAgumentStack stack, EnegyData data)
        {
            ArrayVariabel array = (ArrayVariabel)stack.pop();
            CVar value = stack.pop();

            foreach(string key in array.Keys())
            {
                if (array.get(key, new Posision(0, 0)).compare(value, new Posision(0, 0)))
                    return new BooleanVariabel(true);
            }

            return new BooleanVariabel(false);
        }
    }

    class ArrayCount : CallInterface
    {
        public CVar call(CallAgumentStack stack, EnegyData data)
        {
            return new IntVariabel(((ArrayVariabel)stack.pop()).length());
        }
    }
}
