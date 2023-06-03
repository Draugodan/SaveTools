namespace SaveTools
{
    public interface ISaver
    {
        void Save(ISaveFormatter reader);
        void Load(ISaveFormatter writer);
    }
}