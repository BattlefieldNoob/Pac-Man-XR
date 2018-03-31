using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoisePlane : MonoBehaviour
{
	private MeshRenderer _renderer;
	public float scale = 1;
	
	private List<Vector2> curves=new List<Vector2>();

	private Texture2D texture;

	public GameObject obj;
	void Start ()
	{
		_renderer = GetComponent<MeshRenderer>();
	    texture=new Texture2D(10,10);
		texture.filterMode = FilterMode.Point;
		
		foreach (var x in Enumerable.Range(0,texture.width))
		{
			foreach (var y in Enumerable.Range(0,texture.height))
			{
				var value=Mathf.PerlinNoise((float)x/(texture.width*scale), (float)y/(texture.height*scale));
				if (value < 0.2f)
				{
					//Debug.Log($"i:{x},i1:{y}, value:{value}");
					texture.SetPixel(x, y, Color.blue);
					var point=new Vector2(x,y);
					if (!curves.Any(vector2 => Vector2.Distance(vector2, point) < 2))
					{
						curves.Add(point);
					}
				}
				else
				{

					texture.SetPixel(x, y, new Color(value, value, value));
				}
			}
		}
		texture.Apply();

		_renderer.material.mainTexture = texture;
		Debug.Log($"points:{curves.Count}");
		PositionateCurves();
	}



	void PositionateCurves()
	{
		foreach (var curve in curves)
		{
			Debug.Log($"x:{curve.x}, y:{curve.y}");

			if (curve.x > texture.width / 2)
			{
				if (curve.y > texture.height / 2)
				{
					Instantiate(obj, new Vector3((-curve.x+5)+0.5f,0,(-curve.y+5)+0.5f), Quaternion.identity);
				}
			}
		}
	}
}
