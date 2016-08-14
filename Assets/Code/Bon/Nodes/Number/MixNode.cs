﻿using System;
using UnityEngine;

namespace Assets.Code.Bon.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Mix")]
	public class MixNode : AbstractNumberNode
	{
		[NonSerialized] private Rect labelInput01;
		[NonSerialized] private Rect labelInput02;
		[NonSerialized] private Rect labelFactor;

		[NonSerialized] private Socket _inputSocket01;
		[NonSerialized] private Socket _inputSocket02;
		[NonSerialized] private Socket _factorSocket;

		public MixNode(int id, Graph parent) : base(id, parent)
		{
			labelInput01 = new Rect(3, 0, 100, 20);
			labelInput02 = new Rect(3, 20, 100, 20);
			labelFactor = new Rect(3, 40, 100, 20);

			_inputSocket01 = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_inputSocket02 = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			_factorSocket = new Socket(this, typeof(AbstractNumberNode), SocketDirection.Input);
			Sockets.Add(_inputSocket01);
			Sockets.Add(_inputSocket02);
			Sockets.Add(_factorSocket);
			Height = 80;
			Width = 80;
		}

		public override void OnGUI()
		{
			GUI.Label(labelInput01, "in 1");
			GUI.Label(labelInput02, "in 2");
			GUI.Label(labelFactor, "factor (0 - 1)");
		}

		public override object GetResultOf(Socket outSocket)
		{
			return GetSampleAt(_x, _y, _seed);
		}


		public static float Clamp(float value, float min, float max)
		{
			return value < min ? min : value > max ? max : value;
		}

		public override float GetSampleAt(float x, float y, float seed)
		{
			var factorValue = GetInputNumber(_factorSocket, x, y, seed);
			if (float.IsNaN(factorValue)) return float.NaN;

			// to avoid calc of obsolete values here..
			if (factorValue <= 0) return GetInputNumber(_inputSocket01, x, y, seed);
			if (factorValue >= 1) return GetInputNumber(_inputSocket02, x, y, seed);

			float v1 = GetInputNumber(_inputSocket01, x, y, seed);
			float v2 = GetInputNumber(_inputSocket02, x, y, seed);

			v1 = Clamp(v1, 0, 1);
			v2 = Clamp(v2, 0, 1);

			if (float.IsNaN(v1) || float.IsNaN(v2)) return float.NaN;

			return v1 * (1 - factorValue) + v2 * factorValue;
		}
	}
}
