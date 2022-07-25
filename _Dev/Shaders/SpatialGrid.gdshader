shader_type spatial;
render_mode unshaded;

uniform float scale = 5.0;

float saturate(float val)
{
	return clamp(val, 0.0, 1.0);
}

void vertex ()
{
	
}

void fragment ()
{
	// in		vec2	VIEWPORT_SIZE
	// in		vec2	UV, UV2
	// in		vec2	POINT_COORD
	// in		vec3	VERTEX
	// in		vec3	VIEW
	// in		vec4 	COLOR
	// in		mat4	CAMERA_MATRIX
	// in		mat4	PROJECTION_MATRIX
	// in		mat4	INV_PROJECTION_MATRIX
	// in		mat4	WORLD_MATRIX
	// in		mat4	INV_WORLD_MATRIX
	
	// 	 out	vec3	ALBEDO
	// 	 out	vec3	EMISSION	
	//   out	float	ALPHA
	
	vec2 uv = POINT_COORD / VIEWPORT_SIZE.xy;
	vec2 tiled = fract(uv * scale);
	vec2 square = abs(tiled * 2.0 - 1.0);
	vec2 sharpSquare = step(0.8, square);
	float result = sharpSquare.x + sharpSquare.y;
	
	EMISSION = vec3(1.0) * result;
	
	float gridAlpha = 1.0 + (
		saturate(min(sharpSquare.x, sharpSquare.y)) *
		saturate(min(sharpSquare.x, sharpSquare.y))
	);
	
	ALPHA = gridAlpha;
}
