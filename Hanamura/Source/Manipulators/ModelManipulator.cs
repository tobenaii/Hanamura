using MoonTools.ECS;
using MoonTools.ECS.Collections;

namespace Hanamura
{
    public class ModelManipulator : Manipulator
    {
        public ModelManipulator(World world) : base(world)
        {
        }

        public void AddModel<T>(Entity entity, AssetRef modelRef, T material) where T : unmanaged, IMaterial
        {
            var model = AssetStore.GetModel(modelRef);

            var entities = new NativeArray<Entity>(model.Parts.Length);
            for (var i = 0; i < model.Parts.Length; i++)
            {
                entities[i] = World.CreateEntity();
            }

            for (var i = 0; i < model.Parts.Length; i++)
            {
                var part = model.Parts[i];
                var meshEntity = entities[i];
                World.Set(meshEntity, new HasMesh(part.Name));
                World.Set(meshEntity, material);
                World.Set(meshEntity, part.Transform);
                switch (model.GetDepthOfPart(i))
                {
                    case 0:
                        World.Set(meshEntity, new ChildDepth1());
                        World.Set(meshEntity, new Parent(entity));
                        break;
                    case 1:
                        World.Set(meshEntity, new ChildDepth2());
                        World.Set(meshEntity, new Parent(entities[model.GetParentOfPart(i).Index]));
                        break;
                    case 2:
                        World.Set(meshEntity, new ChildDepth3());
                        World.Set(meshEntity, new Parent(entities[model.GetParentOfPart(i).Index]));
                        break;
                }
            }
        }

        public void SetMaterial<T>(AssetRef mesh, T material) where T : IMaterial
        {
            throw new NotImplementedException();
        }
    }
}