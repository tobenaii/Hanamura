using MoonWorks.Graphics;

namespace Hanamura.Plants
{
    public readonly record struct PlantConfig(int NameId, int DescriptionId, int IconId, Color Color, int ManaRequirement)
    {
    }
}