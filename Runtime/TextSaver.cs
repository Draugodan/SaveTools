using UnityEngine;
using System;
using System.IO;

namespace SaveTools
{
    public class TextSaver : ISaveFormatter
    {
        protected ISaver saver;
        protected StreamWriter writer;
        protected StreamReader reader;
        protected MemoryStream saveStream;

        private int linecount;

        public TextSaver(ISaver saver)
        {
            this.saver = saver;
        }

        public void Save(string path)
        {
            linecount = 0;

            saveStream = new MemoryStream();
            writer = new StreamWriter(saveStream);

            saver.Save(this);

            writer.Flush();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (FileStream fileStream = File.Open(path, FileMode.Create))
                saveStream.WriteTo(fileStream);

            ((IDisposable)writer).Dispose();
            writer = null;

            Debug.Log("Wrote " + linecount + " lines");
        }

        public bool Load(string path)
        {
            if (File.Exists(path))
            {
                linecount = 0;

                saveStream = new MemoryStream(File.ReadAllBytes(path));
                reader = new StreamReader(saveStream);

                saver.Load(this);

                ((IDisposable)reader).Dispose();
                reader = null;

                Debug.Log("Read " + linecount + " lines");
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

        #region IO operations
        #region Writing
        public void Write(int dat)
        {
            writer.WriteLine(dat.ToString());
            linecount++;
        }

        public void Write(long dat)
        {
            writer.WriteLine(dat.ToString());
            linecount++;
        }

        public void Write(byte dat)
        {
            writer.WriteLine(dat.ToString());
            linecount++;
        }

        public void Write(bool dat)
        {
            writer.WriteLine(dat.ToString());
            linecount++;
        }

        public void Write(float dat)
        {
            writer.WriteLine(dat.ToString());
            linecount++;
        }

        public void Write(string dat)
        {
            writer.WriteLine(dat.ToString());
            linecount++;
        }

        public void Write(Vector3 dat)
        {
            writer.WriteLine(dat.x.ToString());
            writer.WriteLine(dat.y.ToString());
            writer.WriteLine(dat.z.ToString());
            linecount += 3;
        }

        public void Write(Quaternion dat)
        {
            writer.WriteLine(dat.x.ToString());
            writer.WriteLine(dat.y.ToString());
            writer.WriteLine(dat.z.ToString());
            writer.WriteLine(dat.w.ToString());
            linecount += 4;
        }

        public void Write(int[] dat)
        {
            writer.WriteLine(dat.Length.ToString());
            linecount++;

            for (int i = 0; i < dat.Length; i++)
            {
                writer.WriteLine(dat[i].ToString());
                linecount++;
            }
        }
        #endregion

        #region Reading to reference
        public void Read(ref int dat)
        {
            dat = int.Parse(reader.ReadLine());
            linecount++;
        }

        public void Read(ref long dat)
        {
            dat = long.Parse(reader.ReadLine());
            linecount++;
        }

        public void Read(ref byte dat)
        {
            dat = byte.Parse(reader.ReadLine());
            linecount++;
        }

        public void Read(ref bool dat)
        {
            dat = bool.Parse(reader.ReadLine());
            linecount++;
        }

        public void Read(ref float dat)
        {
            dat = float.Parse(reader.ReadLine());
            linecount++;
        }

        public void Read(ref string dat)
        {
            dat = reader.ReadLine();
            linecount++;
        }
        #endregion

        #region Reading to return
        public int ReadInt()
        {
            linecount++;
            return int.Parse(reader.ReadLine());
        }

        public long ReadLong()
        {
            linecount++;
            return long.Parse(reader.ReadLine());
        }

        public float ReadSingle()
        {
            linecount++;
            return int.Parse(reader.ReadLine());
        }

        public bool ReadBoolean()
        {
            linecount++;
            return bool.Parse(reader.ReadLine());
        }

        public byte ReadByte()
        {
            linecount++;
            return byte.Parse(reader.ReadLine());
        }

        public string ReadString()
        {
            linecount++;
            return reader.ReadLine();
        }

        public Vector3 ReadVector3()
        {
            linecount += 3;
            return new Vector3(float.Parse(reader.ReadLine()), float.Parse(reader.ReadLine()), float.Parse(reader.ReadLine()));
        }

        public Quaternion ReadQuaternion()
        {
            linecount += 4;
            return new Quaternion(float.Parse(reader.ReadLine()), float.Parse(reader.ReadLine()), float.Parse(reader.ReadLine()), float.Parse(reader.ReadLine()));
        }

        public int[] ReadIntArray()
        {
            linecount++;
            int[] result = new int[int.Parse(reader.ReadLine())];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = int.Parse(reader.ReadLine());
                linecount++;
            }

            return result;
        }
        #endregion
        #endregion
    }
}