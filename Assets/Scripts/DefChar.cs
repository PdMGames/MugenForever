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

        // [ContextMenu("Load From File")] // Removido pois EditorUtility não funciona em build
        // public void LoadInEditor()
        // {
        //     // string file = EditorUtility.OpenFilePanel("Select Mugen DEF Char File", "", "def");
        //     // if (file.Length != 0)
        //     // {
        //     //    Load(file); // Chamaria o novo método Load
        //     // }
        // }

        public bool Load(string pathFile)
        {
            if (string.IsNullOrEmpty(pathFile) || !System.IO.File.Exists(pathFile))
            {
                Debug.LogError($"[DefChar.Load] Error: File not found or path is null/empty: {pathFile}");
                return false;
            }
            fileName = pathFile; // Store the filename
            try
            {
                ReadFromFile(pathFile);
                // Consider a load successful if essential data like displayname is populated.
                return !string.IsNullOrEmpty(this.displayname);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[DefChar.Load] Exception while loading DEF file '{pathFile}': {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        // ReadFromFile é mantido como protected ou private, chamado por Load.
        // O 'override' sugere que é de uma classe base MugenForever.Reader.Text
        // Vamos assumir que a classe base permite que este método seja chamado e não requer que seja 'public override'
        // Se ReadFromFile for da classe base e for public, podemos simplesmente chamá-lo.
        // Por agora, vamos tornar este método o core da lógica de leitura.
        protected void ReadFromFile(string file) // Mudado de public override para protected
        {
            string text = System.IO.File.ReadAllText(file);
            if (pal == null) pal = new List<string>(); else pal.Clear();
            st.Clear();

            List<string> lines = new List<string>();
            lines.AddRange(text.Split("\n"[0]));

            foreach (string line in lines)
            {
                ParseLine(line);
            }
        }

        protected virtual void ParseLine(string line) // Adicionado virtual se a classe base tiver um ParseLine que possa ser chamado
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
                //default unity variable "name" (herdado de UnityEngine.Object) não deve ser usado para displayname.
                // O campo 'name' da classe DefChar é o correto.
                // Se a classe DefChar herda de UnityEngine.Object, this.name se refere ao nome do GameObject.
                // No entanto, o DefChar.cs fornecido não parece herdar de MonoBehaviour, então this.name é seguro.
                // Apenas garantindo que 'displayname' seja usado quando se refere ao nome de exibição do char.

                System.Type T = this.GetType(); // Usar GetType() para suportar herança se houver
                System.Reflection.FieldInfo field = T.GetField(variable,
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.IgnoreCase); // Tornar a busca de campo mais flexível

                if (field != null)
                {
                    try
                    {
                        // Tratar conversões de tipo se necessário, por exemplo, para 'localcoord' se fosse um Vector3
                        if (field.FieldType == typeof(string))
                        {
                            field.SetValue(this, value);
                        }
                        // Adicionar mais conversões se outros tipos de campos forem usados
                        else
                        {
                            Debug.LogWarning($"[DefChar.SetVariable] Field '{variable}' is not a string. Type is {field.FieldType}. Value '{value}' not set by reflection directly.");
                        }
                    }
                    catch (System.ArgumentException argEx)
                    {
                        Debug.LogError($"[DefChar.SetVariable] ArgumentException for field '{variable}', value '{value}': {argEx.Message}");
                    }
                }
                else
                {
                    // Debug.LogWarning($"[DefChar.SetVariable] Field '{variable}' not found in class {T.Name}.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[DefChar.SetVariable] Error setting variable '{variable}' to value '{value}': {ex.Message}");
            }
        }
    }
}
