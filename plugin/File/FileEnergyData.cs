using script.token;

namespace script.plugin.File
{
    class FileEnergyData : EnegyData
    {
        private EnegyData data;
        public override RunningState State
        {
            get
            {
                return this.data.State;
            }
        }

        public FileEnergyData(EnegyData data)
        {
            this.data = data;
        }

        public override void setError(ScriptError er, VariabelDatabase db)
        {
            data.setError(er, db);
        }
    }
}
