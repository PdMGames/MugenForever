using MugenForever.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace MugenForever.AIR
{
    internal class AIRImpl
    {
        private HashSet<AIRAction> _groups = new();

        public AIRImpl(ReaderConfig data)
        {
            foreach (var group in data.Groups) {
                AIRAction action = new();
                var value = group.Value[0];

                if (data.IsAddLastComment && value[0].StartsWith("COM[") && value[0].EndsWith("]"))
                { 
                    action.Name = value[0].TrimStart("COM[").TrimEnd("]");
                    value.RemoveAt(0);
                }
                else
                {
                    action.Name = group.Key;                    
                }

                var actionNumber = Regex.Replace(group.Key, "[^0-9]", "");
                action.Action = int.Parse(actionNumber);

                AIRFrame.BoxCollision[] boxCollisionDefaults = null;
                AIRFrame.BoxCollision[] boxAttackDefaults = null;
                AIRFrame.BoxCollision[] boxCollisions = null;
                AIRFrame.BoxCollision[] boxAttacks = null;

                bool isDefaultCollision = false;
                bool isDefaultAttack = false;
                bool isCollision = false;
                bool isAttack = false;
                bool isLoopStart = false;
                int totalCollision = 0;

                action.Frames = new();

                foreach (var line in group.Value[0])
                {
                    Match mathCollision = Regex.Match(line.Trim(), "^Clsn(?<clsType>[1|2])(?<clsDefault>Default)?.*?:.*?(?<clsTotal>\\d+?).*?$");
                    if(mathCollision.Success)
                    {
                        var clsType = mathCollision.Groups["clsType"].Value;
                        var clsDefault = mathCollision.Groups["clsDefault"].Value;
                        var clsTotal = mathCollision.Groups["clsTotal"].Value;

                        isDefaultCollision = false;
                        isDefaultAttack = false;
                        isCollision = false;
                        isAttack = false;

                        if (!String.IsNullOrEmpty(clsDefault) && clsDefault == "Default") { 
                            if (clsType == "1")
                            {
                                totalCollision = int.Parse(clsTotal);
                                boxCollisionDefaults = new AIRFrame.BoxCollision[totalCollision];
                                isDefaultCollision = true;
                                continue;
                            }
                            else if (clsType == "2")
                            {
                                totalCollision = int.Parse(clsTotal);
                                boxAttackDefaults = new AIRFrame.BoxCollision[totalCollision];
                                isDefaultAttack = true;
                                continue;
                            }
                        }
                        else
                        {
                            if (clsType == "1")
                            {
                                totalCollision = int.Parse(clsTotal);
                                boxCollisions = new AIRFrame.BoxCollision[totalCollision];
                                isCollision = true;
                                continue;
                            }
                            else if (clsType == "2")
                            {
                                totalCollision = int.Parse(clsTotal);
                                boxAttacks = new AIRFrame.BoxCollision[totalCollision];
                                isAttack = true;
                                continue;
                            }
                        }

                    }

                    if(totalCollision > 0)
                    {
                        var collisionInfo = Regex.Replace(line.Trim().Replace("\\s", "").Replace("=", ""), "Clsn[1|2]\\[\\d+?\\]", "").Split(",");
                        
                        if (collisionInfo.Length < 4) 
                            throw new Exception("Invalid collision info");

                        var xStart = int.Parse(collisionInfo[0]);
                        var yStart = int.Parse(collisionInfo[1]);
                        var xEnd = int.Parse(collisionInfo[2]);
                        var yEnd = int.Parse(collisionInfo[3]);

                        var width = Math.Abs(xEnd - xStart);
                        var height = Math.Abs(yEnd - yStart);

                        if(isDefaultCollision)
                        {
                            boxCollisionDefaults[boxCollisionDefaults.Length - totalCollision] = new AIRFrame.BoxCollision
                            {
                                AxisX = xStart,
                                AxisY = yStart,
                                Width = width,
                                Height = height
                            };
                        }
                        else if(isDefaultAttack)
                        {
                            boxAttackDefaults[boxAttackDefaults.Length - totalCollision] = new AIRFrame.BoxCollision
                            {
                                AxisX = xStart,
                                AxisY = yStart,
                                Width = width,
                                Height = height
                            };
                        }
                        else if(isCollision)
                        {
                            boxCollisions[boxCollisions.Length - totalCollision] = new AIRFrame.BoxCollision
                            {
                                AxisX = xStart,
                                AxisY = yStart,
                                Width = width,
                                Height = height
                            };
                        }
                        else if(isAttack)
                        {
                            boxAttacks[boxAttacks.Length - totalCollision] = new AIRFrame.BoxCollision
                            {
                                AxisX = xStart,
                                AxisY = yStart,
                                Width = width,
                                Height = height
                            };
                        }

                        totalCollision--;
                        continue;
                    }

                    if(totalCollision == 0)
                    {
                        if(line.Trim().ToLower() == "loopstart")
                        {
                            isLoopStart = true;
                            continue;
                        }

                        var frameInfo = line.Trim().Replace("\\s", "").Split(",");

                        Debug.Log(line);

                        if (frameInfo.Length < 5)
                            throw new Exception("Invalid frame info");

                        var groupIndex = frameInfo[0];
                        var frameIndex = frameInfo[1];
                        var axisX = frameInfo[2];
                        var axisY = frameInfo[3];
                        var time = frameInfo[4];
                        AIRFrame.FlipType flipType = AIRFrame.FlipType.NONE;

                        if (frameInfo.Length > 5)
                        {
                            var flip = frameInfo[5];
                            if (flip == "H")
                                flipType = AIRFrame.FlipType.H;
                            else if (flip == "V")
                                flipType = AIRFrame.FlipType.V;
                            else if (flip == "HV" || flip == "VH")
                                flipType = AIRFrame.FlipType.HV;
                        }

                        AIRFrame frame = new()
                        {
                            Group                       = int.Parse(groupIndex),
                            Index                       = int.Parse(frameIndex),
                            AxisX                       = int.Parse(axisX),
                            AxisY                       = int.Parse(axisY),
                            Time                        = int.Parse(time),
                            BoxAttackDefaults           = boxAttackDefaults,
                            BoxCollisionDefaults        = boxCollisionDefaults,
                            BoxAttacks                  = boxAttacks,
                            BoxCollisions               = boxCollisions,
                            Flip                        = flipType,
                            StartLoop                   = isLoopStart,
                        };

                        action.Frames.Add(frame);
                        isLoopStart = false;
                    }
                }
                _groups.Add(action);
            }            

            // Imprima o JSON
            Debug.Log(json());
        }

        public class AIRAction
        {
            public string Name;
            public int Action;
            public List<AIRFrame> Frames;
        }

        public class AIRFrame
        {
            public int Group;
            public int Index;
            public int AxisX;
            public int AxisY;
            public int Time;
            public FlipType Flip;
            public bool StartLoop;
            public BoxCollision[] BoxCollisionDefaults;
            public BoxCollision[] BoxAttackDefaults;
            public BoxCollision[] BoxCollisions;
            public BoxCollision[] BoxAttacks;

            public enum FlipType
            {
                NONE,
                H,
                V,
                HV
            }

            public class BoxCollision
            {
                public int Width;
                public int Height;
                public int AxisX;
                public int AxisY;
            }
        }

        public string json()
        {
            // ...

            // Crie um StringBuilder para construir o JSON
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            // Adicione cada AIRAction individualmente ao JSON
            foreach (var action in _groups)
            {
                sb.Append("\"Name\": \"" + action.Name + "\", ");
                sb.Append("\"Action\": " + action.Action + ", ");
                sb.Append("\"Frames\": [");

                // Adicione cada AIRFrame individualmente ao JSON
                foreach (var frame in action.Frames)
                {
                    sb.Append("{");
                    sb.Append("\"Group\": " + frame.Group + ", ");
                    sb.Append("\"Index\": " + frame.Index + ", ");
                    sb.Append("\"AxisX\": " + frame.AxisX + ", ");
                    sb.Append("\"AxisY\": " + frame.AxisY + ", ");
                    sb.Append("\"Time\": " + frame.Time + ", ");
                    sb.Append("\"Flip\": \"" + frame.Flip.ToString() + "\", ");
                    sb.Append("\"StartLoop\": " + frame.StartLoop.ToString().ToLower() + ", ");
                    sb.Append("\"BoxCollisionDefaults\": [");

                    // Adicione cada BoxCollision individualmente ao JSON
                    if (frame.BoxCollisionDefaults != null)
                    {
                        for (int i = 0; i < frame.BoxCollisionDefaults.Length; i++)
                        {
                            var boxCollision = frame.BoxCollisionDefaults[i];
                            sb.Append("{");
                            sb.Append("\"Width\": " + boxCollision.Width + ", ");
                            sb.Append("\"Height\": " + boxCollision.Height + ", ");
                            sb.Append("\"AxisX\": " + boxCollision.AxisX + ", ");
                            sb.Append("\"AxisY\": " + boxCollision.AxisY);
                            sb.Append("}");

                            // Adicione vírgula entre os elementos, exceto o último
                            if (i < frame.BoxCollisionDefaults.Length - 1)
                                sb.Append(", ");
                        }
                    }

                    sb.Append("], ");
                    sb.Append("\"BoxAttackDefaults\": [");

                    // Adicione cada BoxCollision individualmente ao JSON
                    if (frame.BoxAttackDefaults != null)
                    {
                        for (int i = 0; i < frame.BoxAttackDefaults.Length; i++)
                        {
                            var boxCollision = frame.BoxAttackDefaults[i];
                            sb.Append("{");
                            sb.Append("\"Width\": " + boxCollision.Width + ", ");
                            sb.Append("\"Height\": " + boxCollision.Height + ", ");
                            sb.Append("\"AxisX\": " + boxCollision.AxisX + ", ");
                            sb.Append("\"AxisY\": " + boxCollision.AxisY);
                            sb.Append("}");

                            // Adicione vírgula entre os elementos, exceto o último
                            if (i < frame.BoxAttackDefaults.Length - 1)
                                sb.Append(", ");
                        }
                    }

                    sb.Append("], ");
                    sb.Append("\"BoxCollisions\": [");

                    // Adicione cada BoxCollision individualmente ao JSON
                    if (frame.BoxCollisions != null)
                    {
                        for (int i = 0; i < frame.BoxCollisions.Length; i++)
                        {
                            var boxCollision = frame.BoxCollisions[i];
                            sb.Append("{");
                            sb.Append("\"Width\": " + boxCollision.Width + ", ");
                            sb.Append("\"Height\": " + boxCollision.Height + ", ");
                            sb.Append("\"AxisX\": " + boxCollision.AxisX + ", ");
                            sb.Append("\"AxisY\": " + boxCollision.AxisY);
                            sb.Append("}");

                            // Adicione vírgula entre os elementos, exceto o último
                            if (i < frame.BoxCollisions.Length - 1)
                                sb.Append(", ");
                        }
                    }

                    sb.Append("], ");
                    sb.Append("\"BoxAttacks\": [");

                    // Adicione cada BoxCollision individualmente ao JSON
                    if (frame.BoxAttacks != null)
                    {
                        for (int i = 0; i < frame.BoxAttacks.Length; i++)
                        {
                            var boxCollision = frame.BoxAttacks[i];
                            sb.Append("{");
                            sb.Append("\"Width\": " + boxCollision.Width + ", ");
                            sb.Append("\"Height\": " + boxCollision.Height + ", ");
                            sb.Append("\"AxisX\": " + boxCollision.AxisX + ", ");
                            sb.Append("\"AxisY\": " + boxCollision.AxisY);
                            sb.Append("}");

                            // Adicione vírgula entre os elementos, exceto o último
                            if (i < frame.BoxAttacks.Length - 1)
                                sb.Append(", ");
                        }
                    }

                    sb.Append("}");

                    // Adicione vírgula entre os elementos, exceto o último
                    if (frame != action.Frames[action.Frames.Count - 1])
                        sb.Append(", ");
                }

                sb.Append("]");

                // Adicione vírgula entre os elementos, exceto o último
                if (action != _groups.Last())
                    sb.Append(", ");
            }

            sb.Append("}");
            return sb.ToString();
        }


    }

}
