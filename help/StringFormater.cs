using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

namespace script.help
{
    class StringFormater
    {
        private StringBuilder builder = new StringBuilder();
        private StringReader reader;
        private ArrayList param = new ArrayList();

        public void appendParam(object context)
        {
            param.Add(context);
        }

        public string format(string text)
        {
            reader = new StringReader(text);

            int cache;

            while((cache = reader.Peek()) != -1)
            {
                if(cache == '%')
                {
                    reader.Read();
                    string c;
                    if((c = controlIdentify()) != null)
                    {
                        builder.Append(c);
                    }else
                    {
                        break;
                    }
                }
                else
                {
                    builder.Append((char)reader.Read());
                }
            }

            return builder.ToString();
        }

        private string controlIdentify()
        {
            switch (reader.Peek())
            {
                case '%':
                    reader.Read();
                    return "%";
                case 'b':
                    return binary();
                case 'c':
                    return ansii();
                case 'd':
                    return number();
                case 'e':
                    return e();
                case 'u':
                    return u();
                case 'f':
                    return f();
                case 'o':
                    return o();
                case 's':
                    return s();
                case 'x':
                    return x();
                case 'X':
                    return X();

            }

            return null;
        }

        private string X()
        {
            reader.Read();
            return getNextInts().ToString("X");
        }

        private string x()
        {
            reader.Read();
            return getNextInts().ToString("x");
        }

        private string s()
        {
            reader.Read();
            return getNextString();
        }

        private string o()
        {
            reader.Read();
            return Convert.ToString(getNextInts(), 8);
        }

        private string f()
        {
            reader.Read();
            return ((float)getNextInts()).ToString();
        }

        private string u()
        {
            reader.Read();
            return ((decimal)getNextInts()).ToString();
        }

        private string e()
        {
            reader.Read();
            return getNextInts().ToString("e", CultureInfo.InvariantCulture);
        }

        private string number()
        {
            reader.Read();
            return getNextInts().ToString();
        }

        private string ansii()
        {
            reader.Read();
            return ((char)getNextInts()).ToString();
        }

        private string binary()
        {
            int cache = getNextInts();
            
            string result = string.Empty;

            while(cache > 0)
            {
                result = (cache % 2).ToString() + result;
                cache /= 2;
            }
            reader.Read();
            return result;
        }

        private int getNextInts()
        {
            for(int i = 0; i < param.Count; i++)
            {
                if(param[i] is int)
                {
                    int cache = (int)param[i];
                    param.Remove(cache);
                    return cache;
                }
            }

            return 0;
        }

        private string getNextString()
        {
            for (int i = 0; i < param.Count; i++)
            {
                if (param[i] is string)
                {
                    string cache = (string)param[i];
                    param.Remove(cache);
                    return cache;
                }
            }

            return null;
        }
    } 
}
