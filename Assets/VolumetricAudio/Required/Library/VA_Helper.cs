using UnityEngine;
using System.Collections.Generic;

public static partial class VA_Helper
{
	public static int MeshVertexLimit = 65000;
	
	private static AudioListener audioListener;
	
	public static AudioListener AudioListener
	{
		get
		{
			if (audioListener == null) audioListener = Object.FindObjectOfType<AudioListener>();
			
			return audioListener;
		}
	}
	
	public static Vector2 SinCos(float a)
	{
		return new Vector2(Mathf.Sin(a), Mathf.Cos(a));
	}
	
	public static void Destroy(Object o)
	{
#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			Object.DestroyImmediate(o, true); return;
		}
#endif
		
		Object.Destroy(o);
	}
	
	public static void StealthSet(MeshFilter mf, Mesh m)
	{
		if (mf != null && mf.sharedMesh != m)
		{
#if UNITY_EDITOR
			var hf = mf.hideFlags;
			
			mf.hideFlags  = HideFlags.DontSave;
			mf.sharedMesh = m;
			mf.hideFlags  = hf;
#else
			mf.sharedMesh = m;
#endif
		}
	}
	
	public static void SetPosition(Transform t, Vector3 v)
	{
		if (t != null)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false && t.position == v) return;
#endif
			t.position = v;
		}
	}
	
	public static void SetRotation(Transform t, Quaternion q)
	{
		if (t != null)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false && t.rotation == q) return;
#endif
			t.rotation = q;
		}
	}
	
	public static bool Enabled(Behaviour b)
	{
		return b != null && b.enabled == true && b.gameObject.activeInHierarchy == true;
	}
	
	public static float Divide(float a, float b)
	{
		return Zero(b) == false ? a / b : 0.0f;
	}
	
	public static float Reciprocal(float v)
	{
		return Zero(v) == false ? 1.0f / v : 0.0f;
	}
	
	public static bool Zero(float v)
	{
		return Mathf.Approximately(v, 0.0f);
	}
	
	public static Matrix4x4 RotationMatrix(Quaternion q)
	{
		var matrix = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
		
		return matrix;
	}
	
	public static Matrix4x4 TranslationMatrix(Vector3 xyz)
	{
		return TranslationMatrix(xyz.x, xyz.y, xyz.z);
	}
	
	public static Matrix4x4 TranslationMatrix(float x, float y, float z)
	{
		var matrix = Matrix4x4.identity;
		
		matrix.m03 = x;
		matrix.m13 = y;
		matrix.m23 = z;
		
		return matrix;
	}
	
	public static Matrix4x4 ScalingMatrix(float xyz)
	{
		return ScalingMatrix(xyz, xyz, xyz);
	}
	
	public static Matrix4x4 ScalingMatrix(Vector3 xyz)
	{
		return ScalingMatrix(xyz.x, xyz.y, xyz.z);
	}
	
	public static Matrix4x4 ScalingMatrix(float x, float y, float z)
	{
		var matrix = Matrix4x4.identity;
		
		matrix.m00 = x;
		matrix.m11 = y;
		matrix.m22 = z;
		
		return matrix;
	}
	
	public static float DampenFactor(float dampening, float elapsed)
	{
		return 1.0f - Mathf.Pow((float)System.Math.E, - dampening * elapsed);
	}
	
	public static Quaternion Dampen(Quaternion current, Quaternion target, float dampening, float elapsed, float minStep = 0.0f)
	{
		var factor   = DampenFactor(dampening, elapsed);
		var maxDelta = Quaternion.Angle(current, target) * factor + minStep * elapsed;
		
		return MoveTowards(current, target, maxDelta);
	}
	
	public static float Dampen(float current, float target, float dampening, float elapsed, float minStep = 0.0f)
	{
		var factor   = DampenFactor(dampening, elapsed);
		var maxDelta = Mathf.Abs(target - current) * factor + minStep * elapsed;
		
		return MoveTowards(current, target, maxDelta);
	}
	
	public static Vector3 Dampen3(Vector3 current, Vector3 target, float dampening, float elapsed, float minStep = 0.0f)
	{
		var factor   = DampenFactor(dampening, elapsed);
		var maxDelta = Mathf.Abs((target - current).magnitude) * factor + minStep * elapsed;
		
		return Vector3.MoveTowards(current, target, maxDelta);
	}
	
	public static Quaternion MoveTowards(Quaternion current, Quaternion target, float maxDelta)
	{
		var delta = Quaternion.Angle(current, target);
		
		return Quaternion.Slerp(current, target, Divide(maxDelta, delta));
	}
	
	public static float MoveTowards(float current, float target, float maxDelta)
	{
		if (target > current)
		{
			current = System.Math.Min(target, current + maxDelta);
		}
		else
		{
			current = System.Math.Max(target, current - maxDelta);
		}
		
		return current;
	}
	
	public static Vector3 ClosestPointToLineSegment(Vector3 a, Vector3 b, Vector3 point)
	{
		var l = (b - a).magnitude;
		var d = (b - a).normalized;
		
		return a + Mathf.Clamp(Vector3.Dot(point - a, d), 0.0f, l) * d;
	}
	
	public static Vector3 ClosestPointToTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
	{
		var r  = Quaternion.Inverse(Quaternion.LookRotation(-Vector3.Cross(a - b, a - c)));
		var ra = r * a;
		var rb = r * b;
		var rc = r * c;
		var rp = r * p;
		
		var a2 = VA_Helper.VectorXY(ra);
		var b2 = VA_Helper.VectorXY(rb);
		var c2 = VA_Helper.VectorXY(rc);
		var p2 = VA_Helper.VectorXY(rp);
		
		if (PointLeftOfLine(a2, b2, p2) == true)
		{
			return ClosestPointToLineSegment(a, b, p);
		}
		
		if (PointLeftOfLine(b2, c2, p2) == true)
		{
			return ClosestPointToLineSegment(b, c, p);
		}
		
		if (PointLeftOfLine(c2, a2, p2) == true)
		{
			return ClosestPointToLineSegment(c, a, p);
		}
		
		var barycentric = GetBarycentric(a2, b2, c2, p2);
		
		return barycentric.x * a + barycentric.y * b + barycentric.z * c;
	}
	
	public static Vector3 GetBarycentric(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
	{
		var barycentric = Vector3.zero;
		var v0          = b - a;
		var v1          = c - a;
		var v2          = p - a;
		var d00         = Vector2.Dot(v0, v0);
		var d01         = Vector2.Dot(v0, v1);
		var d11         = Vector2.Dot(v1, v1);
		var d20         = Vector2.Dot(v2, v0);
		var d21         = Vector2.Dot(v2, v1);
		var denom       = VA_Helper.Reciprocal(d00 * d11 - d01 * d01);
		
		barycentric.y = (d11 * d20 - d01 * d21) * denom;
		barycentric.z = (d00 * d21 - d01 * d20) * denom;
		barycentric.x = 1.0f - barycentric.y - barycentric.z;
		
		return barycentric;
	}
	
	public static bool PointLeftOfLine(Vector2 a, Vector2 b, Vector2 p) // NOTE: CCW
	{
		return ((b.x - a.x) * (p.y - a.y) - (p.x - a.x) * (b.y - a.y)) >= 0.0f;
	}
	
	public static bool PointRightOfLine(Vector2 a, Vector2 b, Vector2 p) // NOTE: CCW
	{
		return ((b.x - a.x) * (p.y - a.y) - (p.x - a.x) * (b.y - a.y)) <= 0.0f;
	}
	
	public static Vector2 VectorXY(Vector3 xyz)
	{
		return new Vector2(xyz.x, xyz.y);
	}
}