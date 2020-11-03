using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Core.EventSystem;
using UnityEngine;

namespace Core.CommandSystem
{
    /// <summary> Parser class for string commands </summary>
    [Serializable]
    public class CommandInterpreter : MonoBehaviour
    {
        [Serializable]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class ArgumentTranslatorAttribute : Attribute
        {
            public const int    c_DefaultOrder = 1;
            public const bool   c_DefaultActive = true;

            public bool         Active { get; set; } = c_DefaultActive;
            public int          Order { get; set; } = c_DefaultOrder;
        }
        
        [Serializable]
        public abstract class ArgumentTranslator
        {
            // translate data
            public abstract bool TryParse(string data, out object value);
        }
        
        [Serializable]
        [ArgumentTranslator(Active = true)]
        private class ArgumentTranslatorConstant : ArgumentTranslator
        {
            public override bool TryParse(string data, out object value)
            {
                // is constant
                switch (data)
                {
                    case "null":
                        value = null;
                        return true;

                    default:
                        value = null;
                        return false;
                }
            }
        }
        
        [Serializable]
        [ArgumentTranslator(Active = true)]
        private class ArgumentTranslatorString : ArgumentTranslator
        {
            public override bool TryParse(string data, out object value)
            {
                // must be quoted string
                if ((data.Length > 1 
                     && data.StartsWith("\"") 
                     && data.EndsWith("\"")) == false)
                {
                    value = default;
                    return false;
                }

                value = data;
                return true;
            }
        }
        
        [Serializable]
        [ArgumentTranslator(Active = true)]
        private class ArgumentTranslatorInt : ArgumentTranslator
        {
            public override bool TryParse(string data, out object value)
            {
                // parse int
                if (int.TryParse(data, out var result))
                {
                    value = result;
                    return true;
                }

                value = null;
                return false;
            }
        }
        
        [Serializable]
        [ArgumentTranslator(Active = true)]
        private class ArgumentTranslatorFloat : ArgumentTranslator
        {
            public override bool TryParse(string data, out object value)
            {
                // parse float
                if (float.TryParse(data, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                {
                    value = result;
                    return true;
                }

                value = null;
                return false;
            }
        }

        public interface ICommandInvoker
        {
            void Init();
            void Release();
            void Run(in CommandInterpreter.CommandCall call);
        }

        public struct CommandCall
        {
            public string           Command;
            public List<object>     Arguments;
        }

        //////////////////////////////////////////////////////////////////////////
        private List<ArgumentTranslator>    m_TranslatorList;

        [SerializeField, SerializeReference, ClassReference]
        private ICommandInvoker             m_CommandInvoker;

        public ICommandInvoker              CommandInvoker
        {
            get => m_CommandInvoker;
            set
            {
                m_CommandInvoker?.Release();
                m_CommandInvoker = value;
                m_CommandInvoker?.Init();
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public void Init()
        {
            // get all ArgumentTranslator inherit classes, apply attribute options active/sorting options
            m_TranslatorList = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(type => 
                    type.IsClass 
                    && type.IsAbstract == false 
                    && type.IsSubclassOf(typeof(ArgumentTranslator)) 
                    && (type.GetCustomAttribute<ArgumentTranslatorAttribute>()?.Active ?? ArgumentTranslatorAttribute.c_DefaultActive))
                .OrderBy(n => (n.GetCustomAttribute<ArgumentTranslatorAttribute>()?.Order ?? ArgumentTranslatorAttribute.c_DefaultOrder))
                .Select(type => (ArgumentTranslator)Activator.CreateInstance(type))
                .ToList();

            // init command processor
            m_CommandInvoker?.Init();
        }

        public void Invoke(string command)
        {
            // must have processor
            if (m_CommandInvoker == null)
                return;

            // command can't be empty
            if (string.IsNullOrEmpty(command))
                return;

            // to lower not case sensitive
            command = command.ToLower();

            // get function name & parameters
            var cmd = Regex.Match(command, @"^(?:\s*)([\w\d]+)(?:\s*\()(.*)(?=\))");

            // bad command
            if (cmd.Success == false)
                return;

            var commandName     = cmd.Groups[1].Value;
            var innerArgs       = cmd.Groups[2].Value;

            // get parameters
            var paramTags = Regex.Matches(innerArgs, @"(""[^""]*""|[^,]+)");

            // parse parameters
            var args = paramTags
                .Cast<Match>()
                .Select(match =>
                {
                    // first or default translation result
                    foreach (var translator in m_TranslatorList)
                        if (translator.TryParse(match.Value, out var result))
                            return result;

                    return default;
                })
                .ToList();

            // invoke
            //Debug.Log($"{functionName}, {args.DeepToString()}");
            m_CommandInvoker.Run(new CommandCall(){Command = commandName, Arguments = args});
        }
    }
}