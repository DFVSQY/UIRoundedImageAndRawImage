using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(RoundedRawImage), true)]
[CanEditMultipleObjects]
public class RoundedRawImageEditor : RawImageEditor
{
	private SerializedProperty topLeftProp;
	private SerializedProperty topRightProp;
	private SerializedProperty bottomLeftProp;
	private SerializedProperty bottomRightProp;
	private SerializedProperty setModeProp;

	private GUIContent radiusLabel = new GUIContent("Radius");
	private GUIContent segmentsLabel = new GUIContent("Segments");

	protected override void OnEnable()
	{
		base.OnEnable();

		topLeftProp = serializedObject.FindProperty("topLeft");
		topRightProp = serializedObject.FindProperty("topRight");
		bottomLeftProp = serializedObject.FindProperty("bottomLeft");
		bottomRightProp = serializedObject.FindProperty("bottomRight");
		setModeProp = serializedObject.FindProperty("setMode");
	}

	public override void OnInspectorGUI()
	{
		// 调用基类方法显示基础属性
		base.OnInspectorGUI();

		// 更新序列化对象
		serializedObject.Update();

		// 添加分割线
		EditorGUILayout.Space();
		EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
		EditorGUILayout.Space();

		// 设置模式的展示
		EditorGUILayout.PropertyField(setModeProp);

		RoundedRawImage image = target as RoundedRawImage;
		RoundedRawImage.SetMode setMode = image.setMode;
		if (setMode == RoundedRawImage.SetMode.United)
		{
			EditorGUI.BeginChangeCheck();
			float radius = EditorGUILayout.FloatField(radiusLabel, image.topLeft.radius);
			if (EditorGUI.EndChangeCheck())
			{
				SetRadius(topLeftProp, radius);
				SetRadius(topRightProp, radius);
				SetRadius(bottomLeftProp, radius);
				SetRadius(bottomRightProp, radius);
			}

			EditorGUI.BeginChangeCheck();
			int segments = EditorGUILayout.IntField(segmentsLabel, image.topLeft.segments);
			if (EditorGUI.EndChangeCheck())
			{
				SetSegments(topLeftProp, segments);
				SetSegments(topRightProp, segments);
				SetSegments(bottomLeftProp, segments);
				SetSegments(bottomRightProp, segments);
			}
		}
		else
		{
			EditorGUILayout.PropertyField(topLeftProp);
			EditorGUILayout.PropertyField(topRightProp);
			EditorGUILayout.PropertyField(bottomLeftProp);
			EditorGUILayout.PropertyField(bottomRightProp);
		}

		// 应用修改后的属性
		serializedObject.ApplyModifiedProperties();
	}

	private void SetRadius(SerializedProperty cornerProp, float radius)
	{
		cornerProp.FindPropertyRelative("radius").floatValue = radius;
	}

	private void SetSegments(SerializedProperty cornerProp, int segments)
	{
		cornerProp.FindPropertyRelative("segments").intValue = segments;
	}
}
