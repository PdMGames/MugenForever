using System;
using System.Collections.Generic;
using System.IO;

namespace MugenForever.Util
{
    internal class ReaderConfig
    {
        private Dictionary<string, List<List<string>>> _groups = new();
        private bool _isAddLastComment { get; set; }
        public ReaderConfig(Stream data, bool isAddLastComment=false) {

            using var reader = new StreamReader(data);
            string currentGroup = null;
            List<List<string>> groupValues = null;
            string lastComment = null;
            _isAddLastComment = isAddLastComment;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith(";") && isAddLastComment)
                    lastComment = String.Format("COM[{0}]", line.TrimStart(';').Trim());

                string currentLine = RemoveComment(line).Trim();

                if (string.IsNullOrEmpty(currentLine)) continue;

                if (currentLine.StartsWith("[") && currentLine.EndsWith("]"))
                {
                    currentGroup = currentLine.Trim('[', ']');
                    if (!_groups.ContainsKey(currentGroup))
                    {
                        _groups[currentGroup] = new List<List<string>>();
                    }
                    groupValues = _groups[currentGroup];
                    groupValues.Add(new List<string>());
                    
                    if(isAddLastComment && lastComment != null)
                    {
                        groupValues[groupValues.Count - 1].Add(lastComment);
                        lastComment = null;
                    }

                }
                else if (groupValues != null)
                {
                    groupValues[groupValues.Count - 1].Add(currentLine);
                }
            }

            if (currentGroup != null && groupValues != null && lastComment != null)
            {
                groupValues[groupValues.Count - 1].Add(lastComment);
            }


        }

        static string RemoveComment(string line)
        {
            int commentIndex = line.IndexOf(';');
            if (commentIndex >= 0)
            {
                return line.Substring(0, commentIndex).Trim();
            }
            return line;
        }

        public Dictionary<string, List<List<string>>> Groups { get { return _groups; } }
        public bool IsAddLastComment { get { return _isAddLastComment; } }
        public bool HasGroup(string group) { return _groups.ContainsKey(group); }
        public List<List<string>> GetGroup(string group) { return _groups[group]; }
        public List<string> GetGroup(string group, int index) { return _groups[group][index]; }

    }
}
