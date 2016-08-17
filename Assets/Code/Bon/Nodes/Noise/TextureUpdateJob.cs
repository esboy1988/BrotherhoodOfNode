﻿using System;
using System.Collections.Generic;
using Assets.Code.Bon.Interface;
using Assets.Code.Bon.Nodes.Vector3;
using Assets.Code.Thread;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Noise
{
	public class TextureUpdateJob : ThreadedJob
	{

		private ISampler3D _sampler3D;
		private IColorSampler1D _samplerColor;
		private List<UnityEngine.Vector3> _positions;

		private int _width;
		private int _height;
		private float[,] _values;

		public Texture2D Texture;

		public void Request(int width, int height, ISampler3D sampler, IColorSampler1D colorSampler = null)
		{
			_sampler3D = sampler;
			_samplerColor = colorSampler;
			_values = new float[width, height];
			Init(width, height);
		}

		public void Request(int width, int height, List<UnityEngine.Vector3> positions)
		{
			_positions = positions;
			_samplerColor = new Vector3DisplayColorSampler();
			Init(width, height);
		}

		private void Init(int width, int height)
		{
			_width = width;
			_height = height;
		}

		protected override void ThreadFunction()
		{
			if (_sampler3D == null) return;
			var _xStart = 0;
			var _yStart = 0;
			for (var x = _xStart; x < _xStart + _width; x++)
			{
				for (var y = _yStart; y < _yStart + _height; y++)
				{
					_values[x - _xStart, y - _yStart] = _sampler3D.GetSampleAt(x, y, 0);
				}
			}
		}

		protected override void OnFinished()
		{
			if (Texture != null) Texture2D.DestroyImmediate(Texture);
			Texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);

			if (_sampler3D != null) Texture.SetPixels(NodeUtils.ToColorMap(_values, _samplerColor));
			else if (_positions != null)
			{
				Texture.SetPixels(new UnityEngine.Color[_width * _height]);
				foreach (var position in _positions)
				{
					Texture.SetPixel((int) position.x, (int) position.y, UnityEngine.Color.white);
				}
			}
			Texture.Apply();
		}

	}
}
