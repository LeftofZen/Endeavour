using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Priority_Queue;

namespace ConsoleApp1
{
	class MyForm : Form
	{
		public MyForm() : base()
		{
			this.SetStyle(ControlStyles.ResizeRedraw, true);
		}
	}

	class ArtGallery
	{
		class Line2D
		{
			public Line2D(Point a, Point b)
			{ A = a; B = b; }

			public Point A { get; set; }
			public Point B { get; set; }
		}

		static List<Line2D> Connect(params Point[] rest)
		{
			if (rest.Length < 2)
			{
				return new List<Line2D>();
			}

			var lines = new List<Line2D>(rest.Length - 1);
			for (var i = 0; i < rest.Length - 1; ++i)
			{
				lines.Add(new Line2D(rest[i], rest[i + 1]));
			}

			return lines;
		}

		public ArtGallery()
		{
			mGalleryGeom = Connect(
				new Point(100, 0),
				new Point(200, 0),
				new Point(200, 100),
				new Point(250, 100),
				new Point(250, 50),
				new Point(300, 50),
				//new Point(300, 100),
				new Point(300, 200),
				new Point(200, 200),
				new Point(200, 300),
				new Point(100, 300),
				new Point(100, 200),
				new Point(0, 200),
				new Point(0, 100),
				new Point(100, 100),
				new Point(100, 0)
				);

			mPlayer = new Point(150, 150);

			mForm = new MyForm();
			mForm.BackColor = Color.White;
			mForm.FormBorderStyle = FormBorderStyle.Sizable;
			mForm.Bounds = new Rectangle(100, 100, 512, 512);//Screen.PrimaryScreen.Bounds;
			mForm.TopMost = true;
			Application.EnableVisualStyles();

			mGraphics = mForm.CreateGraphics();
			//mGraphics.TranslateTransform(10, 10);

			mForm.Paint += new PaintEventHandler(this.Draw);
			mForm.MouseMove += new MouseEventHandler(this.MouseMove);

		}

		public void Draw(object sender, PaintEventArgs e)
		{
			// setup
			var b1 = Brushes.White;
			var p1 = new Pen(b1);
			var p2 = new Pen(Color.Red);
			var p3 = new Pen(Color.Orange);
			var p4 = new Pen(Color.Green);

			mGraphics.Clear(Color.CornflowerBlue);

			var intersections = new List<Line2D>();

			// draw gallery
			foreach (var l in mGalleryGeom)
			{
				if (LinesIntersect(l, new Line2D(mPlayer, mMouse)))
				{
					intersections.Add(l);
				}

				mGraphics.DrawLine(p1, l.A, l.B);
			}

			intersections.Sort((lineA, lineB) =>
			{
				var distA = DistancePointToLine(lineA, mPlayer);
				var distB = DistancePointToLine(lineB, mPlayer);

				return distA.CompareTo(distB);
			});


			var first = true;
			foreach (var l in intersections)
			{
				if (first)
				{
					mGraphics.DrawLine(p2, l.A, l.B);
					first = false;
				}
				else
				{
					mGraphics.DrawLine(p3, l.A, l.B);
				}
			}


			// player
			const int playerSize = 20;
			mGraphics.FillEllipse(
				b1,
				mPlayer.X - playerSize / 2,
				mPlayer.Y - playerSize / 2,
				playerSize,
				playerSize);

			// mouse

			foreach (var l in intersections)
			{
				if (first)
				{
					mGraphics.DrawLine(p2, l.A, l.B);
					first = false;
				}
				else
				{
					mGraphics.DrawLine(p3, l.A, l.B);
				}
			}

			//mGraphics.DrawLine(p1, mPlayer, mMouse);
		}

		public void MouseMove(object sender, MouseEventArgs e)
		{
			mMouse = e.Location;
			mForm.Refresh();
		}

		public int ccw(Point a, Point b, Point c)
		{
			return (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
		}

		bool LinesIntersect(Line2D l1, Line2D l2)
		{
			if (ccw(l1.A, l1.B, l2.A) * ccw(l1.A, l1.B, l2.B) > 0)
			{
				return false;
			}

			if (ccw(l2.A, l2.B, l1.A) * ccw(l2.A, l2.B, l1.B) > 0)
			{
				return false;
			}

			return true;
		}

		Point ClosestPointToLine(Line2D line, Point p0)
		{
			var a = line.B.X - line.A.X;
			var b = line.B.Y - line.A.Y;
			var t = (a * (p0.X - line.A.X) + b * (p0.Y - line.A.Y)) / (a * a + b * b);

			if (t > 1)
			{
				return line.B;
			}
			else if (t < 0)
			{
				return line.A;
			}
			else
			{
				return new Point((line.A.X + t * a), (line.A.Y + t * b));
			}
		}

		int DistancePointToLine(Line2D line, Point p0)
		{
			var point = ClosestPointToLine(line, p0);
			return (int)Math.Sqrt(Math.Pow((p0.X - point.X), 2) + Math.Pow((p0.Y - point.Y), 2));
		}

		public void Run()
		{
			Application.Run(mForm);
		}

		// Window drawing
		MyForm mForm;

		Graphics mGraphics;
		Point mPlayer;
		Point mMouse;

		//Bitmap mImg;
		List<Line2D> mGalleryGeom;
	}

}
