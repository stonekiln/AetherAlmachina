using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DIVFactor.Event;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Compilation;
using UnityEditor.UnityLinker;

namespace AetherAlmachina.Editor.DIVFactor
{
    /// <summary>
    /// Hubの追加時に保持設定を手作業で更新せずに済むよう、ビルドごとに生成する。
    /// </summary>
    public sealed class EventHubLinkerProcessor : IUnityLinkerProcessor
    {
        public int callbackOrder => 0;

        public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData buildData)
        {
            UnityEditor.Compilation.Assembly[] playerAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);
            HashSet<string> playerAssemblyNames = new(playerAssemblies.Select(assembly => assembly.name));
            foreach (UnityEditor.Compilation.Assembly assembly in playerAssemblies)
            {
                foreach (string referencePath in assembly.compiledAssemblyReferences)
                    playerAssemblyNames.Add(Path.GetFileNameWithoutExtension(referencePath));
            }

            Type[] hubTypes = TypeCache.GetTypesWithAttribute<EventHubAttribute>()
                .Where(hubType => playerAssemblyNames.Contains(hubType.Assembly.GetName().Name))
                .ToArray();

            XDocument document;
            try
            {
                document = CreateDocument(hubTypes);
            }
            catch (InvalidOperationException exception)
            {
                throw new BuildFailedException($"EventHubの保持設定を生成できません: {exception.Message}");
            }

            // 既存のlink.xmlとAssetsを変更せず、このビルドの追加設定として渡す。
            string outputPath = Path.GetFullPath(Path.Combine("Temp", "DIVFactor", buildData.target.ToString(), "link.xml"));
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            using (XmlWriter writer = XmlWriter.Create(outputPath, new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                NewLineChars = "\n"
            }))
                document.Save(writer);

            return outputPath;
        }

        internal static XDocument CreateDocument(IEnumerable<Type> hubTypes)
        {
            // Hubを使わずRegisterEventだけで登録するEventPortも保持する。
            HashSet<Type> preservedTypes = new() { typeof(EventPort<>), typeof(EventHubAttribute) };
            HashSet<Type> visitedHubs = new();
            foreach (Type hubType in hubTypes) Visit(hubType);

            XElement linker = new("linker");
            foreach (IGrouping<string, Type> assemblyGroup in preservedTypes
                .GroupBy(preservedType => preservedType.Assembly.GetName().Name)
                .OrderBy(group => group.Key, StringComparer.Ordinal))
            {
                XElement assemblyElement = new("assembly", new XAttribute("fullname", assemblyGroup.Key));
                foreach (Type preservedType in assemblyGroup.OrderBy(candidate => candidate.FullName, StringComparer.Ordinal))
                    assemblyElement.Add(new XElement("type",
                        new XAttribute("fullname", preservedType.FullName.Replace('+', '/')),
                        new XAttribute("preserve", "all")));
                linker.Add(assemblyElement);
            }

            return new XDocument(linker);

            void Visit(Type hubType)
            {
                Type definitionType = hubType.IsGenericType ? hubType.GetGenericTypeDefinition() : hubType;
                if (!visitedHubs.Add(definitionType)) return;

                EventHubMetadata metadata = EventHubMetadata.Get(definitionType);
                // 派生Hubの保持だけでは、基底型にあるprivateなInjectメソッドを保持できない。
                for (Type declaringType = definitionType; declaringType != null && declaringType != typeof(object);
                     declaringType = declaringType.BaseType)
                    preservedTypes.Add(declaringType.IsGenericType ? declaringType.GetGenericTypeDefinition() : declaringType);
                foreach (Type dependencyType in metadata.Dependencies)
                {
                    if (dependencyType.IsGenericType && dependencyType.GetGenericTypeDefinition() == typeof(EventPort<>))
                        continue;
                    Visit(dependencyType);
                }
            }
        }
    }
}
