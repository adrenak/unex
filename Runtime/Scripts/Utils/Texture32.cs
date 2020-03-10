using System;
using UnityEngine;

namespace Adrenak.Unex {
	/// <summary>
	/// A class to hold texture data as Color32 internally.
	/// All operations are done over only the array and  
	/// </summary>
	public class Texture32 {
		public Color32[] Pixels { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		/// <summary>
		/// Creates an empty instance of the given width and height
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Texture32(int width, int height) {
			Pixels = new Color32[width * height];
			Width = width;
			Height = height;
		}

		/// <summary>
		/// Creates an instance from the pixels and the width and height they represent
		/// </summary>
		public Texture32(Color32[] pixels, int width, int height) {
			Pixels = pixels;
			Width = width;
			Height = height;
		}
		
		/// <summary>
		/// Creates a Texture32 isntance from a Texture2D object
		/// </summary>
		/// <param name="tex">The Texture2D object from which the instance should be created</param>
		/// <returns>The new Texture32 instance</returns>
		public static Texture32 FromTexture2D(Texture2D tex) {
			return new Texture32(tex.GetPixels32(), tex.width, tex.height);
		}

		/// <summary>
		/// Gets the pixels at the given coordinates
		/// </summary>
		public Color32 GetPixel(int x, int y) {
			return Pixels[Width * y + x];
		}

		/// <summary>
		/// Sets a value to the given pixel
		/// </summary>
		public void SetPixel(int x, int y, Color32 pixel) {
			Pixels[Width * y + x] = pixel;
		}

		/// <summary>
		/// Returns a block of pixels from the Texture2D
		/// </summary>
		/// <param name="x">The X coordinate from where the block starts (bottom left)</param>
		/// <param name="y">The Y coordinate from where the block starts (bottom left)</param>
		/// <param name="width">The width of the block returned</param>
		/// <param name="height">The height of the block returned</param>
		/// <returns></returns>
		public Texture32 GetBlock(int x, int y, int width, int height) {
			try {
				Color32[] bytes = new Color32[width * height];

				int index = 0;
				for (int j = y; j < y + height; j++) {
					for (int i = x; i < x + width; i++) {
						bytes[index] = GetPixel(i, j);
						index++;
					}
				}
				return new Texture32(bytes, width, height);
			}
			catch(Exception e) {
				Debug.LogError(e);
				return null;
			}
		}

		/// <summary>
		/// Crops the texture represented by the pixels
		/// </summary>
		/// <param name="x">The X coordinate from where the cropping starts (bottom left)</param>
		/// <param name="y">The Y coordinate from where the cropping starts (bottom left)</param>
		/// <param name="width">The width of the texture after cropping</param>
		/// <param name="height">The height of the texture after cropping</param>
		public void Crop(int x, int y, int width, int height) {
			try {
				var block = GetBlock(x, y, width, height);
				Pixels = block.Pixels;
				Width = block.Width;
				Height = block.Height;
			}
			catch(Exception e) {
				Debug.LogError(e);
			}
		}

		/// <summary>
		/// Replaces a block of pixels with the ones in the provided Texture32
		/// </summary>
		/// <param name="destX">The X coordinate from where the replacement should started</param>
		/// <param name="destY">The Y coordinate from where the replacement should started</param>
		/// <param name="tex">the Texture32 instance that is used to replace</param>
		public void ReplaceBlock(int destX, int destY, Texture32 tex) {
			ReplaceBlock(destX, destY, tex.Pixels, tex.Width, tex.Height);
		}

		/// <summary>
		/// Replaces a block of pixels with the onces provided
		/// </summary>
		/// <param name="destX">The X coordinate from where the replacement should started</param>
		/// <param name="destY">The Y coordinate from where the replacement should be started</param>
		/// <param name="pixels">The pixels with this the Texture32 pixels are to be replaced</param>
		/// <param name="sourceWidth">The width of the block</param>
		/// <param name="sourceHeight">The height of the block</param>
		public void ReplaceBlock(int destX, int destY, Color32[] pixels, int sourceWidth, int sourceHeight) {
			try {
				for(int i = 0; i < sourceWidth; i++) {
					for(int j = 0; j < sourceHeight; j++) {
						var pixel = pixels[j * sourceWidth + i];
						SetPixel(destX + i, destY + j, pixel);
					}
				}
			}
			catch(Exception e) {
				Debug.LogError(e);
			}
		}
		
		/// <summary>
		/// Creates and returns a Texture2D objects from the internal Color32 pixels
		/// </summary>
		/// <param name="format">The format of the texture to be created</param>
		/// <param name="isMipMapped">Whether the created texture has mip maps</param>
		/// <param name="isLinear">Whether the creates texture is linear</param>
		/// <returns></returns>
		public Texture2D GetTexture2D(TextureFormat format, bool isMipMapped = true, bool isLinear = true) {
			var result = new Texture2D(Width, Height, format, isMipMapped, isLinear);
			result.SetPixels32(Pixels);
			result.Apply();
			return result;
		}

		/// <summary>
		/// Clears the pixels and sets size to zero
		/// </summary>
		public void Clear() {
			Pixels = null;
			Width = 0;
			Height = 0;
		}
	}
}