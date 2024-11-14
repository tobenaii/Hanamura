using MoonTools.ECS;

namespace Hanamura
{
    public class GameWorld
    {
        private AssetStore AssetStore { get; }
        private World World { get; }
        private List<Entity> _childEntities = new(10);
        
        public GameWorld(AssetStore assetStore, World world)
        {
            AssetStore = assetStore;
            World = world;
        }
        
        public EntityBuilder Spawn<T>() where T : unmanaged, IGameEntity
        {
            var entity = World.CreateEntity();
            World.Set(entity, default(T));
            if (T.GameObject != string.Empty)
            {
                var gameObject = AssetStore.GetGameObject(T.GameObject);
                _childEntities.Clear();
                for (var i = 0; i < gameObject.Parts.Length; i++)
                {
                    _childEntities.Add(World.CreateEntity());
                }
                for (var i = 0; i < gameObject.Parts.Length; i++)
                {
                    var part = gameObject.Parts[i];
                    var meshEntity = _childEntities[i];
                    World.Set(meshEntity, new MeshConfig(T.GameObject + "." + part.Name));
                    World.Set(meshEntity, new StandardMaterialConfig("PixPal_BaseColor"));
                    World.Set(meshEntity, part.Transform);
                    switch (gameObject.GetDepthOfPart(i))
                    {
                        case 0:
                            World.Set(meshEntity, new ChildDepth1());
                            World.Set(meshEntity, new Parent(entity));
                            break;
                        case 1:
                            World.Set(meshEntity, new ChildDepth2());
                            World.Set(meshEntity, new Parent(_childEntities[gameObject.GetParentOfPart(i).Index]));
                            break;
                        case 2:
                            World.Set(meshEntity, new ChildDepth3());
                            World.Set(meshEntity, new Parent(_childEntities[gameObject.GetParentOfPart(i).Index]));
                            break;
                    }
                }
            }
            T.Configure(World, entity);
            T.SetState(World, entity);
            return new EntityBuilder(World, entity);
        }
    }
}