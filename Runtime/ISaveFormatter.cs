using UnityEngine;

namespace SaveTools
{
    public interface ISaveFormatter
    {
        void Save(string path);

        bool Load(string path);

        void Save(ISaver saver);

        void Load(ISaver saver);

        #region IO Operations
        #region Writing
        void Write(int dat);
        void Write(uint dat);
        void Write(long dat);
        void Write(byte dat);
        void Write(bool dat);
        void Write(float dat);
        void Write(string dat);
        void Write(Vector3 dat);
        void Write(Quaternion dat);
        void Write(int[] dat);
        #endregion

        #region Reading to reference
        void Read(ref int dat);
        void Read(ref long dat);
        void Read(ref byte dat);
        void Read(ref bool dat);
        void Read(ref float dat);
        void Read(ref string dat);
        #endregion

        #region Reading to return
        int ReadInt();
        uint ReadUInt();
        long ReadLong();
        float ReadSingle();
        bool ReadBoolean();
        byte ReadByte();
        string ReadString();
        Vector3 ReadVector3();
        Quaternion ReadQuaternion();
        int[] ReadIntArray();
        #endregion
        #endregion
    }
}