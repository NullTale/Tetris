using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace Core.CommandSystem
{
    [Serializable]
    public class CommandInvoker : CommandInterpreter.ICommandInvoker
    {
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class RunCommandAttribute : Attribute
        {
        }

        public abstract class Command
        {
            public virtual string       Name
            {
                get
                {
                    var result = GetType().Name;

                    // remove prefix if has
                    if (result.StartsWith("Cmd"))
                        result = result.Substring("Cmd".Length);
                    else
                    if (result.StartsWith("Command"))
                        result = result.Substring("Command".Length);

                    // lower case result
                    return result.ToLower();
                }
            }

            [SerializeField]
            private List<MethodInfo>    m_Methods;
            
            //////////////////////////////////////////////////////////////////////////
            public void Init()
            {
                m_Methods = GetType()
                    .GetMethods()
                    .Where(n => n.CustomAttributes.Any(atr => atr.AttributeType == typeof(RunCommandAttribute)))
                    .ToList();
            }

            public void Run(List<object> args)
            {
                var runMethod = m_Methods.FirstOrDefault(method =>
                {
                    var parameterInfos = method.GetParameters();

                    // same length
                    if (parameterInfos.Length != args.Count)
                        return false;

                    // same or assignable types
                    for (var n = 0; n < parameterInfos.Length; n++)
                    {
                        if (parameterInfos[n].ParameterType.IsInstanceOfType(args[n]) == false)
                            return false;
                    }

                    // 
                    return true;
                });

                // run available method
                if (runMethod != null)
                    runMethod.Invoke(this, args.ToArray());
            }
        }

        private class CmdLog : Command
        {
            public override string Name => "cmd";
            
            //////////////////////////////////////////////////////////////////////////
            [RunCommand]
            public void LogInt(int i)
            {
                Debug.Log(i);
            }

            [RunCommand]
            public void LogFloat(float f)
            {
                Debug.Log(f);
            }

            [RunCommand]
            public void LogString(string str)
            {
                Debug.Log(str);
            }

            [RunCommand]
            public void LogDefault()
            {
                Debug.Log($"default");
            }
        }
        
        //////////////////////////////////////////////////////////////////////////
        private Dictionary<string, Command>    m_CommandDictionary;

        //////////////////////////////////////////////////////////////////////////
        public void Init()
        {
            // get all Command inherit classes
            m_CommandDictionary = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.IsClass && type.IsAbstract == false && type.IsSubclassOf(typeof(Command)))
                .Select(type => (Command)Activator.CreateInstance(type))
                .ToDictionary(k => k.Name, v => v);

            // init commands
            foreach (var command in m_CommandDictionary.Values)
                command.Init();
        }

        public void Release()
        {
        }

        public void Run(in CommandInterpreter.CommandCall call)
        {
            // call command from dictionary
            if (m_CommandDictionary.TryGetValue(call.Command, out var cmd))
                cmd.Run(call.Arguments);
        }
    }
}