sampler2D TextureSampler;

float4 DefaultShader( float2 Tex : TEXCOORD0 ) : COLOR0
{
    float4 Color;
    
    Color = tex2D( TextureSampler, Tex.xy);
    return Color;
}


technique Default
{
    pass p1
    {
        VertexShader = null;
        PixelShader = compile ps_2_0 MyShader();
    }
}
