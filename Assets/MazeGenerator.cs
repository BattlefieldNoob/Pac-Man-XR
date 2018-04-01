using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    private const float STANDARD_HEIGHT = 0.5f;

    public GameObject CubePrefab;

    public int XSize = 20;
    public int ZSize = 20;

    private const float scale = 0.5f;

    // Use this for initialization
    private void Start()
    {
        CreateMap(XSize, ZSize);
    }

    private List<Vector3> _points = new List<Vector3>();

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
            _points.Add(new Vector3(xi * scale, STANDARD_HEIGHT, zi * scale));
        }

        // borders
        for (var i = 0; i < x; i++)
        {
            _points.Add(new Vector3(i * scale, STANDARD_HEIGHT, 0f));
            _points.Add(new Vector3(i * scale, STANDARD_HEIGHT, z * scale));
        }

        for (var i = 0; i < z; i++)
        {
            _points.Add(new Vector3(0f, STANDARD_HEIGHT, i * scale));
            _points.Add(new Vector3(x * scale, STANDARD_HEIGHT, i * scale));
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

        //cerco e rimuovo i punti che hanno zero, una o due connessioni, dove almeno un punto è collegato al bordo

        var pointsToRemove = new List<Vector3>();
        foreach (var point in _points)
        {
            //se è un bordo non faccio questo controllo
            if (point.IsBorder(XSize, ZSize,scale)) continue;

            var connectionCount = BoolToInt(ThereIsAnotherItemLeft(point)) + BoolToInt(ThereIsAnotherItemRight(point)) +
                                  BoolToInt(ThereIsAnotherItemUp(point)) + BoolToInt(ThereIsAnotherItemDown(point));

            if (connectionCount == 0)
            {
                //se il cubo non ha connessioni lo rimuovo
                pointsToRemove.Add(point);
            }
            else if (connectionCount <= 2 && ThereIsAtLeastOneBorderItem(point))
            {
                //se ho una o due connessioni, e sto collidendo con almeno un bordo, allora rimovo il punto
                pointsToRemove.Add(point);
            }
        }

        Debug.Log("Removing " + pointsToRemove.Count + " items");

        _points = _points.Except(pointsToRemove).ToList();

        var mazeContainter=new GameObject("MazeContainer");
        mazeContainter.transform.position=new Vector3((XSize*scale)/2,0, (ZSize*scale)/2);
        
        // generate things
        foreach (var vector3 in _points)
        {
            //il prefab ha già la scala corretta e viene instanziato nella posizione giusta
            var obj=Instantiate(CubePrefab, vector3, Quaternion.identity);
            obj.transform.parent = mazeContainter.transform;
        }

        GameObject.Find("Agent").transform.position = aRandomPointWithoutCubes();
        GameObject.Find("GreenGhost").transform.position = aRandomPointWithoutCubes();
    }

    private Vector3 aRandomPointWithoutCubes()
    {
        var maxAttempts = 1000;
        for (var i = 0; i < maxAttempts; i++)
        {
            var xi = Random.Range(0, XSize);
            var zi = Random.Range(0, ZSize);
            if (!_points.ToList().Exists(it => (it.x == xi && it.z == zi)))
                return new Vector3(xi * scale, 0.5566f, zi * scale);
        }

        return new Vector3(-1, 0, -1);
    }

    private bool ItWillBeTheAreaStillWalkable(Vector3 pointCandidate)
    {
        return !_points.Exists(it =>
        {
            var isValidPosition = it.x == pointCandidate.x && it.z == pointCandidate.z;
            isValidPosition = isValidPosition || ((it.x == pointCandidate.x + 2 * scale ||
                                                   it.x == pointCandidate.x - 2 * scale) &&
                                                  (it.z == pointCandidate.z - 2 * scale ||
                                                   it.z == pointCandidate.z - 2 * scale));
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

    private bool ThereIsBorderItemUp(Vector3 point)
    {
        return _points.Exists(it => point.Up(scale).Equals(it) && point.Up(scale).IsBorder(XSize, ZSize,scale));
    }

    private bool ThereIsBorderItemDown(Vector3 point)
    {
        return _points.Exists(it => point.Down(scale).Equals(it) && point.Down(scale).IsBorder(XSize, ZSize,scale));
    }

    private bool ThereIsBorderItemLeft(Vector3 point)
    {
        return _points.Exists(it => point.Left(scale).Equals(it) && point.Left(scale).IsBorder(XSize, ZSize,scale));
    }

    private bool ThereIsBorderItemRight(Vector3 point)
    {
        return _points.Exists(it => point.Right(scale).Equals(it) && point.Right(scale).IsBorder(XSize, ZSize,scale));
    }


    private int BoolToInt(bool expression) => expression ? 1 : 0;

    private bool ThereIsAtLeastOneBorderItem(Vector3 point)
    {
        return ThereIsBorderItemUp(point) || ThereIsBorderItemDown(point) || ThereIsBorderItemLeft(point) ||
               ThereIsBorderItemRight(point);
    }
}

public static class Vector3Extension
{
    public static Vector3 Up(this Vector3 point, float scale)
    {
        return point + Vector3.right * scale;
    }

    public static Vector3 Left(this Vector3 point, float scale)
    {
        return point + Vector3.back * scale;
    }

    public static Vector3 Right(this Vector3 point, float scale)
    {
        return point + Vector3.forward * scale;
    }

    public static Vector3 Down(this Vector3 point, float scale)
    {
        return point + Vector3.left * scale;
    }

    public static bool IsBorder(this Vector3 point, int borderX, int borderZ, float scale)
    {
        return Math.Abs(point.x) < 0.001f || Math.Abs(point.x - borderX*scale) < 0.001f || Math.Abs(point.z) < 0.001f ||  Math.Abs(point.z - borderZ*scale) < 0.001f;
    }
}