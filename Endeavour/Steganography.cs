using System;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleApp1
{
	class Steganography
	{
		public Steganography()
		{ }

		public void Encode(Bitmap im1, Bitmap im2, Bitmap imOut, uint mask)
		{
			for (var y = 0; y < im1.Height; ++y)
			{
				for (var x = 0; x < im1.Width; ++x)
				{
					var c1 = im1.GetPixel(x, y);
					var c2 = im2.GetPixel(x, y);

					var reverseMask = ~mask;

					var c3 = ((uint)c1.ToArgb() & mask);
					var c4 = ((uint)c2.ToArgb() & reverseMask);
					var c5 = c3 + c4;

					//Console.WriteLine("c3={0:X} c4={1:X} c5={2:X}", c3, c4, c5);
					//return;
					var c6 = Color.FromArgb((int)c5);
					imOut.SetPixel(x, y, c6);
				}
			}

			imOut.Save("imEncoded.png");
		}

		public void Decode(Bitmap imEncoded, Bitmap imDecoded, uint mask)
		{
			for (var y = 0; y < imEncoded.Height; ++y)
			{
				for (var x = 0; x < imEncoded.Width; ++x)
				{
					var c1 = imEncoded.GetPixel(x, y);

					var reverseMask = ~mask;
					var c2 = ((uint)c1.ToArgb() & reverseMask);
					var c3 = Color.FromArgb((int)c2);
					imDecoded.SetPixel(x, y, c3);
				}
			}

			imDecoded.Save("imDecoded.png");
		}

		public bool ImagesAreEqual(Bitmap im1, Bitmap im2)
		{
			for (var y = 0; y < im1.Height; ++y)
			{
				for (var x = 0; x < im1.Width; ++x)
				{
					var c1 = im1.GetPixel(x, y);
					var c2 = im2.GetPixel(x, y);

					if (c1 != c2)
					{
						return false;
					}
				}
			}

			return true;
		}

		public void Run()
		{
			// encode im2 into im1
			var im1 = new Bitmap("im1.png");
			var im2 = new Bitmap("im2.png");
			var imEncoded = new Bitmap(im1.Width, im1.Height);
			var imDecoded = new Bitmap(im1.Width, im1.Height);

			// lower two bits used for steganography
			const uint mask = 0xFCFCFCFC;

			Encode(im1, im2, imEncoded, mask);
			Decode(imEncoded, imDecoded, mask);

			Console.WriteLine("Images match: {0}", ImagesAreEqual(im2, imDecoded));
			_ = Console.ReadLine();
		}

	}
}
