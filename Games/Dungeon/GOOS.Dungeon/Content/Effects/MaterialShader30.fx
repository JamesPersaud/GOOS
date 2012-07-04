//////////////////////////////////////////////////////////////
// Example 5.1                                              //
//                                                          //
// The include statement is interpreted automatically by    //
// the XNA Framework Content Pipeline when effects are      //
// imported though the Content Pipeline.  While you don't   //
// have to explicitly add the include file to the           //
// project, it's often useful for organizational purposes.  //
//////////////////////////////////////////////////////////////
#include "Includes.inc"


shared Light lights[8];

float4 MultipleLightPS(PixelShaderInput input) : COLOR
{
    float4 diffuseColor = materialColor;
    float4 specularColor = materialColor;
     
    float depth = input.PositionZW.x; // input.PositionZW.y;
     
    if(diffuseTexEnabled)
    {
        diffuseColor *= tex2D(diffuseSampler, input.TexCoords);
        //TEST
        //diffuseColor += tex2D(diffuseSampler, input.TexCoords+0.001);
        //diffuseColor += tex2D(diffuseSampler, input.TexCoords+0.002);
        //diffuseColor += tex2D(diffuseSampler, input.TexCoords+0.003);
        
        //diffuseColor = diffuseColor/4;
    }
     
    if(specularTexEnabled)
    {
        specularColor *= tex2D(specularSampler, input.TexCoords);
        //TEST
        //specularColor += tex2D(specularSampler, input.TexCoords+0.001);
        //specularColor += tex2D(specularSampler, input.TexCoords+0.002);
        //specularColor += tex2D(specularSampler, input.TexCoords+0.003);
        
        //specularColor = specularColor/4;
    }
    
    //BLUR??        
     
    float4 color = ambientLightColor * diffuseColor;
     
    //////////////////////////////////////////////////////////////
    // Example 5.2                                              //
    //                                                          //
    // Each light is summed into a final pixel color.  The 3.0  //
    // shader supports dynamic control instructions, which      //
    // allows for loops to  behave as they would on a general   //
    // purpose CPU.                                             //
    //////////////////////////////////////////////////////////////
    for(int i=0; i< numLights; i++)
    {
        color += CalculateSingleLight(lights[i], 
                  input.WorldPosition, input.WorldNormal,
                  diffuseColor, specularColor );
    }
    color.a = 1.0;
    
    //Add fog
    //if(depth >1)
    //{
		//color.x = depth;
		//color.y = 0;
		//color.z = 0;
    //}               
    
    
  
    
    return color;
}


technique MultipleLights
{

    pass P0
    {
        //////////////////////////////////////////////////////////////
        // Example 5.3                                              //
        //                                                          //
        // Render, sampler, and texture states can be set in an     //
        // effect, in addition to Vertex and Pixel shaders states.  //
        // Many effect will not be sensible without a specific      //
        // set of states that correspond to the kind of shaders     //
        // being employed.                                          //
        //////////////////////////////////////////////////////////////
        
        //set sampler states
        MagFilter[0] = ANISOTROPIC;
        MinFilter[0] = ANISOTROPIC;
        MipFilter[0] = ANISOTROPIC;
        AddressU[0] = WRAP;
        AddressV[0] = WRAP;
        MagFilter[1] = ANISOTROPIC;
        MinFilter[1] = ANISOTROPIC;
        MipFilter[1] = ANISOTROPIC;
        AddressU[1] = WRAP;
        AddressV[1] = WRAP;
        
        //set texture states (notice the '<' , '>' brackets)
        //as the texture state assigns a reference
        Texture[0] = <diffuseTexture>;
        Texture[1] = <specularTexture>;
        
        // set render states
        AlphaBlendEnable = FALSE;

        //set pixel shader states
        VertexShader = compile vs_3_0 BasicVS();
        PixelShader = compile ps_3_0 MultipleLightPS();
    }
}