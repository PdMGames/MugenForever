using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MugenForever.Reader
{
    public class Text : Reader
    {
        /*
         * Reme mugen comments and, "" and make a trim
         */
        protected string ParseValue(string value)
        {
            value.Trim();

            //remove comments
            if (value.Contains(";"))
            {

                List<string> parts = new List<string>();
                parts.AddRange(value.Split(";"[0]));

                value = parts[0].Trim();
            }

            value = value.Replace("\"", "");

            return value.Trim();
        }
    }
}
