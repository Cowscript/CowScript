using script.builder;
using script.stack;
using script.Type;
using script.variabel;
using System;
using System.Globalization;

namespace script.plugin
{
    class Time : PluginInterface
    {
        public void open(VariabelDatabase database, EnegyData data, Posision pos)
        {
            Class time = new Class("Time");

            Method constructor = new Method("");
            
            constructor.GetAgumentStack().push("int", "year", new NullVariabel());
            constructor.GetAgumentStack().push("int", "month", new NullVariabel());
            constructor.GetAgumentStack().push("int", "day", new NullVariabel());
            constructor.GetAgumentStack().push("int", "hour", new NullVariabel());
            constructor.GetAgumentStack().push("int", "minut", new NullVariabel());
            constructor.GetAgumentStack().push("int", "second", new NullVariabel());
            constructor.GetAgumentStack().push("int", "millisecond", new NullVariabel());

            constructor.SetBody(Constructor_caller);
            time.SetConstructor(constructor, data, database, pos);

            Method year = new Method("year");
            year.SetBody(Year_caller);
            time.SetMethod(year, data, database, pos);

            Method month = new Method("month");
            month.SetBody(Month_caller);
            time.SetMethod(month, data, database, pos);

            Method week = new Method("week");
            week.SetBody(Week_caller);
            time.SetMethod(week, data, database, pos);

            Method dayOfWeek = new Method("dayOfWeek");
            dayOfWeek.SetBody(DayOfWeek_caller);
            time.SetMethod(dayOfWeek, data, database, pos);

            Method day = new Method("day");
            day.SetBody(Day_caller);
            time.SetMethod(day, data, database, pos);

            Method minute = new Method("minute");
            minute.SetBody(Minute_caller);
            time.SetMethod(minute, data, database, pos);

            Method times = new Method("hour");
            times.SetBody(Times_caller);
            time.SetMethod(times, data, database, pos);

            Method second = new Method("second");
            second.SetBody(Second_caller);
            time.SetMethod(second, data, database, pos);

            Method millisecond = new Method("millisecond");
            millisecond.SetBody(Millisecond_caller);
            time.SetMethod(millisecond, data, database, pos);

            database.pushClass(time, data);
        }

        private CVar Millisecond_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt(((DateTime)TypeHandler.ToObjectVariabel(obj).systemItems["time"]).Millisecond, data, db, pos);
        }

        private CVar Second_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt(((DateTime)TypeHandler.ToObjectVariabel(obj).systemItems["time"]).Second, data, db, pos);
        }

        private CVar Times_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt(((DateTime)TypeHandler.ToObjectVariabel(obj).systemItems["time"]).Hour, data, db, pos);
        }

        private CVar Minute_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt(((DateTime)TypeHandler.ToObjectVariabel(obj).systemItems["time"]).Minute, data, db, pos);
        }

        private CVar Day_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt(((DateTime)TypeHandler.ToObjectVariabel(obj).systemItems["time"]).Day, data, db, pos);
        }

        private CVar DayOfWeek_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt((double)((DateTime)TypeHandler.ToObjectVariabel(obj).systemItems["time"]).DayOfWeek, data, db, pos);
        }

        private CVar Week_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            DateTime dt = (DateTime)TypeHandler.ToObjectVariabel(obj).systemItems["time"];

            DayOfWeek dow = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dt);

            if(dow >= DayOfWeek.Monday && dow <= DayOfWeek.Wednesday)
            {
                dt = dt.AddDays(3);
            }

            return Types.toInt(CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday), data, db, pos);
        }

        private CVar Month_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt(((DateTime)TypeHandler.ToObjectVariabel(obj).systemItems["time"]).Month, data, db, pos);
        }

        private CVar Year_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt(((DateTime)TypeHandler.ToObjectVariabel(obj).systemItems["time"]).Year, data, db, pos);
        }

        private CVar Constructor_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            DateTime dt = new DateTime();

            int append;

            if(!(stack[0] is NullVariabel))
            {
                append = Convert.ToInt32(stack[0].toInt(pos, data, db)) - dt.Year;
                if(append != 0)
                    dt = dt.AddYears(append);
            }

            if (!(stack[1] is NullVariabel))
            {
                append = Convert.ToInt32(stack[1].toInt(pos, data, db)) - dt.Month;
                if (append != 0)
                    dt = dt.AddMonths(append);
            }

            if (!(stack[2] is NullVariabel))
            {
                append = Convert.ToInt32(stack[2].toInt(pos, data, db)) - dt.Day;
                if (append != 0)
                    dt = dt.AddDays(append);
            }

            if (!(stack[3] is NullVariabel))
            {
                append = Convert.ToInt32(stack[3].toInt(pos, data, db)) - dt.Hour;
                if(append != 0)
                    dt = dt.AddHours(append);
            }

            if (!(stack[4] is NullVariabel))
            {
                append = Convert.ToInt32(stack[4].toInt(pos, data, db)) - dt.Minute;
                if(append != 0)
                    dt = dt.AddMinutes(append);
            }

            if (!(stack[5] is NullVariabel))
            {
                append = Convert.ToInt32(stack[5].toInt(pos, data, db)) - dt.Second;
                if(append != 0)
                    dt = dt.AddSeconds(append);
            }

            if (!(stack[6] is NullVariabel))
            {
                append = Convert.ToInt32(stack[6].toInt(pos, data, db)) - dt.Millisecond;
                if(append != 0)
                    dt = dt.AddMilliseconds(append);
            }

            TypeHandler.ToObjectVariabel(obj).systemItems.Add("time", dt);
            return new NullVariabel();
        }
    }
}
