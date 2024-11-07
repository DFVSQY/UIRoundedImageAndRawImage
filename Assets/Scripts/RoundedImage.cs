using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Rounded Image", 12)]
public class RoundedImage : Image
{
	/// <summary>
	/// 设置边角的模式
	/// </summary>
	[Serializable]
	public enum SetMode
	{
		/* 统一设置 */
		United,

		/* 分别设置 */
		Separate,
	}

	/// <summary>
	/// 矩形的角
	/// </summary>
	[Serializable]
	public class CornerProperties
	{
		/* 圆半径 */
		public float radius = 20f;

		/* 1/4圆的段数 */
		public int segments = 20;
	}

	public SetMode setMode = SetMode.United;

	public CornerProperties topLeft = new CornerProperties();
	public CornerProperties topRight = new CornerProperties();
	public CornerProperties bottomLeft = new CornerProperties();
	public CornerProperties bottomRight = new CornerProperties();

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();

		if (type != Type.Simple)
		{
			base.OnPopulateMesh(vh);
			return;
		}

		// 仅支持最简单的Simple类型的网格生成
		GenerateRoundedRectMesh(vh);
	}

	/// <summary>
	/// 圆润角的矩形网格
	/// </summary>
	private void GenerateRoundedRectMesh(VertexHelper vh)
	{
		Rect r = GetPixelAdjustedRect();
		Vector4 v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
		Color32 color32 = color;

		// 确保所有圆角半径不超过矩形的一半
		float minSize = Mathf.Min(r.width, r.height) / 2;
		ClampRadius(topLeft, minSize);
		ClampRadius(topRight, minSize);
		ClampRadius(bottomLeft, minSize);
		ClampRadius(bottomRight, minSize);

		// Vector4 uv = new Vector4(0f, 0f, 1f, 1f);

		Vector4 uv = GetSpriteUV();
		float uv_w = uv.z - uv.x;
		float uv_h = uv.w - uv.y;

		int vertexCount = 0;

		// 计算最大圆角半径
		float maxLeftRadius = Mathf.Max(topLeft.radius, bottomLeft.radius);
		float maxRightRadius = Mathf.Max(topRight.radius, bottomRight.radius);
		float maxTopRadius = Mathf.Max(topLeft.radius, topRight.radius);
		float maxBottomRadius = Mathf.Max(bottomLeft.radius, bottomRight.radius);

		// 生成中心矩形
		AddRectangle(
			vh,
			new Vector2(v.x + maxLeftRadius, v.y + maxBottomRadius),
			new Vector2(v.z - maxRightRadius, v.w - maxTopRadius),
			color32,
			new Vector2(uv.x + uv_w * maxLeftRadius / r.width, uv.y + uv_h * maxBottomRadius / r.height),
			new Vector2(uv.z - uv_w * maxRightRadius / r.width, uv.w - uv_h * maxTopRadius / r.height),
			ref vertexCount
		);

		// 生成四个边缘矩形
		// 上边
		AddRectangle(
			vh,
			new Vector2(v.x + topLeft.radius, v.w - maxTopRadius),
			new Vector2(v.z - topRight.radius, v.w),
			color32,
			new Vector2(uv.x + uv_w * topLeft.radius / r.width, uv.w - uv_h * maxTopRadius / r.height),
			new Vector2(uv.z - uv_w * topRight.radius / r.width, uv.w),
			ref vertexCount
		);

		// 下边
		AddRectangle(
			vh,
			new Vector2(v.x + bottomLeft.radius, v.y),
			new Vector2(v.z - bottomRight.radius, v.y + maxBottomRadius),
			color32,
			new Vector2(uv.x + uv_w * bottomLeft.radius / r.width, uv.y),
			new Vector2(uv.z - uv_w * bottomRight.radius / r.width, uv.y + uv_h * maxBottomRadius / r.height),
			ref vertexCount
		);

		// 左边
		AddRectangle(
			vh,
			new Vector2(v.x, v.y + bottomLeft.radius),
			new Vector2(v.x + maxLeftRadius, v.w - topLeft.radius),
			color32,
			new Vector2(uv.x, uv.y + uv_h * bottomLeft.radius / r.height),
			new Vector2(uv.x + uv_w * maxLeftRadius / r.width, uv.w - uv_h * topLeft.radius / r.height),
			ref vertexCount
		);

		// 右边
		AddRectangle(
			vh,
			new Vector2(v.z - maxRightRadius, v.y + bottomRight.radius),
			new Vector2(v.z, v.w - topRight.radius),
			color32,
			new Vector2(uv.z - uv_w * maxRightRadius / r.width, uv.y + uv_h * bottomRight.radius / r.height),
			new Vector2(uv.z, uv.w - uv_h * topRight.radius / r.height),
			ref vertexCount
		);

		// 生成四个圆角
		// 左上角
		AddCorner(vh, new Vector2(v.x + topLeft.radius, v.w - topLeft.radius),
			topLeft.radius, 90, 180, color32,
			new Vector2(uv.x + uv_w * topLeft.radius / r.width, uv.w - uv_h * topLeft.radius / r.height),
			uv_w * topLeft.radius / r.width, uv_h * topLeft.radius / r.height,
			topLeft.segments, ref vertexCount);

		// 右上角
		AddCorner(vh, new Vector2(v.z - topRight.radius, v.w - topRight.radius),
			topRight.radius, 0, 90, color32,
			new Vector2(uv.z - uv_w * topRight.radius / r.width, uv.w - uv_h * topRight.radius / r.height),
			uv_w * topRight.radius / r.width, uv_h * topRight.radius / r.height,
			topRight.segments, ref vertexCount);

		// 左下角
		AddCorner(vh, new Vector2(v.x + bottomLeft.radius, v.y + bottomLeft.radius),
			bottomLeft.radius, 180, 270, color32,
			new Vector2(uv.x + uv_w * bottomLeft.radius / r.width, uv.y + uv_h * bottomLeft.radius / r.height),
			uv_w * bottomLeft.radius / r.width, uv_h * bottomLeft.radius / r.height,
			bottomLeft.segments, ref vertexCount);

		// 右下角
		AddCorner(vh, new Vector2(v.z - bottomRight.radius, v.y + bottomRight.radius),
			bottomRight.radius, 270, 360, color32,
			new Vector2(uv.z - uv_w * bottomRight.radius / r.width, uv.y + uv_h * bottomRight.radius / r.height),
			uv_w * bottomRight.radius / r.width, uv_h * bottomRight.radius / r.height,
			bottomRight.segments, ref vertexCount);
	}

	private void ClampRadius(CornerProperties corner, float maxRadius)
	{
		corner.radius = Mathf.Clamp(corner.radius, 0, maxRadius);
		corner.segments = Mathf.Max(corner.segments, 0);
	}

	private void AddRectangle(VertexHelper vh, Vector2 min, Vector2 max, Color32 color, Vector2 uvMin, Vector2 uvMax, ref int vertexCount)
	{
		vh.AddVert(new Vector3(min.x, min.y), color, new Vector2(uvMin.x, uvMin.y));
		vh.AddVert(new Vector3(min.x, max.y), color, new Vector2(uvMin.x, uvMax.y));
		vh.AddVert(new Vector3(max.x, max.y), color, new Vector2(uvMax.x, uvMax.y));
		vh.AddVert(new Vector3(max.x, min.y), color, new Vector2(uvMax.x, uvMin.y));

		vh.AddTriangle(vertexCount + 0, vertexCount + 1, vertexCount + 2);
		vh.AddTriangle(vertexCount + 2, vertexCount + 3, vertexCount + 0);

		vertexCount += 4;
	}

	private void AddCorner(VertexHelper vh, Vector2 center, float radius, float startAngle, float endAngle,
		Color32 color, Vector2 uvCenter, float uvRadiusX, float uvRadiusY, int cornerSegments, ref int vertexCount)
	{
		if (cornerSegments <= 0) return;

		// 添加圆心顶点
		int centerIndex = vertexCount;
		vh.AddVert(new Vector3(center.x, center.y), color, uvCenter);
		vertexCount++;

		int startIndex = vertexCount;
		float angleStep = (endAngle - startAngle) / cornerSegments;

		// 生成圆弧上的顶点
		for (int idx = 0; idx <= cornerSegments; idx++)
		{
			float angle = (startAngle + idx * angleStep) * Mathf.Deg2Rad;
			float cos = Mathf.Cos(angle);
			float sin = Mathf.Sin(angle);

			Vector2 pos = center + new Vector2(cos * radius, sin * radius);
			Vector2 uv = uvCenter + new Vector2(cos * uvRadiusX, sin * uvRadiusY);

			vh.AddVert(new Vector3(pos.x, pos.y), color, uv);
			vertexCount++;

			// 生成三角形，跳过第一个点
			if (idx > 0)
			{
				// 使用逆时针顺序确保正面朝向
				vh.AddTriangle(
					centerIndex,           // 中心点
					startIndex + idx,        // 当前圆弧顶点
					startIndex + idx - 1     // 上一个圆弧顶点
				);
			}
		}
	}

	private Vector4 GetSpriteUV()
	{
		Sprite activeSprite = overrideSprite ?? sprite;
		if (activeSprite == null)
		{
			return new Vector4(0, 0, 1, 1);
		}

		Rect rect = activeSprite.textureRect;

		float x = rect.x / activeSprite.texture.width;
		float y = rect.y / activeSprite.texture.height;
		float z = x + rect.width / activeSprite.texture.width;
		float w = y + rect.height / activeSprite.texture.height;
		return new Vector4(x, y, z, w);
	}
}