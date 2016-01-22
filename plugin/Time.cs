using script.builder;
using script.stack;
using script.variabel;
using System;
using System.Globalization;

namespace script.plugin
{
    class Time : PluginInterface
    {
        public void open(VariabelDatabase database)
        {
            Class time = new Class("Time");

            ClassMethods constructor = new ClassMethods(time, null);

            constructor.Aguments.push("int", "year", new NullVariabel());
            constructor.Aguments.push("int", "month", new NullVariabel());
            constructor.Aguments.push("int", "day", new NullVariabel());
            constructor.Aguments.push("int", "hour", new NullVariabel());
            constructor.Aguments.push("int", "minut", new NullVariabel());
            constructor.Aguments.push("int", "second", new NullVariabel());
            constructor.Aguments.push("int", "millisecond", new NullVariabel());
            
            constructor.caller += Constructor_caller;
            constructor.createConstructor();

            ClassMethods year = new ClassMethods(time, "year");
            year.caller += Year_caller;
            year.create();

            ClassMethods month = new ClassMethods(time, "month");
            month.caller += Month_caller;
            month.create();

            ClassMethods week = new ClassMethods(time, "week");
            week.caller += Week_caller;
            week.create();

            ClassMethods dayOfWeek = new ClassMethods(time, "dayOfWeek");
            dayOfWeek.caller += DayOfWeek_caller;
            dayOfWeek.create();

            ClassMethods day = new ClassMethods(time, "day");
            day.caller += Day_caller;
            day.create();

            ClassMethods minute = new ClassMethods(time, "minute");
            minute.caller += Minute_caller;
            minute.create();

            ClassMethods times = new ClassMethods(time, "hour");
            times.caller += Times_caller;
            times.create();

            ClassMethods second = new ClassMethods(time, "second");
            second.caller += Second_caller;
            second.create();

            ClassMethods millisecond = new ClassMethods(time, "millisecond");
            millisecond.caller += Millisecond_caller;
            millisecond.create();

            database.pushClass(time);
        }

        private CVar Millisecond_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Millisecond);
        }

        private CVar Second_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Second);
        }

        private CVar Times_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Hour);
        }

        private CVar Minute_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Minute);
        }

        private CVar Day_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Day);
        }

        private CVar DayOfWeek_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel((double)((DateTime)obj.systemItems["time"]).DayOfWeek);
        }

        private CVar Week_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            DateTime dt = (DateTime)obj.systemItems["time"];

            DayOfWeek dow = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dt);

            if(dow >= DayOfWeek.Monday && dow <= DayOfWeek.Wednesday)
            {
                dt = dt.AddDays(3);
            }

            return new IntVariabel(CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday));
        }

        private CVar Month_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Month);
        }

        private CVar Year_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Year);
        }

        private CVar Constructor_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            DateTime dt = new DateTime();

            int append;

            if(stack[0] is IntVariabel)
            {
                append = Convert.ToInt32(stack[0].toInt(pos)) - dt.Year;
                if(append != 0)
                    dt = dt.AddYears(append);
            }

            if (stack[1] is IntVariabel)
            {
                append = Convert.ToInt32(stack[1].toInt(pos)) - dt.Month;
                if (append != 0)
                    dt = dt.AddMonths(append);
            }

            if (stack[2] is IntVariabel)
            {
                append = Convert.ToInt32(stack[2].toInt(pos)) - dt.Day;
                if (append != 0)
                    dt = dt.AddDays(append);
            }

            if (stack[3] is IntVariabel)
            {
                append = Convert.ToInt32(stack[3].toInt(pos)) - dt.Hour;
                if(append != 0)
                    dt = dt.AddHours(append);
            }

            if (stack[4] is IntVariabel)
            {
                append = Convert.ToInt32(stack[4].toInt(pos)) - dt.Minute;
                if(append != 0)
                    dt = dt.AddMinutes(append);
            }

            if (stack[5] is IntVariabel)
            {
                append = Convert.ToInt32(stack[5].toInt(pos)) - dt.Second;
                if(append != 0)
                    dt = dt.AddSeconds(append);
            }

            if (stack[6] is IntVariabel)
            {
                append = Convert.ToInt32(stack[6].toInt(pos)) - dt.Millisecond;
                if(append != 0)
                    dt = dt.AddMilliseconds(append);
            }

            obj.systemItems.Add("time", dt);
            return new NullVariabel();
        }
    }
}
