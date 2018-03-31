using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour
{
	private const float STANDARD_HEIGHT = 0.5f;

	public GameObject CubePrefab;
	
	private const float scale = 0.5f;

	// Use this for initialization
	private void Start ()
	{
		CreateMap(20, 20);
  
	}
	
	private List<Vector3> _points = new List<Vector3>();
 
	// Update is called once per frame
	void Update () {
  
	}

	
	//Ho reso la funzione non statica, altrimenti non potevo usare la variabile "CubePrefab" (non potevo farla statica perchè unity non 
	//mostra le variabili statiche nell'inspector)
	private void CreateMap(int x, int z)
	{
		_points = new List<Vector3>();

		for (var i = 0; i < x; i++)
		{
			// Random points
			var xi = Random.Range(0, x);
			var zi = Random.Range(0, z);
			//if (!points.ToList().Exists(it => (it.x + 2 == xi || it.x - 2 == xi) && (it.z - 2 == zi || it.z + 2 == zi)))
			_points.Add(new Vector3(xi*scale, STANDARD_HEIGHT, zi*scale));
		}

		// borders
		for (var i = 0; i < x; i++)
		{
			_points.Add(new Vector3(i*scale, STANDARD_HEIGHT, 0f));
			_points.Add(new Vector3(i*scale, STANDARD_HEIGHT, z*scale));
		}

		for (var i = 0; i < z; i++)
		{
			_points.Add(new Vector3(0f, STANDARD_HEIGHT, i*scale));
			_points.Add(new Vector3(x*scale, STANDARD_HEIGHT, i*scale));
		}

		//    Random points leaving space to move
		for (var i = 0; i < x * z; i++)
		{
			
			var pointCandidate = new Vector3(Random.Range(0, x) * scale, STANDARD_HEIGHT, Random.Range(0, z) * scale);
			if (CanAddPointAtCoordinates(pointCandidate) && ItWillBeTheAreaStillWalkable(pointCandidate))
			{
				_points.Add(pointCandidate);
			}
		}

		
		// generate things
		foreach (var vector3 in _points)
		{
			//il prefab ha già la scala corretta e viene instanziato nella posizione giusta
			Instantiate(CubePrefab, vector3, Quaternion.identity);
		}
		
	}

	private bool ItWillBeTheAreaStillWalkable(Vector3 pointCandidate)
	{
		return !_points.Exists(it =>
		{
			var isValidPosition = it.x == pointCandidate.x && it.z == pointCandidate.z;	
			isValidPosition = isValidPosition || ((it.x == pointCandidate.x + 2 * scale ||
			                                       it.x == pointCandidate.x - 2 * scale) &&
			                                      (it.z  == pointCandidate.z - 2 * scale || it.z == pointCandidate.z - 2*scale));
			return isValidPosition;
		});
	}

	private bool CanAddPointAtCoordinates(Vector3 point)
	{
		//	*	* 
		//	*	x
		if (ThereIsAnotherItemUp(point) && ThereIsAnotherItemLeftUp(point) && ThereIsAnotherItemLeft(point))
		{
			return false; 
		}
		//	*	* 
		//	x	*
		if (ThereIsAnotherItemUp(point) && ThereIsAnotherItemRightUp(point) && ThereIsAnotherItemRight(point))
		{
			return false; 
		}
		//	x	* 
		//	*	*
		if (ThereIsAnotherItemDown(point) && ThereIsAnotherItemRight(point) && ThereIsAnotherItemRightDown(point))
		{
			return false; 
		}
		//	*	x 
		//	*	*
		if (ThereIsAnotherItemDown(point) && ThereIsAnotherItemUp(point) && ThereIsAnotherItemLeftDown(point))
		{
			return false; 
		}
		return true;
	}

	private bool ThereIsAnotherItemUp(Vector3 point)
	{
		return _points.Exists(it => point.x + 1 * scale == it.x && point.z == it.z); 
	}
	private bool ThereIsAnotherItemDown(Vector3 point)
	{
		return _points.Exists(it => point.x - 1 * scale == it.x && point.z == it.z); 
	}
	private bool ThereIsAnotherItemLeft(Vector3 point)
	{
		return _points.Exists(it => point.x == it.x && point.z - 1 * scale == it.z); 
	}
	private bool ThereIsAnotherItemRight(Vector3 point)
	{
		return _points.Exists(it => point.x == it.x && point.z + 1 * scale == it.z); 
	}
	private bool ThereIsAnotherItemLeftUp(Vector3 point)
	{
		return _points.Exists(it => point.x + 1 * scale == it.x && point.z - 1 * scale == it.z); 
	}
	private bool ThereIsAnotherItemRightUp(Vector3 point)
	{
		return _points.Exists(it => point.x + 1 * scale == it.x && point.z + 1 * scale == it.z); 
	}
	private bool ThereIsAnotherItemLeftDown(Vector3 point)
	{
		return _points.Exists(it => point.x - 1 * scale == it.x && point.z - 1 * scale == it.z); 
	}
	private bool ThereIsAnotherItemRightDown(Vector3 point)
	{
		return _points.Exists(it => point.x - 1 * scale == it.x && point.z + 1 * scale == it.z); 
	}
	
}
