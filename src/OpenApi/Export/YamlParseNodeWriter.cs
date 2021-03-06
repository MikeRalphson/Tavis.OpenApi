﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavis.OpenApi.Export
{
    public class YamlParseNodeWriter : IParseNodeWriter
    {
        public enum State
        {
            InDocument,
            InList,
            InMap, 
            InProperty
        }

        Stack<State> state = new Stack<State>();
        StreamWriter writer;
        
        public struct DebugInfo
        {
            public Stack<State> StackState { get; set; }
            public string Indent { get; set; }
        }
        public DebugInfo GetDebugInfo()
        {
            return new DebugInfo
            {
                StackState = this.state,
                Indent = Indent
            };
        }
        public YamlParseNodeWriter(Stream stream)
        {
            this.writer = new StreamWriter(stream);
        }

        public void Flush()
        {
            this.writer.Flush();
        }

        string Indent = "";
        void IncreaseIndent()
        {
            Indent += "  ";
        }
        void DecreaseIndent()
        {
            Indent = Indent.Substring(0, Indent.Length - 2);
        }

        private bool InMap()
        {
            return state.Peek() == State.InMap;
        }
        private bool InList()
        {
            return state.Peek() == State.InList;
        }
        private bool InProperty()
        {
            return state.Peek() == State.InProperty;
        }
        private bool InDocument()
        {
            return state.Peek() == State.InDocument;
        }
        public void WritePropertyName(string name)
        {
            writer.Write(Indent + name + ": ");
            state.Push(State.InProperty);
        }

        public void WriteStartDocument()
        {
            state.Push(State.InDocument);
        }

        public void WriteEndDocument()
        {
            state.Pop();
        }

        public void WriteStartList()
        {
            if (InList()) writer.Write(Indent + "- ");
            state.Push(State.InList);
            writer.WriteLine();

        }

        public void WriteEndList()
        {
            state.Pop();
            if (InProperty())
            {
                state.Pop();
            }
        }

        public void WriteStartMap()
        {
            if (InList())
            {
                writer.Write(Indent + "- ");
            }

            if (state.Count > 1)
            {
                writer.WriteLine();
                IncreaseIndent();
            }
            state.Push(State.InMap);
        }

        public void WriteEndMap()
        {

            state.Pop();
            if (!InDocument())
            {
                DecreaseIndent();
            }
            if (InProperty())
            {
                state.Pop();
            }
        }

        public void WriteValue(string value)
        {
            if (InList()) writer.Write(Indent + "- ");
            if (value.Contains("#"))
            {
                writer.Write("\"");
                writer.Write(value);
                writer.WriteLine("\"");
            } else if (value.Contains("\n") || value.Contains("\r"))
            {
                writer.WriteLine(" |");
                writer.WriteLine(value);
            } else
            {
                writer.WriteLine(value);
            }
            if (InProperty()) state.Pop();
        }

        public void WriteValue(Decimal value)
        {
            if (InList()) writer.Write(Indent + "- ");
            writer.WriteLine(value.ToString());  //TODO deal with culture issues
            if (InProperty()) state.Pop();
        }

        public void WriteValue(int value)
        {
            if (InList()) writer.Write(Indent + "- ");
            writer.WriteLine(value.ToString());  //TODO deal with culture issues
            if (InProperty()) state.Pop();
        }

        public void WriteValue(bool value)
        {
            if (InList()) writer.Write(Indent + "- ");
            writer.WriteLine(value.ToString().ToLower());  //TODO deal with culture issues
            if (InProperty()) state.Pop();
        }

        public void WriteNull()
        {
            if (InList()) writer.Write(Indent + "- ");
            writer.WriteLine("{}");
            //            /*if (InMap())*/ DecreaseIndent();  //Negate decreasing indent in the EndMap 
            if (InProperty()) state.Pop();
        }
    }
}
