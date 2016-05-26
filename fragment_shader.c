#version 450
const int raytraceDepth = 3; //Глубина следа луча
in vec4 color;
in vec3 org, dir;
out vec4 outputColor;

struct Ray
{
	vec3 org;
	vec3 dir;
};
struct Sphere
{
	vec3 c;
	float r;
	vec3 col;
};
struct Plane  //Плоскость
{
	vec3 p;
	vec3 n;
	vec3 col;
};

struct Intersection  //Пересечение
{
	float t;
	vec3 p;     // hit point
	vec3 n;     // normal
	int hit;
	vec3 col;
};


//Проверка пересечения луча со сферой 
//Если пересекаются то идет пересчет цвета
void shpere_intersect(Sphere s, Ray ray, inout Intersection isect)
{

	//µ = -(d . ∂p) ± √{ (d . ∂p)^2 - (|∂p|^2 - r^2)
	vec3 rs = ray.org - s.c;
	float B = dot(rs, ray.dir);  // (d . ∂p)
	float C = dot(rs, rs) - (s.r * s.r);  // |∂p|^2 - r^2
	float D = B * B - C;

	if (D > 0.0)
	{
		float t = -B - sqrt(D);   //Луч входа
		if ((t > 0.0) && (t < isect.t))
		{
			isect.t = t;
			isect.hit = 1;

			// calculate normal.
			vec3 p = vec3(ray.org.x + ray.dir.x * t,
				ray.org.y + ray.dir.y * t,
				ray.org.z + ray.dir.z * t);
			vec3 n = p - s.c;
			n = normalize(n);
			isect.n = n;
			isect.p = p;
			isect.col = s.col;
		}
	}
}

//Пересечение с плоскостью
void plane_intersect(Plane plan, Ray ray, inout Intersection isect)
{
	float d = -dot(plan.p, plan.n);
	float v = dot(ray.dir, plan.n);

	if (abs(v) < 1.0e-6)
		return; // the plane is parallel to the ray.

	float t = -(dot(ray.org, plan.n) + d) / v;

	if ((t > 0.0) && (t < isect.t))
	{
		isect.hit = 1;
		isect.t = t;
		isect.n = plan.n;

		vec3 p = vec3(ray.org.x + t * ray.dir.x,
			ray.org.y + t * ray.dir.y,
			ray.org.z + t * ray.dir.z);
		isect.p = p;
		float offset = 0.2;
		vec3 dp = p + offset;
		if (plan.n.z != 0 && ((mod(dp.x, 1.0) > 0.5 && mod(dp.y, 1.0) > 0.5)
			|| (mod(dp.x, 1.0) < 0.5 && mod(dp.y, 1.0) < 0.5)))
			isect.col = plan.col;
		else if (plan.n.z != 0)
			isect.col = plan.col * 0.5;
		else if (plan.n.x != 0 && ((mod(dp.y, 1.0)>0.5 && mod(dp.z, 1.0)>0.5)
			|| (mod(dp.y, 1.0)<0.5 && mod(dp.z, 1.0)<0.5)))
			isect.col = plan.col;
		else if (plan.n.x != 0)
			isect.col = plan.col*0.5;
		else if (plan.n.y != 0 && ((mod(dp.x, 1.0)>0.5 && mod(dp.z, 1.0)>0.5)
			|| (mod(dp.x, 1.0)<0.5 && mod(dp.z, 1.0)<0.5)))
			isect.col = plan.col;
		else if (plan.n.y != 0)
			isect.col = plan.col*0.5;
	}
}

Sphere sphere[3];
Plane plane[5];
void Intersect(Ray r, inout Intersection i)  //Ближайшее пересечение с плоскостью или сферой
{
	for (int c = 0; c < 3; c++)
	{
		shpere_intersect(sphere[c], r, i);

	}
	for (int c = 0; c<4; c++)
	{
		plane_intersect(plane[c], r, i);
	}
}

int seed = 0;
float random()
{
	seed = int(mod(float(seed)*1364.0 + 626.0, 509.0));
	return float(seed) / 509.0;
}
vec3 computeLightShadow(in Intersection isect)  //Вычисление тени объектов
{
	int i, j;
	int ntheta = 16;
	int nphi = 16;
	float eps = 0.0001;

	// Slightly move ray org towards ray dir to avoid numerical probrem.
	vec3 p = vec3(isect.p.x + eps * isect.n.x,
		isect.p.y + eps * isect.n.y,
		isect.p.z + eps * isect.n.z);

	vec3 lightPoint = vec3(5, 3, 5);
	Ray ray;
	ray.org = p;
	ray.dir = normalize(lightPoint - p);

	Intersection lisect;
	lisect.hit = 0;
	lisect.t = 1.0e+30;
	lisect.n = lisect.p = lisect.col = vec3(0, 0, 0);
	Intersect(ray, lisect);
	if (lisect.hit != 0)
		return vec3(0.0, 0.0, 0.0);
	else
	{
		float shade = max(0.0, dot(isect.n, ray.dir)*0.9);
		shade = pow(shade, 4.0) + shade * 0.5;
		return vec3(shade, 1.5*shade, shade);
	}

}

void main()
{
	sphere[0].c = vec3(-2.0, 0.0, -3.5); //Устанавливаем значения для сфер (радиус,центр,цвет)
	sphere[0].r = 0.5;
	sphere[0].col = vec3(0.4, 0.9, 0.1);
	sphere[1].c = vec3(0.0, 0.0, -3.0);
	sphere[1].r = 0.5;
	sphere[1].col = vec3(0.9, 0.5, 0.1);
	sphere[2].c = vec3(1.0, 0.0, -2.2);
	sphere[2].r = 0.5;
	sphere[2].col = vec3(0.7, 0.7, 0.7);
	plane[0].p = vec3(0, -0.5, 0);
	plane[0].n = vec3(0, 1.0, 0);
	plane[0].col = vec3(0.5, 1, 1);
	plane[1].p = vec3(0.0, -0.5, -6.0);
	plane[1].n = vec3(0.0, 0.0, 1.0);
	plane[1].col = vec3(0, 0.5, 1);
	plane[2].p = vec3(-3.0, -0.5, 0);
	plane[2].n = vec3(1.0, 0, 0);
	plane[2].col = vec3(1, 0.1, 0.1);
	plane[3].p = vec3(8.0, -0.5, 0);
	plane[3].n = vec3(-1.0, 0, 1.0);
	plane[3].col = vec3(1, 0.1, 0.1);
	plane[4].p = vec3(0, 2.0, 0);
	plane[4].n = vec3(0.0, 2.0, 0);
	plane[4].col = vec3(0.9, 0.9, 0.9);
	seed = int(mod(dir.x * dir.y * 4525434.0, 65536.0));

	Ray r;
	r.org = org;
	r.dir = normalize(dir);
	vec4 col = vec4(0, 0, 0, 1);
	float eps = 0.0001;
	vec3 bcol = vec3(1, 1, 1);
	for (int j = 0; j < raytraceDepth; j++)
	{
		Intersection i;
		i.hit = 0;
		i.t = 1.0e+30;
		i.n = i.p = i.col = vec3(0, 0, 0);

		Intersect(r, i);
		if (i.hit != 0)
		{
			col.rgb += bcol * i.col * computeLightShadow(i);
			bcol *= i.col;
		}
		else
		{
			break;
		}

		r.org = vec3(i.p.x + eps * i.n.x,
			i.p.y + eps * i.n.y,
			i.p.z + eps * i.n.z);
		r.dir = reflect(r.dir, vec3(i.n.x, i.n.y, i.n.z));
	}
	outputColor = col;
}
