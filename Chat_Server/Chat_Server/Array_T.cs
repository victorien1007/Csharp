using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Server
{
    class Array_Type<Type>//Genericity Array_T
    {
        private List<string> K;
        private List<Type> T;
        private int size;
        public Array_Type()
        {
            K = new List<string>();
            T = new List<Type>();
            size = 0;
        }
        public void Add(string k,Type t)
        {
            K.Add(k);
            T.Add(t);
            size++;
        }
        public void Remove(string k)
        {
            int i = K.IndexOf(k);
            K.RemoveAt(i);
            T.RemoveAt(i);
            size--;
        }
        public int Size()
        {
            return size;
        }
        public Type this[string k]
        {
            get
            {
                return T[K.IndexOf(k)];
            }
            set
            {
                T[K.IndexOf(k)]=value;
            }
        }
        public Type this[int k]
        {
            get
            {
                return T[k];
            }
        }
    }
    class Array_String
    {
        private List<string> K;
        private List<string> T;
        private int size;
        public Array_String()
        {
            K = new List<string>();
            T = new List<string>();
            size = 0;
        }
        public void Add(string k, string t)
        {
            K.Add(k);
            T.Add(t);
            size++;
        }
        public void Remove(string k)
        {
            int i = K.IndexOf(k);
            K.RemoveAt(i);
            T.RemoveAt(i);
            size--;
        }
        public int Size()
        {
            return size;
        }
        public string toK(string t)//find the IP according to the nickname
        {
            return K[T.IndexOf(t)];
        }
             
        public string this[string k]//find the nickname according to the IP
        {
            get
            {
                return T[K.IndexOf(k)];
            }
            set
            {
                T[K.IndexOf(k)] = value;
            }
        }
        public string this[int k]
        {
            get
            {
                return T[k];
            }
        }
    }
}
