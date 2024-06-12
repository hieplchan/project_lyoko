using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using CW.Common;

namespace Lean.Texture
{
	/// <summary>This allows you to convert the texture into a normal map based on height data extracted from the RGBA values in the specified way.</summary>
	[System.Serializable]
	public class LeanFilterMetallic : LeanFilter
	{
		public override string Title
		{
			get
			{
				return "Convert To Metallic";
			}
		}

		public enum DistanceType
		{
			MaxChannel,
			Average
		}

		/// <summary>The color of the most metallic part of the source texture.</summary>
		public Color TargetColor { set { targetColor = value; } get { return targetColor; } } [SerializeField] private Color targetColor = Color.white;

		/// <summary>The metallic value given to areas that don't match the <b>TargetColor</b>.</summary>
		public float DefaultMetallic { set { defaultMetallic = value; } get { return defaultMetallic; } } [SerializeField] [Range(0.0f, 1.0f)] private float defaultMetallic = 0.2f;

		/// <summary>The metallic value given to areas that match the <b>TargetColor</b>.</summary>
		public float TargetMetallic { set { targetMetallic = value; } get { return targetMetallic; } } [SerializeField] [Range(0.0f, 1.0f)] private float targetMetallic = 0.5f;

		/// <summary>The calculation to find the difference between the current pixel color and the <b>OldColor</b>.
		/// MaxChannel = The maximum difference between any of the color channels.
		/// Average = The average of all channel differences.</summary>
		public DistanceType Distance { set { distance = value; } get { return distance; } } [SerializeField] private DistanceType distance;

		/// <summary>The <b>OldColor</b> can be off by this much.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [SerializeField] [Range(0.0f, 1.0f)] private float threshold = 0.01f;

		/// <summary>How much should colors outside the <b>Threshold</b> be replaced?</summary>
		public float Edge { set { edge = value; } get { return edge; } } [SerializeField] [Range(0.0f, 1.0f)] private float edge = 0.1f;

		[BurstCompile]
		struct FilterJob : IJobParallelFor
		{
			public NativeArray<float4> INOUT;

			[ReadOnly] public float3       TargetColor;
			[ReadOnly] public float        DefaultMetallic;
			[ReadOnly] public float        TargetMetallic;
			[ReadOnly] public DistanceType Distance;
			[ReadOnly] public float        Threshold;
			[ReadOnly] public float        Edge;

			public float GetDistance(float4 pixel)
			{
				var delta = math.abs(pixel.xyz - TargetColor);

				if (Distance == DistanceType.MaxChannel)
				{
					return math.max(delta.x, math.max(delta.y, delta.z));
				}
				else// if (Distance == DistanceType.Average)
				{
					return (delta.x + delta.y + delta.z) / 3.0f;
				}
			}

			public void Execute(int index)
			{
				var pixel    = INOUT[index];
				var dist     = GetDistance(pixel);
				var weight   = 1.0f - math.saturate((dist - Threshold) / Edge);
				var metallic = math.lerp(DefaultMetallic, TargetMetallic, weight);

				pixel.xyz = metallic;

				INOUT[index] = pixel;
			}
		}

		public override void Schedule(LeanPendingTexture data)
		{
			var filter = new FilterJob();

			filter.INOUT           = data.Pixels;
			filter.TargetColor     = (Vector3)(Vector4)targetColor;
			filter.DefaultMetallic = defaultMetallic;
			filter.TargetMetallic  = TargetMetallic;
			filter.Threshold       = threshold;
			filter.Edge            = math.max(0.0001f, edge);
			filter.Distance        = distance;

			data.Handle = filter.Schedule(data.Pixels.Length, 32, data.Handle);
		}

#if UNITY_EDITOR
		protected override void DrawInspector()
		{
			CwEditor.Draw("targetColor", "The color of the most metallic part of the source texture.");
			CwEditor.Draw("defaultMetallic", "The metallic value given to areas that don't match the <b>TargetColor</b>.");
			CwEditor.Draw("targetMetallic", "The metallic value given to areas that match the <b>TargetColor</b>.");
			CwEditor.Draw("distance", "The calculation to find the difference between the current pixel color and the <b>OldColor</b>.\n\nMaxChannel = The maximum difference between any of the color channels.\n\nAverage = The average of all channel differences.");
			CwEditor.Draw("threshold", "The <b>OldColor</b> can be off by this much.");
			CwEditor.Draw("edge", "How much should colors outside the <b>Threshold</b> be replaced?");
		}
#endif
	}
}