using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;
using static MugenForever.Util.AbstractReaderConfig;

namespace MugenForever.Util
{
    internal abstract class AbstractReaderConfig
    {
        protected Dictionary<string, List<List<string>>> groups = new();

        private StringBuilder content;

        public AbstractReaderConfig(Stream data) {

            using (var reader = new StreamReader(data))
            {
                string currentGroup = null;
                List<List<string>> groupValues = null;
                string lastComment = null;

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string currentLine = RemoveComment(line).Trim();

                    if (string.IsNullOrEmpty(currentLine)) continue;

                    if (currentLine.StartsWith("[") && currentLine.EndsWith("]"))
                    {
                        currentGroup = currentLine.Trim('[', ']');
                        if (!groups.ContainsKey(currentGroup))
                        {
                            groups[currentGroup] = new List<List<string>>();
                        }
                        groupValues = groups[currentGroup];
                        groupValues.Add(new List<string>());
                    }
                    else if (groupValues != null)
                    {
                        groupValues[groupValues.Count - 1].Add(currentLine);
                    }
                    else if (lastComment == null && !string.IsNullOrEmpty(currentLine))
                    {
                        lastComment = currentLine;
                    }
                }

                if (currentGroup != null && groupValues != null && lastComment != null)
                {
                    groupValues[groupValues.Count - 1].Add(lastComment);
                }

                Debug.Log(groups);
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

    }
}
