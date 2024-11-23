struct VSOutput {
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
};

cbuffer UniformBlock : register(b0, space3)
{
    float3 LightDir;
    float Time;
};

// Fade function to smooth the transition between points
float Fade(float t) {
    return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

// Linear interpolation function
float Lerp(float a, float b, float t) {
    return a + t * (b - a);
}

// Hash function to generate gradient directions
int Hash(int x, int y, int z) {
    // A simple hash function to pseudo-randomly shuffle the coordinates
    return (x * 73856093) ^ (y * 19349663) ^ (z * 83492791);
}

// Gradient function returns a pseudo-random gradient direction and computes the dot product
float Gradient(int hash, float x, float y, float z) {
    // Calculate the gradient vector based on the hash value (modulo 12)
    int h = hash & 15;
    float u = h < 8 ? x : y;
    float v = h < 4 ? y : (h == 12 || h == 14 ? x : z);

    // Return the dot product of the gradient direction and the distance vector
    return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
}

// 3D Perlin Noise function
float PerlinNoise3D(float3 pos) {
    // Grid cell coordinates
    int X = (int)floor(pos.x) & 255;
    int Y = (int)floor(pos.y) & 255;
    int Z = (int)floor(pos.z) & 255;

    // Relative position in cell
    float3 f = pos - floor(pos);

    // Fade curves for each of x, y, z
    float u = Fade(f.x);
    float v = Fade(f.y);
    float w = Fade(f.z);

    // Hash the corner coordinates to get pseudo-random gradient directions
    int A = Hash(X, Y, Z);
    int B = Hash(X + 1, Y, Z);
    int C = Hash(X, Y + 1, Z);
    int D = Hash(X + 1, Y + 1, Z);

    int E = Hash(X, Y, Z + 1);
    int F = Hash(X + 1, Y, Z + 1);
    int G = Hash(X, Y + 1, Z + 1);
    int H = Hash(X + 1, Y + 1, Z + 1);

    // Calculate the gradient dot products
    float n000 = Gradient(A, f.x, f.y, f.z);
    float n100 = Gradient(B, f.x - 1, f.y, f.z);
    float n010 = Gradient(C, f.x, f.y - 1, f.z);
    float n110 = Gradient(D, f.x - 1, f.y - 1, f.z);

    float n001 = Gradient(E, f.x, f.y, f.z - 1);
    float n101 = Gradient(F, f.x - 1, f.y, f.z - 1);
    float n011 = Gradient(G, f.x, f.y - 1, f.z - 1);
    float n111 = Gradient(H, f.x - 1, f.y - 1, f.z - 1);

    // Interpolate the results along x, y, and z
    float nx00 = Lerp(n000, n100, u);
    float nx01 = Lerp(n001, n101, u);
    float nx10 = Lerp(n010, n110, u);
    float nx11 = Lerp(n011, n111, u);

    float nxy0 = Lerp(nx00, nx10, v);
    float nxy1 = Lerp(nx01, nx11, v);

    // Final interpolation
    float result = Lerp(nxy0, nxy1, w);

    // Normalize the result to the range [0, 1]
    return (result + 1.0) * 0.5;
}

float4 main(VSOutput input) : SV_Target {
    float scatteringNoise = PerlinNoise3D((float3)input.Position * 2.0);
    float scattering = max(dot(LightDir, input.Normal), 0.0) * scatteringNoise;
    return float4(1, 1, 1, 0.5);
}
