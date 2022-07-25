shader_type canvas_item;


uniform vec4 position = vec4(vec3(0.0), 0.0);

const float PI = 3.141592;

const vec2 RESOLUTION = vec2(1920, 1080);

uniform vec2 vmouse = vec2(1, 1);

varying float var_arr[3];

//void vertex()
//{
//	var_arr[0] = 1.0;
//	var_arr[1] = 0.0;
//}

mat3 CameraRotation ( vec2 m )
{
	m.y = -m.y;
	
	vec2 s = sin(m);
	vec2 c = cos(m);
	
	mat3 rotX = mat3(
		vec3( 1.0,  0.0,  0.0 ), 
		vec3( 0.0,  c.y,  s.y ), 
		vec3( 0.0, -s.y,  c.y )
	);
	
	mat3 rotY = mat3(
		vec3( c.x,  0.0, -s.x ), 
		vec3( 0.0,  1.0,  0.0 ), 
		vec3( s.x,  0.0,  c.x )
	);
	
	return rotX * rotY;
}

vec3 Grid (vec3 ro, vec3 rd)
{
	float d = -ro.y / rd.y;
	
	if (d <= 0.0) return vec3(0.4);
	
	vec2 p = (ro.xz + rd.xz * d);
	vec2 e = min(vec2(1.0), fwidth(p));
	vec2 l = (
		smoothstep(vec2(1.0), 1.0 - e, fract(p)) + 
		smoothstep(vec2(0.0), e, fract(p)) - (1.0 - e)
	);
	
	return mix(vec3(0.4), vec3(0.8) * (l.x + l.y) * 0.5, exp(-d * 0.01));
}

void Camera (in vec2 fragCoord, out vec3 ro, out vec3 rd)
{
	ro = position.xyz;
	vec2 m = vmouse.xy / RESOLUTION.x;
	
	float a = 1.0 / max(RESOLUTION.x, RESOLUTION.y);
	rd = normalize(vec3((fragCoord - RESOLUTION.xy * 0.5) * a, 0.5));
	
	rd = CameraRotation(m) * rd;
}

void vertex() {
	// VERTEX = (EXTRA_MATRIX * (WORLD_MATRIX * vec4(VERTEX, 0.0, 1.0))).xy;
}

void fragment() {
	vec3 ro = vec3(0.0);
	vec3 rd = vec3(0.0);
	
	Camera(FRAGCOORD.xy, ro, rd);
	vec3 color = Grid(ro, rd);
	
	COLOR = vec4(pow(color, vec3(0.4545)), 0.25);
}
