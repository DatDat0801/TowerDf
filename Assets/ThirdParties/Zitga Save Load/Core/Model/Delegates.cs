namespace ZitgaSaveLoad
{
    public class Delegates
    {
        public delegate void OnSave(int logicCode);

        public delegate void OnLoad(int logicCode, Snapshot snapshot);
    }
}