using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using System.Windows.Forms;

namespace Util.Commands
{

    public static class commandController
    {

        public static string ValidateCommands(string __message, string commandSide = "")
        {
            //caso algum comando seja criado com um denominador diferente do /, será implementado aqui.
            //posibilidade de expansão para outros comandos.
            List<string> startCommands = new List<string>() { "/", "#" };
            bool commandFound = startCommands.Any(__message.StartsWith);
            //se algum comando for encontrado, sera separado e retornado.
            if (commandFound)
            {

                return getCommand(__message);
            }
            else
            {
                return null;
            }
        }

        public static string getCommand(string __message)
        {
            //forma simples de capturar o comando, todo comando deve iniciar com os Starters e finalizar com uum espaço em branco;
            int i = __message.IndexOf(' ');
            int lengthCommand = i == -1 ? __message.Length : i;
            string command = __message.Substring(0, lengthCommand);
            return command;
        }


        public static string getFirstArgument(string __message, string __commandFound)
        {
            //neste momento o comando ja foi encontrado e validado, é um comando existente.
            string __messageWithoutCommand = __message.Substring(__commandFound.Length).TrimStart();
            var indexes = __messageWithoutCommand.AllIndexesOf(" ").ToList();
            var __firstArgument = string.Empty;

            if (indexes.Count > 0)
            {
                __firstArgument = __messageWithoutCommand.Substring(0, indexes[0]);
            }
            else
            {
                __firstArgument = __messageWithoutCommand;
            }
            return __firstArgument;
        }


        public static string getSecondArgument(string __message, string __commandFound, string __firstArgument = "")
        {

            if (string.IsNullOrWhiteSpace(__firstArgument))
            {
                __firstArgument = getFirstArgument(__message, __commandFound);
            }
            //neste momento o comando ja foi encontrado e validado, é um comando existente.
            string _messageWithoutCommand = __message.Substring(__commandFound.Length).TrimStart();
            var indexes = _messageWithoutCommand.AllIndexesOf(" ").ToList();
            var __secondArgument = string.Empty;

            if (indexes.Count > 0)
            {
                __secondArgument = _messageWithoutCommand.Substring(indexes[0]).TrimStart();
            }
            return __secondArgument;
        }

        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class, IComparable<T>
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            objects.Sort();
            return objects;
        }


        public static IEnumerable<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    break;
                yield return index;
            }
        }

    }





}
