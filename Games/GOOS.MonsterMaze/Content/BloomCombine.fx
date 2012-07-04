// Pixel shader combines the bloom image with the original
// scene, using tweakable intensity levels and saturation.
// This is the final step in applying a bloom postprocess.

//
//
//  TODO: Add a sine wave 
//
//
//


sampler BloomSampler : register(s0);
sampler BaseSampler : register(s1);

float BloomIntensity;
float BaseIntensity;

float BloomSaturation;
float BaseSaturation;

//Post fade settings
bool PostFade;
bool FadeTo;
float FadeLevel;
float FadeRed;
float FadeGreen;
float FadeBlue;

//Coloration settings
bool FloodFill;
float FloodRed;
float FloodGreen;
float FloodBlue;
bool Sepia;
bool Mono;

//Sinewave
bool SineWave;
float SineAmplitude;          // A
float SineFrequency;          // omega
float SineTime;               // t
float SinePhaseX;             // theta
float SinePhaseY;			  //theta  (since cos(x) = sin(x + pi/2) this works for cosine too!)


// Helper for modifying the saturation of a color.
float4 AdjustSaturation(float4 color, float saturation)
{
    // The constants 0.3, 0.59, and 0.11 are chosen because the
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}


float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
	//Fiddle with Tex coords to create sine effect
	if(SineWave)
	{		
		texCoord.x += SineAmplitude * sin(texCoord.y*SineFrequency + SinePhaseX);
		texCoord.y += SineAmplitude * sin(texCoord.x*SineFrequency + SinePhaseY);
    }

    // Look up the bloom and original base image colors.
    float4 bloom = tex2D(BloomSampler, texCoord.xy);
    float4 base = tex2D(BaseSampler, texCoord.xy);
    
    // Adjust color saturation and intensity.
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    // Darken down the base image in areas where there is a lot of bloom,
    // to prevent things looking excessively burned-out.
    base *= (1 - saturate(bloom));                
    
    // Combine the two images.
    float4 color = base + bloom;
    
    if(FloodFill)
    {
		color.x = FloodRed;
		color.y = FloodGreen;
		color.z = FloodBlue;
    }
    
    if(Mono)
    {	    
		float mono = color*0.3;				    	     
		color.x = mono;
		color.y = mono;
		color.z = mono;  			
		color *=3;		
    }
    
    if(Sepia)
    {	    
		float mono = color*0.3;				    	     
		color.x = mono * 0.87;
		color.y = mono * 0.72;
		color.z = mono * 0.53;    			
		color *=3;		
    }
    
    //Now we have the base color after bloom, apply post fade
    if(PostFade)
    {
		if(!FadeTo)
			FadeLevel = 1.0 - FadeLevel;    	
		color.x -= (color.x - FadeRed)   * FadeLevel;
		color.y -= (color.y - FadeGreen)  * FadeLevel;
		color.z -= (color.z - FadeBlue) * FadeLevel;		
    }  
    
    return color;      
}


technique BloomCombine
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
