using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace DIVFactor.Event
{
    /// <summary>
    /// 実行時登録とビルド前検証で共有する、型ごとの不変な解析結果。
    /// </summary>
    public sealed class EventHubMetadata
    {
        static readonly ConcurrentDictionary<Type, Lazy<EventHubMetadata>> Cache = new();
        static readonly Func<Type, Lazy<EventHubMetadata>> MetadataFactory = CreateMetadata;

        public VContainer.Lifetime Lifetime { get; }
        public IReadOnlyList<Type> Dependencies { get; }

        EventHubMetadata(Type hubType)
        {
            if (!hubType.IsClass || hubType.IsAbstract)
                throw new InvalidOperationException($"{hubType} は生成可能なHubクラスではありません。");

            EventHubAttribute hubAttribute = hubType.GetCustomAttribute<EventHubAttribute>(false)
                ?? throw new InvalidOperationException($"{hubType} にEventHub属性がありません。");

            if (!Enum.IsDefined(typeof(VContainer.Lifetime), hubAttribute.Lifetime))
                throw new InvalidOperationException($"{hubType} のLifetimeが不正です: {hubAttribute.Lifetime}");

            const BindingFlags memberFlags = BindingFlags.Instance | BindingFlags.Public |
                                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            bool hasHubConstructor = false;
            bool hasInjectConstructor = false;
            ConstructorInfo injectionConstructor = null;
            ParameterInfo[] constructorParameters = null;
            foreach (ConstructorInfo constructor in hubType.GetConstructors(memberFlags))
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                hasHubConstructor |= constructor.IsPublic && parameters.Length >= 2;

                // VContainerと同じく、[Inject]を優先し、未指定なら最大引数数のコンストラクタを使う。
                if (constructor.IsDefined(typeof(VContainer.InjectAttribute), false))
                {
                    if (hasInjectConstructor)
                        throw new InvalidOperationException($"{hubType} に[Inject]付きコンストラクタが複数あります。");
                    hasInjectConstructor = true;
                    injectionConstructor = constructor;
                    constructorParameters = parameters;
                }
                else if (!hasInjectConstructor &&
                         (constructorParameters == null || parameters.Length > constructorParameters.Length))
                {
                    injectionConstructor = constructor;
                    constructorParameters = parameters;
                }
            }

            List<MethodInfo> injectionMethods = new();
            HashSet<MethodInfo> methodDefinitions = new();
            int injectMethodParameterCount = 0;
            // 継承元のprivateな[Inject]メソッドは、派生型のGetMethodsだけでは取得できない。
            for (Type declaringType = hubType; declaringType != null && declaringType != typeof(object);
                 declaringType = declaringType.BaseType)
            {
                foreach (MethodInfo method in declaringType.GetMethods(memberFlags))
                {
                    if (!method.IsDefined(typeof(VContainer.InjectAttribute), false)) continue;
                    // VContainerの走査順に合わせ、同じoverrideを検出したら次の基底型へ進む。
                    if (!methodDefinitions.Add(method.GetBaseDefinition())) break;
                    injectionMethods.Add(method);
                    injectMethodParameterCount += method.GetParameters().Length;
                }
            }

            // 注入処理を複数のメソッドに分けても、合計2引数以上ならHubとして扱う。
            if (!hasHubConstructor && injectMethodParameterCount < 2)
                throw new InvalidOperationException(
                    $"{hubType} には2つ以上の引数を持つpublicコンストラクタ、または引数が合計2つ以上の[Inject]付きインスタンスメソッドが必要です。");

            List<Type> dependencies = new();
            HashSet<Type> dependencyTypes = new();
            if (injectionConstructor != null) AddDependencies(injectionConstructor, constructorParameters);
            foreach (MethodInfo method in injectionMethods) AddDependencies(method, method.GetParameters());

            Lifetime = hubAttribute.Lifetime;
            Dependencies = dependencies.AsReadOnly();

            void AddDependencies(MethodBase member, ParameterInfo[] parameters)
            {
                foreach (ParameterInfo parameter in parameters)
                {
                    Type dependencyType = parameter.ParameterType;
                    if (!IsEventPort(dependencyType) && !dependencyType.IsDefined(typeof(EventHubAttribute), false))
                        throw new InvalidOperationException(
                            $"{hubType} の {member.Name} の引数 {parameter.Name} ({dependencyType}) はEventPortまたは属性付きHubである必要があります。");

                    if (dependencyTypes.Add(dependencyType)) dependencies.Add(dependencyType);
                }
            }
        }

        // ビルド時はジェネリック定義、DI登録時は型引数が確定した型をそれぞれキャッシュする。
        public static EventHubMetadata Get(Type hubType)
        {
            if (hubType == null) throw new ArgumentNullException(nameof(hubType));
            return Cache.GetOrAdd(hubType, MetadataFactory).Value;
        }

        static Lazy<EventHubMetadata> CreateMetadata(Type hubType) =>
            new(() => new EventHubMetadata(hubType), LazyThreadSafetyMode.ExecutionAndPublication);

        internal static bool IsEventPort(Type dependencyType) =>
            dependencyType.IsGenericType && dependencyType.GetGenericTypeDefinition() == typeof(EventPort<>);
    }
}