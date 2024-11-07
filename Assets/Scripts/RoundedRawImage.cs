using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Rounded RawImage", 12)]
public class RoundedRawImage : RawImage
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
		GenerateRoundedRectMesh(vh);
	}

	private void GenerateRoundedRectMesh(VertexHelper vh)
	{
		Rect r = rectTransform.rect;
		int vertexCount = 0;

		Rect rect = rectTransform.rect;
		float maxRadius = Mathf.Min(rect.width, rect.height) * 0.5f;
		ClampRadius(topLeft, maxRadius);
		ClampRadius(topRight, maxRadius);
		ClampRadius(bottomLeft, maxRadius);
		ClampRadius(bottomRight, maxRadius);

		// 计算四个角的圆心位置
		Vector2 topLeftCenter = new Vector2(r.xMin + topLeft.radius, r.yMax - topLeft.radius);
		Vector2 topRightCenter = new Vector2(r.xMax - topRight.radius, r.yMax - topRight.radius);
		Vector2 bottomLeftCenter = new Vector2(r.xMin + bottomLeft.radius, r.yMin + bottomLeft.radius);
		Vector2 bottomRightCenter = new Vector2(r.xMax - bottomRight.radius, r.yMin + bottomRight.radius);

		// 生成中心矩形
		GenerateCenterRect(vh, topLeftCenter, topRightCenter, bottomLeftCenter, bottomRightCenter, ref vertexCount);

		// 生成四个边缘矩形
		GenerateEdgeRects(vh, r, topLeftCenter, topRightCenter, bottomLeftCenter, bottomRightCenter, ref vertexCount);

		// 修正圆角的角度范围
		GenerateCorner(vh, topLeftCenter, topLeft.radius, topLeft.segments, 90, 180, ref vertexCount);       // 左上角
		GenerateCorner(vh, topRightCenter, topRight.radius, topRight.segments, 0, 90, ref vertexCount);      // 右上角
		GenerateCorner(vh, bottomLeftCenter, bottomLeft.radius, bottomLeft.segments, 180, 270, ref vertexCount); // 左下角
		GenerateCorner(vh, bottomRightCenter, bottomRight.radius, bottomRight.segments, 270, 360, ref vertexCount); // 右下角
	}
	private void GenerateCenterRect(VertexHelper vh, Vector2 topLeft, Vector2 topRight,
			Vector2 bottomLeft, Vector2 bottomRight, ref int vertexCount)
	{
		// 添加四个顶点
		AddVertex(vh, new Vector2(topLeft.x, topLeft.y), ref vertexCount);
		AddVertex(vh, new Vector2(topRight.x, topRight.y), ref vertexCount);
		AddVertex(vh, new Vector2(bottomLeft.x, bottomLeft.y), ref vertexCount);
		AddVertex(vh, new Vector2(bottomRight.x, bottomRight.y), ref vertexCount);

		// 修改三角形顶点顺序为逆时针
		vh.AddTriangle(vertexCount - 4, vertexCount - 3, vertexCount - 2);
		vh.AddTriangle(vertexCount - 3, vertexCount - 1, vertexCount - 2);
	}

	private void GenerateEdgeRects(VertexHelper vh, Rect rect, Vector2 topLeft, Vector2 topRight,
		Vector2 bottomLeft, Vector2 bottomRight, ref int vertexCount)
	{
		// 上边矩形
		AddVertex(vh, new Vector2(topLeft.x, rect.yMax), ref vertexCount);
		AddVertex(vh, new Vector2(topRight.x, rect.yMax), ref vertexCount);
		AddVertex(vh, new Vector2(topLeft.x, topLeft.y), ref vertexCount);
		AddVertex(vh, new Vector2(topRight.x, topRight.y), ref vertexCount);
		AddQuadTriangles(vh, vertexCount - 4);

		// 右边矩形
		AddVertex(vh, new Vector2(topRight.x, topRight.y), ref vertexCount);
		AddVertex(vh, new Vector2(rect.xMax, topRight.y), ref vertexCount);
		AddVertex(vh, new Vector2(bottomRight.x, bottomRight.y), ref vertexCount);
		AddVertex(vh, new Vector2(rect.xMax, bottomRight.y), ref vertexCount);
		AddQuadTriangles(vh, vertexCount - 4);

		// 下边矩形
		AddVertex(vh, new Vector2(bottomLeft.x, bottomLeft.y), ref vertexCount);
		AddVertex(vh, new Vector2(bottomRight.x, bottomRight.y), ref vertexCount);
		AddVertex(vh, new Vector2(bottomLeft.x, rect.yMin), ref vertexCount);
		AddVertex(vh, new Vector2(bottomRight.x, rect.yMin), ref vertexCount);
		AddQuadTriangles(vh, vertexCount - 4);

		// 左边矩形
		AddVertex(vh, new Vector2(rect.xMin, topLeft.y), ref vertexCount);
		AddVertex(vh, new Vector2(topLeft.x, topLeft.y), ref vertexCount);
		AddVertex(vh, new Vector2(rect.xMin, bottomLeft.y), ref vertexCount);
		AddVertex(vh, new Vector2(bottomLeft.x, bottomLeft.y), ref vertexCount);
		AddQuadTriangles(vh, vertexCount - 4);
	}

	private void GenerateCorner(VertexHelper vh, Vector2 center, float radius, int segments,
		float startAngle, float endAngle, ref int vertexCount)
	{
		// 添加圆心顶点
		AddVertex(vh, center, ref vertexCount);
		int centerIndex = vertexCount - 1;

		float angleStep = (endAngle - startAngle) / segments;

		// 生成圆弧顶点
		for (int idx = 0; idx <= segments; idx++)
		{
			float angle = startAngle + (idx * angleStep);
			float angleRad = angle * Mathf.Deg2Rad;

			Vector2 position = center + new Vector2(
				Mathf.Cos(angleRad) * radius,
				Mathf.Sin(angleRad) * radius
			);

			AddVertex(vh, position, ref vertexCount);

			if (idx > 0)
			{
				// 修改三角形顶点顺序为逆时针
				vh.AddTriangle(centerIndex, vertexCount - 1, vertexCount - 2);
			}
		}
	}

	private void AddQuadTriangles(VertexHelper vh, int startIndex)
	{
		// 修改三角形顶点顺序为逆时针
		vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
		vh.AddTriangle(startIndex + 1, startIndex + 3, startIndex + 2);
	}

	private void AddVertex(VertexHelper vh, Vector2 position, ref int vertexCount)
	{
		Color32 color32 = color;

		// 先计算顶点在RectTransform中的相对位置 (0-1)
		Vector2 normalizedPosition = new Vector2(
			(position.x - rectTransform.rect.xMin) / rectTransform.rect.width,
			(position.y - rectTransform.rect.yMin) / rectTransform.rect.height
		);

		// 将归一化位置映射到uvRect定义的UV范围内
		Vector2 uv = new Vector2(
			uvRect.x + normalizedPosition.x * uvRect.width,
			uvRect.y + normalizedPosition.y * uvRect.height
		);

		vh.AddVert(new Vector3(position.x, position.y, 0f), color32, uv);
		vertexCount++;
	}

	private void ClampRadius(CornerProperties corner, float maxRadius)
	{
		corner.radius = Mathf.Clamp(corner.radius, 0, maxRadius);
		corner.segments = Mathf.Max(corner.segments, 0);
	}
}