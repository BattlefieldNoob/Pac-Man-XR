using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour
{
	private const float STANDARD_HEIGHT = 0.5f;

	public GameObject CubePrefab;


	// Use this for initialization
	void Start ()
	{
		createMap(10, 10);
  
	}
 
	// Update is called once per frame
	void Update () {
  
	}

	
	//Ho reso la funzione non statica, altrimenti non potevo usare la variabile "CubePrefab" (non potevo farla statica perchè unity non 
	//mostra le variabili statiche nell'inspector)
	private void createMap(int x, int z)
	{
		var points = new List<Vector3>();
  
		for (int i = 0; i < x; i++)
		{
			// Random points
			var xi = Random.Range(0, x);
			var zi = Random.Range(0, z);
			if (!points.ToList().Exists(it => (it.x + 2 == xi || it.x - 2 == xi) && (it.z - 2 == zi || it.z + 2 == zi)))
			points.Add(new Vector3(xi, STANDARD_HEIGHT, zi));
		}
  
		// borders
		for (int i = 0; i < x; i++)
		{
			points.Add(new Vector3(i, STANDARD_HEIGHT, 0f));
			points.Add(new Vector3(i, STANDARD_HEIGHT, z));
		}

		for (int i = 0; i < z; i++)
		{
			points.Add(new Vector3(0f, STANDARD_HEIGHT, i));
			points.Add(new Vector3(x, STANDARD_HEIGHT, i));
		}

		var supportPoints = new List<Vector3>();

		for (int xi = 0; xi < x; xi++)
		{
			for (int zi = 0 ; zi < z; zi++)
			{
				if (!points.Union(supportPoints).ToList().Exists(it => (it.x + 2 == xi || it.x - 2 == xi) && (it.z - 2 == zi || it.z + 2 == zi)))
				{
					supportPoints.Add(new Vector3(xi, STANDARD_HEIGHT, zi));
				}
			}
		}
  
		// generate things
		foreach (var vector3 in points.Union(supportPoints))
		{
			//il prefab ha già la scala corretta e viene instanziato nella posizione giusta
			Instantiate(CubePrefab, vector3, Quaternion.identity);
			
			/*	var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.transform.position = new Vector3(vector3.x, vector3.y, vector3.z);
				cube.transform.localScale = new Vector3(1, 2, 1);*/
		}
	}
}
