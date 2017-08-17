using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MugenForever
{
    public class DefChar : MugenForever.Reader.Text
    {
        public string displayname;
        public string versiondate;
        public string mugenversion;
        public string author;
        public string paldefaults;
        public string localcoord;

        [Header("Files")]
        public string cmd;
        public string cns;
        public List<string> st;
        public string ai;
        public string sprite;
        public string anim;
        public string sound;

        [Header("Arcade")]
        public string introstoryboard;
        public string endingstoryboard;

        [Header("Palette Keymap")]
        public string x;
        public string y;
        public string z;
        public string a;
        public string b;
        public string c;

        [Header("Pals")]
        public List<string> pal;

        void Start()
        {
            if ( !string.IsNullOrEmpty(fileName))
            {
                ReadFromFile(fileName);
            }
        }

        [ContextMenu("Load From File")]
        public void LoadInEditor()
        {
            string file = EditorUtility.OpenFilePanel("Select Mugen DEF Char File", "", "def");

            if (file.Length != 0)
            {
                ReadFromFile(file);
                fileName = file;
            }
        }

        public override void ReadFromFile(string file)
        {
            string text = System.IO.File.ReadAllText(file);
            pal.Clear();
            st.Clear();

            List<string> lines = new List<string>();
            lines.AddRange(text.Split("\n"[0]));

            foreach (string line in lines)
            {
                ParseLine(line);
            }
        }

        protected void ParseLine(string line)
        {
            if (line.Contains("=") )
            {
                List<string> parts = new List<string>();
                parts.AddRange(line.Split("="[0]));
                
                if ( parts.Count == 2)
                {
                    //remove point to fill paldefaults
                    string variable = ParseValue(parts[0].Trim().Replace(".",""));
                    string value = ParseValue(parts[1]);

                    if (variable.StartsWith("pal") && !variable.Contains("paldefaults"))
                    {
                        pal.Add(value);
                    }
                    else if (variable.StartsWith("st"))
                    {
                        st.Add(value);
                    }
                    else if (!string.IsNullOrEmpty(variable))
                    {
                        SetVariable(variable, value);
                    }
                }
            }
        }

        protected void SetVariable(string variable, string value)
        {
            try
            {
                //default unity variable "name" not work with reflection
                if (variable == "name")
                {
                    name = value;

                    return;
                }

                System.Type T = typeof(DefChar);

                System.Reflection.FieldInfo reflectionField = T.GetField(variable);
                //int tmp = (int)reflectionField.GetValue(this);

                reflectionField.SetValue(this, value);
            }
            catch (System.Exception ex)
            {
                Debug.Log("Error="+variable+" - "+ex.Message);                
            }
            
        }
    }
}
