using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleApp1
{
	class RayTracing
	{
		struct Vector3f
		{
			public Vector3f(float x, float y, float z)
			{
				X = x;
				Y = y;
				Z = z;
			}

			public Vector3f(double x, double y, double z)
			{
				X = (float)x;
				Y = (float)y;
				Z = (float)z;
			}

			public float X, Y, Z;
		}

		class Ray
		{
			public Ray(Vector3f origin, Vector3f direction)
			{
				Origin = origin;
				Direction = direction;
				InvDirection = new Vector3f(1f / Direction.X, 1f / Direction.Y, 1f / Direction.Z);

				sign = new int[3];
				sign[0] = (InvDirection.X < 0) ? 1 : 0;
				sign[1] = (InvDirection.Y < 0) ? 1 : 0;
				sign[2] = (InvDirection.Z < 0) ? 1 : 0;
			}

			public Vector3f Origin;

			// two ways to point the ray - at another coord, or with angles
			public Vector3f Direction;
			public Vector3f InvDirection;

			public float? Azimuth; // [0, 2pi)
			public float? Zenith;  // [0, pi)

			public int[] sign;

			//public bool Intersects(IGeometry geo)
			//{

			//}
		}

		struct Camera
		{
			public Camera(Vector3f pos, Vector3f dir)
			{
				Position = pos;
				Direction = dir;
			}

			public Vector3f Position;
			public Vector3f Direction;
		}

		interface IGeometry
		{
			Vector3f GetPosition();
			void SetPosition(Vector3f v);

			//bool Intersects(Ray r);
			bool Intersects(Ray r, out Vector3f[] points);
		}

		class Sphere : IGeometry
		{
			public Sphere(Vector3f position, float radius)
			{
				mPosition = position;
				mRadius = radius;
			}

			Vector3f mPosition;
			float mRadius;

			public Vector3f GetPosition() { return mPosition; }
			public void SetPosition(Vector3f pos) { mPosition = pos; }


			public bool Intersects(Ray r, out Vector3f[] points)
			{
				double cx = mPosition.X;
				double cy = mPosition.Y;
				double cz = mPosition.Z;

				double px = r.Origin.X;
				double py = r.Origin.Y;
				double pz = r.Origin.Z;

				double vx = (r.Origin.X + r.Direction.X * 1000) - px;
				double vy = (r.Origin.Y + r.Direction.Y * 1000) - py;
				double vz = (r.Origin.Z + r.Direction.Z * 1000) - pz;

				double A = vx * vx + vy * vy + vz * vz;
				double B = 2.0 * (px * vx + py * vy + pz * vz - vx * cx - vy * cy - vz * cz);
				double C = px * px - 2 * px * cx + cx * cx + py * py - 2 * py * cy + cy * cy +
						   pz * pz - 2 * pz * cz + cz * cz - mRadius * mRadius;

				// discriminant
				double D = B * B - 4 * A * C;

				if (D < 0)
				{
					points = new Vector3f[0];
					return false;
				}

				double t1 = (-B - Math.Sqrt(D)) / (2.0 * A);

				Vector3f solution1 = new Vector3f(px * (1 - t1) + t1 * vx,
												 py * (1 - t1) + t1 * vy,
												 pz * (1 - t1) + t1 * vz);
				if (D == 0)
				{
					points = new Vector3f[] { solution1 };
					return true;
				}

				double t2 = (-B + Math.Sqrt(D)) / (2.0 * A);
				Vector3f solution2 = new Vector3f(px * (1 - t2) + t2 * vx,
												 py * (1 - t2) + t2 * vy,
												 pz * (1 - t2) + t2 * vz);

				// prefer a solution that's on the line segment itself

				if (Math.Abs(t1 - 0.5) < Math.Abs(t2 - 0.5))
				{
					points = new Vector3f[] { solution1, solution2 };
					return true;
				}

				points = new Vector3f[] { solution2, solution1 };
				return true;
			}
		}

		class RectangularPrism //: IGeometry
		{
			public RectangularPrism(Vector3f position, Vector3f m1, Vector3f m2)
			{
				min = m1;
				max = m2;
			}

			// Vertices
			Vector3f min, max;
			Vector3f[] bounds; // = new Vector3f[2] { min, max };

			// Optimized method
			public bool Intersects(Ray r) //, float t0, float t1)
			{
				bounds = new Vector3f[2] { min, max };
				float txmin, txmax, tymin, tymax, tzmin, tzmax;

				txmin = (bounds[r.sign[0]].X - r.Origin.X) * r.InvDirection.X;
				txmax = (bounds[1 - r.sign[0]].X - r.Origin.X) * r.InvDirection.X;
				tymin = (bounds[r.sign[1]].Y - r.Origin.Y) * r.InvDirection.Y;
				tymax = (bounds[1 - r.sign[1]].Y - r.Origin.Y) * r.InvDirection.Y;

				//t0 = 0;

				if ((txmin > tymax) || (tymin > txmax))
					return false;
				if (tymin > txmin)
					txmin = tymin;
				if (tymax < txmax)
					txmax = tymax;

				tzmin = (bounds[r.sign[2]].Z - r.Origin.Z) * r.InvDirection.Z;
				tzmax = (bounds[1 - r.sign[2]].Z - r.Origin.Z) * r.InvDirection.Z;

				if ((txmin > tzmax) || (tzmin > txmax))
					return false;
				if (tzmin > txmin)
					txmin = tzmin;
				if (tzmax < txmax)
					txmax = tzmax;

				// tMin is the closest point of intersection, aka distance to the object
				//t0 = txmin;
				return true; // ((txmin < t1) && (txmax > t0));
			}

			public Vector3f GetPosition() { return mPosition; }
			public void SetPosition(Vector3f pos) { mPosition = pos; }

			Vector3f mPosition;
		}

		public Bitmap Trace()
		{
			// single pass (no bounce) raytrace
			Camera cam = new Camera(new Vector3f(0, 0, 0), new Vector3f(0, 0, -10));

			RectangularPrism rp = new RectangularPrism(
				new Vector3f(0, 0, -100),
				new Vector3f(-10, -10, -10),
				new Vector3f(10, 10, 10));

			Sphere sp = new Sphere(new Vector3f(0, 0, -20), 5f);

			List<IGeometry> sceneObjects = new List<IGeometry>();
			sceneObjects.Add(sp);

			Bitmap im = new Bitmap(256, 256);

			// for each pixel
			for (int y = 0; y < 256; ++y)
			{
				for (int x = 0; x < 256; ++x)
				{
					Vector3f rayOrigin = new Vector3f(cam.Position.X + x - 128, cam.Position.Y - 128, cam.Position.Z);
					Ray r = new Ray(rayOrigin, cam.Direction);

					// sort objects on depth here? then we don't need to iterate over them all

					im.SetPixel(x, y, Color.Black);

					// get first object the ray intersects with
					//IGeometry closestObject;
					foreach (IGeometry geom in sceneObjects)
					{
						Vector3f[] results;
						if (geom.Intersects(r, out results))
						{
							im.SetPixel(x, y, Color.White);
							//if (o.distance < closestObject.distance)
							//	closestObject = o;

						}
					}

					// by now we have the object the ray hits
					//foreach (light in scene)
					//{
					//	p.color += closestObject.color * lightvalue * Normal to obj and light
					//}

				}
			}

			return im;
		}


		public static void RTMain(string[] args)
		{
			RayTracing rt = new RayTracing();
			Bitmap bmp = rt.Trace();
			bmp.Save("raytrace_test.png", ImageFormat.Png);
		}
	}
}
