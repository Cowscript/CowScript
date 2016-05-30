namespace script.plugin
{
    public interface PluginInterface
    {
        string Name { get; }
        void open(VariabelDatabase database, EnegyData data, Posision pos);
    }
}
