namespace Hanamura
{
    public class GameObject
    {
        public record struct Part(string Name, int Index, LocalTransform Transform, int[]? Children);
        public Part[] Parts { get; }
        
        private readonly Dictionary<int, int> _parentLookup;
    
        public GameObject(Part[] meshNames)
        {
            Parts = meshNames;
            _parentLookup = new Dictionary<int, int>();
        
            for (var i = 0; i < Parts.Length; i++)
            {
                var children = Parts[i].Children;
                if (children == null) continue;
                foreach (var child in children)
                {
                    _parentLookup[child] = i;
                }
            }
        }

        public int GetDepthOfPart(int part)
        {
            if (!_parentLookup.ContainsKey(part))
            {
                return 0;
            }
            
            var depth = 0;
            var currentPart = part;
        
            while (_parentLookup.ContainsKey(currentPart))
            {
                depth++;
                currentPart = _parentLookup[currentPart];
            }
        
            return depth;
        }
        
        public Part GetParentOfPart(int part)
        {
            return Parts[_parentLookup[part]];
        }
    }
}