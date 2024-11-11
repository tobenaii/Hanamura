using System.Reflection;
using MoonTools.ECS;

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(Hanamura.HotReloadSystem))]

namespace Hanamura
{
    public class HotReloadSystem : MoonTools.ECS.System
    {
        private static List<Type> ReloadedTypes { get; } = new();
        
        internal static void UpdateApplication(Type[]? types)
        {
            if (types == null) return;
            foreach (var type in types)
            {
                if (typeof(IGameEntity).IsAssignableFrom(type))
                {
                    ReloadedTypes.Add(type);
                }
            }
        }
        
        private readonly Dictionary<Type, Filter> _filters = new();
        
        public HotReloadSystem(World world) : base(world)
        {
            foreach (var type in GetTypesImplementingInterface(typeof(IGameEntity)))
            {
                _filters[type] = GetFilterForType(type);
            }
        }

        private static IEnumerable<Type> GetTypesImplementingInterface(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsValueType);
        }
        
        private Filter GetFilterForType(Type type)
        {
            var filterBuilder = World.FilterBuilder;
            var includeMethod = filterBuilder.GetType().GetMethod("Include", BindingFlags.Instance | BindingFlags.Public)!;
            var genericIncludeMethod = includeMethod.MakeGenericMethod(type);
            genericIncludeMethod.Invoke(filterBuilder, null);
            return filterBuilder.Build();
        }
        
        public override void Update(TimeSpan delta)
        {
            foreach (var type in ReloadedTypes)
            {
                var filter = _filters[type];
                Console.WriteLine($"Reconfiguring {type.Name}");
                foreach (var entity in filter.Entities)
                {
                    var buildMethod = type.GetMethod("Configure", BindingFlags.Static | BindingFlags.Public)!;
                    buildMethod.Invoke(null, new object[] {World, entity});
                    Console.WriteLine("Reconfigured entity: " + entity.ID);
                }
            }
            ReloadedTypes.Clear();
        }
    }
}