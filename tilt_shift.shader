shader_type canvas_item;

uniform float limit: hint_range(0.0, 0.5) = 0.2;
uniform float blur: hint_range(0.0, 5.0) = 2.0;
uniform float intensity: hint_range (0.0, 0.5) = 0.19;
uniform bool debug = true;


void fragment ()
{
	
 	if (UV.y < limit)
	{
		float _step = smoothstep(UV.y, limit, intensity);
		vec4 color = textureLod(SCREEN_TEXTURE, SCREEN_UV, blur);
		COLOR = color;
		
		if (debug == true)
		{
			COLOR = vec4(1.0, 1.0, 1.0, 1.0);
		}
			
		COLOR.a = _step;
	} 
	
	else if (UV.y > 1.0 - limit)
	{		
		float _step = smoothstep(UV.y, 1.0 - limit, 1.0 - intensity);
		vec4 color = textureLod(SCREEN_TEXTURE, SCREEN_UV, blur);
		COLOR = color;
		
		if (debug == true)
		{
			COLOR = vec4(1.0, 1.0, 1.0, 1.0);
		}
		
		COLOR.a = _step;		
	}
	
	else
	{
		COLOR.a = 0f; 
	}
}