using System;
using System.IO;
using UnityEngine;
using System.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace SaveTools
{
    public class JsonSaver : ISaveFormatter
    {
        protected ISaver saver;

        private ExpandoObject contents;
        private Stack<ExpandoObject> contentStack = new Stack<ExpandoObject>();
        private int variableCounter, childCounter;
        private Stack<int> variableCounterStack = new Stack<int>();
        private Stack<int> childCounterStack = new Stack<int>();

        const string NAME = "name";
        const string CHILDREN = "children";
        private string VarName(int index) => $"var{index}";

        public JsonSaver(ISaver saver)
        {
            this.saver = saver;
        }

        public void Save(string path)
        {
            string directoryPath = Path.GetDirectoryName(path);
            if (directoryPath != string.Empty)
                Directory.CreateDirectory(directoryPath);

            contentStack.Clear();
            variableCounterStack.Clear();
            variableCounter = 0;
            contents = CreateContainer("root");

            saver.Save(this);

            File.WriteAllText(path, JsonConvert.SerializeObject(contents, Formatting.Indented));
        }

        public bool Load(string path)
        {
            if (File.Exists(path))
            {
                contents = JsonConvert.DeserializeObject<ExpandoObject>(File.ReadAllText(path));
                contentStack.Clear();
                variableCounterStack.Clear();
                variableCounter = 0;
                childCounterStack.Clear();
                childCounter = 0;

                saver.Load(this);
                return true;
            }
            else
            {
                Debug.Log("Could not open file " + path);
                return false;
            }
        }

        /// <summary>
        /// Saves an ISaver as a new child
        /// </summary>
        /// <param name="saver"></param>
        public void Save(ISaver saver)
        {
            //Store the state of the current nesting level in the stack
            contentStack.Push(contents);
            variableCounterStack.Push(variableCounter);

            //Create a new nesting level
            var childContents = CreateContainer(saver.GetType().Name);
            ((List<ExpandoObject>)((IDictionary<string, object>)contents)[CHILDREN]).Add(childContents);
            contents = childContents;
            variableCounter = 0;

            //Save the saver into this nesting level
            saver.Save(this);

            //Return to the previous nesting level
            contents = contentStack.Pop();
            variableCounter = variableCounterStack.Pop();
        }

        private ExpandoObject CreateContainer(string name)
        {
            ExpandoObject result = new ExpandoObject();
            ((IDictionary<string, object>)result).Add(NAME, name);
            ((IDictionary<string, object>)result).Add(CHILDREN, new List<ExpandoObject>());
            return result;
        }

        /// <summary>
        /// Loads the contents of the next child into the ISaver
        /// </summary>
        /// <param name="saver"></param>
        public void Load(ISaver saver)
        {
            //Store the state of the current nesting level in the stack
            contentStack.Push(contents);
            variableCounterStack.Push(variableCounter);
            childCounterStack.Push(childCounter);

            //Increase the nesting level
            var tmp = ((IDictionary<string, object>)contents)[CHILDREN] as List<object>;
            contents = tmp[childCounter] as ExpandoObject;
            variableCounter = 0;
            childCounter = 0;
            
            //Load the contents of this nesting level into the ISaver
            saver.Load(this);

            //Return to the previous nesting level
            contents = contentStack.Pop();
            variableCounter = variableCounterStack.Pop();
            childCounter = childCounterStack.Pop() + 1;
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
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat;
        }

        public void Write(uint dat)
        {
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat;
        }

        public void Write(long dat)
        {
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat;
        }

        public void Write(byte dat)
        {
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat;
        }

        public void Write(bool dat)
        {
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat;
        }

        public void Write(float dat)
        {
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat;
        }

        public void Write(string dat)
        {
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat;
        }

        public void Write(Vector3 dat)
        {
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat.x;
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat.y;
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat.z;
        }

        public void Write(Quaternion dat)
        {
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat.x;
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat.y;
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat.z;
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat.w;
        }

        public void Write(int[] dat)
        {
            ((IDictionary<string, object>)contents)[VarName(variableCounter++)] = dat;
        }
        #endregion

        #region Reading to reference
        public void Read(ref int dat)
        {
            dat = Convert.ToInt32(ReadNext());
        }

        public void Read(ref long dat)
        {
            dat = Convert.ToInt64(ReadNext());
        }

        public void Read(ref byte dat)
        {
            dat = Convert.ToByte(ReadNext());
        }

        public void Read(ref bool dat)
        {
            dat = Convert.ToBoolean(ReadNext());
        }

        public void Read(ref float dat)
        {
            dat = Convert.ToSingle(ReadNext());
        }

        public void Read(ref string dat)
        {
            dat = (string)ReadNext();
        }
        #endregion

        #region Reading to return
        public int ReadInt()
        {
            return Convert.ToInt32(ReadNext());
        }

        public uint ReadUInt()
        {
            return Convert.ToUInt32(ReadNext());
        }

        public long ReadLong()
        {
            return Convert.ToInt64(ReadNext());
        }

        public float ReadSingle()
        {
            return Convert.ToSingle(ReadNext());
        }

        public bool ReadBoolean()
        {
            return Convert.ToBoolean(ReadNext());
        }

        public byte ReadByte()
        {
            return Convert.ToByte(ReadNext());
        }

        public string ReadString()
        {
            return (string)ReadNext();
        }

        public Vector3 ReadVector3()
        {
            float x = Convert.ToSingle(ReadNext());
            float y = Convert.ToSingle(ReadNext());
            float z = Convert.ToSingle(ReadNext());

            return new Vector3(x, y, z);
        }

        public Quaternion ReadQuaternion()
        {
            float x = Convert.ToSingle(ReadNext());
            float y = Convert.ToSingle(ReadNext());
            float z = Convert.ToSingle(ReadNext());
            float w = Convert.ToSingle(ReadNext());

            return new Quaternion(x, y, z, w);
        }

        public int[] ReadIntArray()
        {
            List<object> tmpList = (List<object>)ReadNext();
            int[] result = new int[tmpList.Count];
            for (int i = 0; i < tmpList.Count; i++)
                result[i] = Convert.ToInt32(tmpList[i]);
            return result;
        }

        private object ReadNext()
        {
            return ((IDictionary<string, object>)contents)[VarName(variableCounter++)];
        }
        #endregion
        #endregion
    }
}