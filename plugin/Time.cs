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

            ClassMethods constructor = new ClassMethods(time);
            AgumentStack constructorStack = new AgumentStack();

            constructorStack.push("int", "year", new NullVariabel());
            constructorStack.push("int", "month", new NullVariabel());
            constructorStack.push("int", "day", new NullVariabel());
            constructorStack.push("int", "hour", new NullVariabel());
            constructorStack.push("int", "minut", new NullVariabel());
            constructorStack.push("int", "second", new NullVariabel());
            constructorStack.push("int", "millisecond", new NullVariabel());

            constructor.Aguments = constructorStack;
            constructor.caller += Constructor_caller;
            constructor.createConstructor();

            ClassMethods year = new ClassMethods(time);
            year.setName("year");
            year.setAccess(true);
            year.caller += Year_caller;
            year.create();

            ClassMethods month = new ClassMethods(time);
            month.setName("month");
            month.setAccess(true);
            month.caller += Month_caller;
            month.create();

            ClassMethods week = new ClassMethods(time);
            week.setName("week");
            week.setAccess(true);
            week.caller += Week_caller;
            week.create();

            ClassMethods dayOfWeek = new ClassMethods(time);
            dayOfWeek.setName("dayOfWeek");
            dayOfWeek.setAccess(true);
            dayOfWeek.caller += DayOfWeek_caller;
            dayOfWeek.create();

            ClassMethods day = new ClassMethods(time);
            day.setName("day");
            day.setAccess(true);
            day.caller += Day_caller;
            day.create();

            ClassMethods minute = new ClassMethods(time);
            minute.setName("minute");
            minute.setAccess(true);
            minute.caller += Minute_caller;
            minute.create();

            ClassMethods times = new ClassMethods(time);
            times.setName("hour");
            times.setAccess(true);
            times.caller += Times_caller;
            times.create();

            ClassMethods second = new ClassMethods(time);
            second.setName("second");
            second.setAccess(true);
            second.caller += Second_caller;
            second.create();

            ClassMethods millisecond = new ClassMethods(time);
            millisecond.setName("millisecond");
            millisecond.setAccess(true);
            millisecond.caller += Millisecond_caller;
            millisecond.create();

            database.pushClass(time);
        }

        private CVar Millisecond_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Millisecond);
        }

        private CVar Second_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Second);
        }

        private CVar Times_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Hour);
        }

        private CVar Minute_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Minute);
        }

        private CVar Day_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Day);
        }

        private CVar DayOfWeek_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            return new IntVariabel((double)((DateTime)obj.systemItems["time"]).DayOfWeek);
        }

        private CVar Week_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            DateTime dt = (DateTime)obj.systemItems["time"];

            DayOfWeek dow = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dt);

            if(dow >= DayOfWeek.Monday && dow <= DayOfWeek.Wednesday)
            {
                dt = dt.AddDays(3);
            }

            return new IntVariabel(CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday));
        }

        private CVar Month_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Month);
        }

        private CVar Year_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            return new IntVariabel(((DateTime)obj.systemItems["time"]).Year);
        }

        private CVar Constructor_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            DateTime dt = new DateTime();

            CVar cache;
            int append;

            if((cache = stack.pop()) is IntVariabel)
            {
                append = Convert.ToInt32(cache.toInt(new Posision(0, 0))) - dt.Year;
                if(append != 0)
                    dt = dt.AddYears(append);
            }

            if ((cache = stack.pop()) is IntVariabel)
            {
                append = Convert.ToInt32(cache.toInt(new Posision(0, 0))) - dt.Month;
                if (append != 0)
                    dt = dt.AddMonths(append);
            }

            if ((cache = stack.pop()) is IntVariabel)
            {
                append = Convert.ToInt32(cache.toInt(new Posision(0, 0))) - dt.Day;
                if (append != 0)
                    dt = dt.AddDays(append);
            }

            if ((cache = stack.pop()) is IntVariabel)
            {
                append = Convert.ToInt32(cache.toInt(new Posision(0, 0))) - dt.Hour;
                if(append != 0)
                    dt = dt.AddHours(append);
            }

            if ((cache = stack.pop()) is IntVariabel)
            {
                append = Convert.ToInt32(cache.toInt(new Posision(0, 0))) - dt.Minute;
                if(append != 0)
                    dt = dt.AddMinutes(append);
            }

            if ((cache = stack.pop()) is IntVariabel)
            {
                append = Convert.ToInt32(cache.toInt(new Posision(0, 0))) - dt.Second;
                if(append != 0)
                    dt = dt.AddSeconds(append);
            }

            if ((cache = stack.pop()) is IntVariabel)
            {
                append = Convert.ToInt32(cache.toInt(new Posision(0, 0))) - dt.Millisecond;
                if(append != 0)
                    dt = dt.AddMilliseconds(append);
            }

            obj.systemItems.Add("time", dt);
            return new NullVariabel();
        }
    }
}
