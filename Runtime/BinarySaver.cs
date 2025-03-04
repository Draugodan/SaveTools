using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace SaveTools
{
    public class BinarySaver : ISaveFormatter
    {
        protected ISaver saver;
        protected BinaryWriter writer;
        protected BinaryReader reader;
        protected MemoryStream saveStream;

        public BinarySaver(ISaver saver)
        {
            this.saver = saver;
        }

        public void Save(string path)
        {
            saveStream = new MemoryStream();
            writer = new BinaryWriter(saveStream);

            saver.Save(this);

            writer.Flush();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (FileStream fileStream = File.Open(path, FileMode.Create))
                saveStream.WriteTo(fileStream);

            ((IDisposable)writer).Dispose();
            writer = null;
        }

        public bool Load(string path)
        {
            if (File.Exists(path))
            {
                saveStream = new MemoryStream(File.ReadAllBytes(path));
                reader = new BinaryReader(saveStream);

                saver.Load(this);

                ((IDisposable)reader).Dispose();
                reader = null;

                return true;
            }

            else
            {
                Debug.Log("Could not open file " + path);
                return false;
            }
        }

        public void Save(ISaver saver)
        {
            saver.Save(this);
        }

        public void Load(ISaver saver)
        {
            saver.Load(this);
        }

        public void Save<T>(List<T> savers) where T : ISaver, new()
        {
            Write(savers.Count);
            for (int i = 0; i < savers.Count; i++)
                Save(savers[i]);
        }

        public void Load<T>(List<T> savers) where T : ISaver, new()
        {
            int count = ReadInt();
            for (int i = 0; i < count; i++)
            {
                T result = new();
                Load(result);
                savers.Add(result);
            }
        }

        public List<T> LoadList<T>() where T : ISaver, new()
        {
            List<T> result = new();
            int count = ReadInt();
            for (int i = 0; i < count; i++)
            {
                T saver = new();
                Load(result);
                result.Add(saver);
            }
            return result;
        }

        public void Save<T>(T[] savers) where T : ISaver, new()
        {
            Write(savers.Length);
            for (int i = 0; i < savers.Length; i++)
                Save(savers[i]);
        }

        public T[] LoadArray<T>() where T : ISaver, new()
        {
            T[] result = new T[ReadInt()];
            for (int i = 0; i < result.Length; i++)
            {
                T saver = new();
                Load(saver);
                result[i] = saver;
            }
            return result;
        }

        #region IO operations
        #region Writing
        public void Write(int dat)
        {
            writer.Write(dat);
        }

        public void Write(uint dat)
        {
            writer.Write(dat);
        }

        public void Write(long dat)
        {
            writer.Write(dat);
        }

        public void Write(byte dat)
        {
            writer.Write(dat);
        }

        public void Write(bool dat)
        {
            writer.Write(dat);
        }

        public void Write(float dat)
        {
            writer.Write(dat);
        }

        public void Write(string dat)
        {
            writer.Write(dat.Length);
            for (int i = 0; i < dat.Length; i++)
                writer.Write(dat[i]);
        }

        public void Write(Vector3 dat)
        {
            writer.Write(dat.x);
            writer.Write(dat.y);
            writer.Write(dat.z);
        }

        public void Write(Quaternion dat)
        {
            writer.Write(dat.x);
            writer.Write(dat.y);
            writer.Write(dat.z);
            writer.Write(dat.w);
        }

        public void Write(int[] dat)
        {
            writer.Write(dat.Length);
            for (int i = 0; i < dat.Length; i++)
                writer.Write(dat[i]);
        }
        #endregion

        #region Reading to reference
        public void Read(ref int dat)
        {
            dat = reader.ReadInt32();
        }

        public void Read(ref long dat)
        {
            dat = reader.ReadInt64();
        }

        public void Read(ref byte dat)
        {
            dat = reader.ReadByte();
        }

        public void Read(ref bool dat)
        {
            dat = reader.ReadBoolean();
        }

        public void Read(ref float dat)
        {
            dat = reader.ReadSingle();
        }

        public void Read(ref string dat)
        {
            int len = reader.ReadInt32();
            dat = new string(reader.ReadChars(len));
        }
        #endregion

        #region Reading to return
        public int ReadInt()
        {
            return reader.ReadInt32();
        }

        public uint ReadUInt()
        {
            return reader.ReadUInt32();
        }

        public long ReadLong()
        {
            return reader.ReadInt64();
        }

        public float ReadSingle()
        {
            return reader.ReadSingle();
        }

        public bool ReadBoolean()
        {
            return reader.ReadBoolean();
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public string ReadString()
        {
            int len = reader.ReadInt32();
            return new string(reader.ReadChars(len));
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public Quaternion ReadQuaternion()
        {
            return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public int[] ReadIntArray()
        {
            int[] result = new int[reader.ReadInt32()];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadInt32();

            return result;
        }
        #endregion
        #endregion
    }
}