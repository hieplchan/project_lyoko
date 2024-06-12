using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using CW.Common;

namespace Lean.Texture
{
	/// <summary>This allows you to combine another texture into the current texture.</summary>
	[System.Serializable]
	public class LeanFilterCombine : LeanFilter
	{
		public override string Title
		{
			get
			{
				return "Combine With Texture";
			}
		}

		public enum BlendMode
		{
			Crossfade,
			AlphaBlend
		}

		/// <summary>The texture that will be combined with the current texture.</summary>
		public UnityEngine.Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private UnityEngine.Texture texture;

		/// <summary>The technique used to combine the textures.
		/// Crossfade = The RGBA channels will be linearly interpolated separately.
		/// AlphaBlend = The RGBA channels will be blended based on the A channel of each texture.</summary>
		public BlendMode Blend { set { blend = value; } get { return blend; } } [SerializeField] private BlendMode blend;

		/// <summary>The opacity of the texture combination.</summary>
		public float Opacity { set { opacity = value; } get { return opacity; } } [SerializeField] [Range(0.0f, 1.0f)] private float opacity = 1.0f;

		[BurstCompile]
		struct FilterJob : IJobParallelFor
		{
			public NativeArray<float4> INOUT;

			[ReadOnly] public BlendMode           Blend;
			[ReadOnly] public int2                Size;
			[ReadOnly] public float               Opacity;
			[ReadOnly] public NativeArray<float4> OtherData;
			[ReadOnly] public int2                OtherSize;

			public void Execute(int index)
			{
				var pixel = INOUT[index];
				var x     = index % Size.x;
				var y     = index / Size.x;
				var uv    = new float2(x, y) / (Size - 1);
				var other = LeanSample.Tex2D_Linear(OtherData, OtherSize, uv);

				switch (Blend)
				{
					case BlendMode.Crossfade:
					{
						pixel = math.lerp(pixel, other, Opacity);
					}
					break;

					case BlendMode.AlphaBlend:
					{
						other.w *= Opacity;
						var str = 1.0f - other.w;
						var div = other.w + pixel.w * str;

						if (div > 0.0f)
						{
							pixel.xyz = (other.xyz * other.w + pixel.xyz * pixel.w * str) / div;
							pixel.w   = div;
						}
					}
					break;
				}

				INOUT[index] = pixel;
			}
		}

		public override void Schedule(LeanPendingTexture data)
		{
			if (texture != null)
			{
				var filter = new FilterJob();

				filter.INOUT     = data.Pixels;
				filter.Blend     = blend;
				filter.Size      = data.Size;
				filter.Opacity   = opacity;
				filter.OtherData = LeanSample.GetPixels(texture, Allocator.TempJob, data.Linear, ref filter.OtherSize); data.Register(filter.OtherData);

				data.Handle = filter.Schedule(data.Pixels.Length, 32, data.Handle);
			}
		}

#if UNITY_EDITOR
		protected override void DrawInspector()
		{
			CwEditor.BeginError(ObjectExists("texture") == false);
				CwEditor.Draw("texture", "The texture that will be combined with the current texture.");
			CwEditor.EndError();
			CwEditor.Draw("blend", "The technique used to combine the textures.\n\nCrossfade = The RGBA channels will be linearly interpolated separately.\n\nAlphaBlend = The RGBA channels will be blended based on the A channel of each texture.");
			CwEditor.Draw("opacity", "The opacity of the texture combination.");
		}
#endif
	}
}