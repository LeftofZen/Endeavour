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
			for (int y = 0; y < im1.Height; ++y)
			{
				for (int x = 0; x < im1.Width; ++x)
				{
					Color c1 = im1.GetPixel(x, y);
					Color c2 = im2.GetPixel(x, y);

					uint reverseMask = ~mask;

					uint c3 = ((uint)c1.ToArgb() & mask);
					uint c4 = ((uint)c2.ToArgb() & reverseMask);
					uint c5 = c3 + c4;

					//Console.WriteLine("c3={0:X} c4={1:X} c5={2:X}", c3, c4, c5);
					//return;
					Color c6 = Color.FromArgb((int)c5);
					imOut.SetPixel(x, y, c6);
				}
			}

			imOut.Save("imEncoded.png");
		}

		public void Decode(Bitmap imEncoded, Bitmap imDecoded, uint mask)
		{
			for (int y = 0; y < imEncoded.Height; ++y)
			{
				for (int x = 0; x < imEncoded.Width; ++x)
				{
					Color c1 = imEncoded.GetPixel(x, y);

					uint reverseMask = ~mask;
					uint c2 = ((uint)c1.ToArgb() & reverseMask);
					Color c3 = Color.FromArgb((int)c2);
					imDecoded.SetPixel(x, y, c3);
				}
			}

			imDecoded.Save("imDecoded.png");
		}

		public bool ImagesAreEqual(Bitmap im1, Bitmap im2)
		{
			for (int y = 0; y < im1.Height; ++y)
			{
				for (int x = 0; x < im1.Width; ++x)
				{
					Color c1 = im1.GetPixel(x, y);
					Color c2 = im2.GetPixel(x, y);

					if (c1 != c2)
						return false;
				}
			}

			return true;
		}

		public void Run()
		{
			// encode im2 into im1
			Bitmap im1 = new Bitmap("im1.png");
			Bitmap im2 = new Bitmap("im2.png");
			Bitmap imEncoded = new Bitmap(im1.Width, im1.Height);
			Bitmap imDecoded = new Bitmap(im1.Width, im1.Height);

			// lower two bits used for steganography
			uint mask = 0xFCFCFCFC;

			Encode(im1, im2, imEncoded, mask);
			Decode(imEncoded, imDecoded, mask);

			Console.WriteLine("Images match: {0}", ImagesAreEqual(im2, imDecoded));
			Console.ReadLine();
		}

	}
}
