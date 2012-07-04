uniform extern float4x4 WVPMatrix;

uniform extern texture SpriteTexture;
struct PS_INPUT
{
    #ifdef XBOX
        float2 TexCoord : SPRITETEXCOORD;
    #else
        float2 TexCoord : TEXCOORD0;
    #endif
    
};
sampler Sampler = sampler_state
{
	Texture = <SpriteTexture>;
};						
float4 PixelShader(PS_INPUT input) : COLOR0
{
    float2 texCoord;

    texCoord = input.TexCoord.xy;

    return tex2D(Sampler, texCoord);
}

float4 VertexShader(float4 pos : POSITION0) : POSITION0
{
	return mul(pos, WVPMatrix);
}

technique PointSpriteTechnique
{
	pass P0
	{
		vertexShader = compile vs_2_0 VertexShader();
		pixelShader = compile ps_2_0 PixelShader();
	}
}    
