#if !defined(EVALUATE_SIMPLEX_NOISE_2D)
#define EVALUATE_SIMPLEX_NOISE_2D

int FastFloor(float x)
{
    return x > 0 ? (int)x : (int)x - 1;
}

float Dot(float2 g, float x, float y)
{
    return g.x * x + g.y * y;
}

float2 Grad(int hash)
{
    int h = hash & 7;
    float2 grad = float2((h < 4) ? 1.0f : 0.0f, (h < 4) ? 0.0f : 1.0f);
    if ((h & 1) != 0) grad.x = -grad.x;
    if ((h & 2) != 0) grad.y = -grad.y;
    return grad;
}

#endif