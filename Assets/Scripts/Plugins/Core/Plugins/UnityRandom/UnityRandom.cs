using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using NPack;
using URandom;
using Random = UnityEngine.Random;

/////////////////////////////////////////////////////////////////////////////
//                                                                         //
// Unity Random                                                            //
//                                                                         //
// This code is free software under the Artistic license.                  //
//                                                                         //
// distributions mainly from: http://www.nrbook.com/a/bookcpdf.php         //
//                                                                         //
//                                                                         //
/////////////////////////////////////////////////////////////////////////////

public interface IRandomGenerator
{
	float Range(float min, float max);
	int Range(int min, int max);
}

public class UnityRandomUnity : IRandomGenerator
{
	public float Range(float min, float max)
	{
		return Random.Range(min, max);
	}

	public int Range(int min, int max)
	{
		return Random.Range(min, max);
	}
}

public class UnityRandomSystem : IRandomGenerator
{
	public System.Random	m_Random;

	//////////////////////////////////////////////////////////////////////////
	public float Range(float min, float max)
	{
		return Random.Range(min, max);
	}

	public int Range(int min, int max)
	{
		return Random.Range(min, max);
	}

	public UnityRandomSystem()
	{
		m_Random = new System.Random();
	}

	public UnityRandomSystem(int seed)
	{
		m_Random = new System.Random(seed);
	}
}

public class UnityRandom : IRandomGenerator
{
	// STORE MAX SEED VALUE (no access to System)
	public static int max_seed = int.MaxValue;

	public enum Normalization
	{
		STDNORMAL = 0,
		POWERLAW = 1
	}

	private MersenneTwister _rand;

	// THE CONSTRUCTORS
	public UnityRandom()
	{
		_rand = new MersenneTwister();
	}

	public UnityRandom(int seed)
	{
		_rand = new MersenneTwister(seed);
	}

	// VALUE Return a Float 0 - 1
	public float Value()
	{
		return _rand.NextSingle(true);
	}

	// VALUE Return a Float 0 - 1
	public float Value(Normalization n, float t)
	{
		if (n == Normalization.STDNORMAL)
			return (float) NormalDistribution.Normalize(_rand.NextSingle(true), t);
		else if (n == Normalization.POWERLAW)
			return (float) PowerLaw.Normalize(_rand.NextSingle(true), t, 0, 1);
		else
			return _rand.NextSingle(true);
	}

	// RANGE Return a int min <= x < max
	public int Range(int minValue, int maxValue)
	{
		return _rand.Next(minValue, maxValue);
	}

	// RANGE Return a int min <= x <= max
	public int RangeInclusive(int minValue, int maxValue)
	{
		return _rand.Next(minValue, maxValue + 1);
	}

	// RANGE Return a int min <= x < max
	public float Range(float minValue, float maxValue)
	{
		return (float) ((double) _rand.NextUInt32() / (ulong) uint.MaxValue) * (maxValue - minValue) + minValue;
		//return ((float)_rand.NextUInt32() / ((UInt64)UInt32.MaxValue + 1)) * (maxValue - minValue) + minValue;
	}

	// RANGE Return a int min <= x <= max
	public float RangeInclusive(float minValue, float maxValue)
	{
		return (float) ((double) _rand.NextUInt32() / ((ulong) uint.MaxValue + 1)) * (maxValue - minValue) + minValue;
	}

	// RANGE Return a Float min < x < max
	public float Range(int minValue, int maxValue, Normalization n, float t)
	{
		if (n == Normalization.STDNORMAL)
			return SpecialFunctions.ScaleFloatToRange((float) NormalDistribution.Normalize(_rand.NextSingle(true), t),
				minValue, maxValue, 0, 1);
		else if (n == Normalization.POWERLAW)
			return (float) PowerLaw.Normalize(_rand.NextSingle(true), t, minValue, maxValue);
		else
			return _rand.Next(minValue, maxValue);
	}

	// POISSON Return a Float
	public float Possion(float lambda)
	{
		return PoissonDistribution.Normalize(ref _rand, lambda);
	}

	// EXPONENTIAL Return a Float
	public float Exponential(float lambda)
	{
		return ExponentialDistribution.Normalize(_rand.NextSingle(false), lambda);
	}

	// GAMMA Return a Float
	public float Gamma(float order)
	{
		return GammaDistribution.Normalize(ref _rand, (int) order);
	}

	// POINT IN A SQUARE Return a Vector2
	public Vector2 PointInASquare()
	{
		return RandomSquare.Area(ref _rand);
	}

	// POINT IN A SQUARE Return a Vector2
	public Vector2 PointInASquare(Normalization n, float t)
	{
		return RandomSquare.Area(ref _rand, n, t);
	}

	// RANDOM POINT IN A CIRCLE centered at 0
	// FROM http://mathworld.wolfram.com/CirclePointPicking.html
	// Take a number between 0 and 2PI and move to Cartesian Coordinates
	public Vector2 PointInACircle()
	{
		return RandomDisk.Circle(ref _rand);
	}

	// RANDOM POINT IN A CIRCLE centered at 0
	// FROM http://mathworld.wolfram.com/CirclePointPicking.html
	// Take a number between 0 and 2PI and move to Cartesian Coordinates
	public Vector2 PointInACircle(Normalization n, float t)
	{
		return RandomDisk.Circle(ref _rand, n, t);
	}

	// RANDOM POINT in a DISK
	// FROM http://mathworld.wolfram.com/DiskPointPicking.html
	public Vector2 PointInADisk()
	{
		return RandomDisk.Disk(ref _rand);
	}

	// RANDOM POINT in a DISK
	// FROM http://mathworld.wolfram.com/DiskPointPicking.html
	public Vector2 PointInADisk(Normalization n, float t)
	{
		return RandomDisk.Disk(ref _rand, n, t);
	}

	// RANDOM POINT IN A CUBE. Return a Vector3
	public Vector3 PointInACube()
	{
		return RandomCube.Volume(ref _rand);
	}

	// RANDOM POINT IN A CUBE. Return a Vector3
	public Vector3 PointInACube(Normalization n, float t)
	{
		return RandomCube.Volume(ref _rand, n, t);
	}

	// RANDOM POINT ON A CUBE. Return a Vector3
	public Vector3 PointOnACube()
	{
		return RandomCube.Surface(ref _rand);
	}

	// RANDOM POINT ON A CUBE. Return a Vector3
	public Vector3 PointOnACube(Normalization n, float t)
	{
		return RandomCube.Surface(ref _rand, n, t);
	}

	// RANDOM POINT ON A SPHERE. Return a Vector3
	public Vector3 PointOnASphere()
	{
		return RandomSphere.Surface(ref _rand);
	}

	// RANDOM POINT ON A SPHERE. Return a Vector3
	public Vector3 PointOnASphere(Normalization n, float t)
	{
		throw new ArgumentException("Normalizations for Sphere is not yet implemented");
	}

	// RANDOM POINT IN A SPHERE. Return a Vector3
	public Vector3 PointInASphere()
	{
		return RandomSphere.Volume(ref _rand);
	}

	// RANDOM POINT IN A SPHERE. Return a Vector3
	public Vector3 PointInASphere(Normalization n, float t)
	{
		throw new ArgumentException("Normalizations for Sphere is not yet implemented");
	}

	// RANDOM POINT IN A CAP. Return a Vector3 
	// TODO: see RandomSphere GetPointOnCap(float spotAngle, ref NPack.MersenneTwister _rand, Quaternion orientation)
	public Vector3 PointOnCap(float spotAngle)
	{
		return RandomSphere.GetPointOnCap(spotAngle, ref _rand);
	}

	public Vector3 PointOnCap(float spotAngle, Normalization n, float t)
	{
		throw new ArgumentException("Normalizations for PointOnCap is not yet implemented");
	}

	// RANDOM POINT IN A RING on a SPHERE. Return a Vector3 
	// TODO: see RandomSphere public static Vector3 GetPointOnRing(float innerSpotAngle, float outerSpotAngle, ref NPack.MersenneTwister _rand, Quaternion orientation)
	public Vector3 PointOnRing(float innerAngle, float outerAngle)
	{
		return RandomSphere.GetPointOnRing(innerAngle, outerAngle, ref _rand);
	}

	public Vector3 PointOnRing(float innerAngle, float outerAngle, Normalization n, float t)
	{
		throw new ArgumentException("Normalizations for PointOnRing is not yet implemented");
	}

	// RANDOM RAINBOW COLOR
	public Color Rainbow()
	{
		return WaveToRgb.LinearToRgb(_rand.NextSingle(true));
	}

	// RANDOM RAINBOW COLOR
	public Color Rainbow(Normalization n, float t)
	{
		if (n == Normalization.STDNORMAL)
			return WaveToRgb.LinearToRgb((float) NormalDistribution.Normalize(_rand.NextSingle(true), t));
		else if (n == Normalization.POWERLAW)
			return WaveToRgb.LinearToRgb((float) PowerLaw.Normalize(_rand.NextSingle(true), t, 0, 1));
		else
			return WaveToRgb.LinearToRgb(_rand.NextSingle(true));
	}

	// RANDOM DICES
	public DiceRoll RollDice(int size, DiceRoll.DiceType type)
	{
		var roll = new DiceRoll(size, type, ref _rand);
		//Debug.Log(roll.TypeToString());
		//Debug.Log(roll.RollToString());
		//Debug.Log(roll.Sum());
		return roll;
	}

	// RANDOM from animation curve position
	public float RollAnimationCurve(AnimationCurve curve, float pos)
	{
		return Range(0.0f, curve.Evaluate(pos));
	}

	// START a FLOAT SHUFFLE BAG
	// Note the a value can be shuffled with himself
	public ShuffleBagCollection<float> ShuffleBag(float[] values)
	{
		var bag = new ShuffleBagCollection<float>();
		foreach (var x in values) bag.Add(x);
		return bag;
	}

	// START a WIGHTED FLOAT SHUFFLE BAG, the trick is the it is added many times
	// Note the a value can be shuffled with himself
	public ShuffleBagCollection<float> ShuffleBag(Dictionary<float, int> dict)
	{
		var bag = new ShuffleBagCollection<float>();
		foreach (var x in dict)
		{
			//Debug.Log(x.Value);
			var val = x.Value;
			var key = x.Key;
			bag.Add(key, val);
		}

		return bag;
	}

	// 
	public WeightedRandomCollection<T> WeightedBag<T>(T[] values, float initalWeight)
	{
		var result = new WeightedRandomCollection<T>(this);
		foreach (var n in values)
			result.Add(n, initalWeight);

		return result;
	}

	public WeightedRandomCollection<T> WeightedBag<T>(T[] values, float[] initalWeights)
	{
		var result = new WeightedRandomCollection<T>(this);
		for (var n = 0; n < values.Length; ++n)
			result.Add(values[n], initalWeights[n]);

		return result;
	}

	//
	public WilledRandomInstance WilledRandom()
	{
		return new WilledRandomInstance() {m_Generator = this};
	}

	//
	public VotedRandomCollection<T> VotedRandom<T>(T[] values, float[] initalWeights)
	{
		var result = new VotedRandomCollection<T>(this);
		for (var n = 0; n < values.Length; ++n)
			result.Add(values[n], initalWeights[n]);

		return result;
	}

	public static WeightedRandomCollection<T> CreateWeightedBag<T>(T[] values, float initalWeight, IRandomGenerator randomGenerator = null)
	{
		var result = new WeightedRandomCollection<T>(randomGenerator);
		foreach (var n in values)
			result.Add(n, initalWeight);

		return result;
	}
	// 
	public static WeightedRandomCollection<T> CreateWeightedBag<T>(IEnumerable<T> values, IEnumerable<float> initalWeights, IRandomGenerator randomGenerator = null)
	{
		var result = new WeightedRandomCollection<T>(randomGenerator);

		using (var valueEnumerator = values.GetEnumerator())
		using (var weightEnumerator = initalWeights.GetEnumerator())
		{
			while (valueEnumerator.MoveNext() && weightEnumerator.MoveNext())
				result.Add(valueEnumerator.Current, weightEnumerator.Current);
		}


		return result;
	}

	// uses unity random function, moves every member to random position
	public static void RandomizeList<T>(IList<T> source)
	{
		var n = source.Count;
		while (n > 1)
		{
			var k = UnityEngine.Random.Range(0, n--);
			var value = source[k];
			source[k] = source[n];
			source[n] = value;
		}
	}

	// uses unity random function, random pos to random location
	public static void RandomizeList<T>(IList<T> source, int swapsCount)
	{
		for (var n = 0; n < swapsCount; ++n)
		{
			var c = UnityEngine.Random.Range(0, source.Count);
			var t = UnityEngine.Random.Range(0, source.Count);

			var value = source[c];
			source[c] = source[t];
			source[t] = value;
		}
	}
	
	public static T RandomFromArray<T>(params T[] source)
	{
		return source[UnityEngine.Random.Range(0, source.GetLength(0))];
	}

    public static T RandomFromArray<T>(T[] source, T noOptionsValue)
    {
        if (source.Length == 0)
            return noOptionsValue;

        return source[UnityEngine.Random.Range(0, source.GetLength(0))];
    }

	// get random element from list
	public static T RandomFromList<T>(IList<T> source)
	{
		return source[UnityEngine.Random.Range(0, source.Count)];
	}

	// get random element from list
	public static T RandomFromList<T>(IList<T> source, T noOptionsValue)
	{
		if (source == null || source.Count == 0)
			return noOptionsValue;

		return source[UnityEngine.Random.Range(0, source.Count)];
	}

    public static List<T> RandomUniqueFromList<T>(IList<T> source, int count)
    {
        if (count == source.Count)
            return source.ToList();

        count = Mathf.Min(count, source.Count);

        var result = new HashSet<T>();
        while (result.Count != count)
            result.Add(RandomFromList(source));

        return result.ToList();
    }

    // get random element from list, except some values
	public static T RandomFromList<T>(IList<T> source, T noOptionsValue, params T[] exept)
	{
		var result = new List<T>(source.Count);

		foreach (var n in source)
			if (Array.IndexOf(exept, n) == -1)
				result.Add(n);

		if (result.Count == 0)
			return noOptionsValue;

		return RandomFromList(result);
	}

	// angle in degrees
	public static Vector2 NormalDeviation(Vector2 normal, float angleMin, float angleMax)
	{
		var angle = Mathf.Atan2(normal.y, normal.x) +
		            UnityEngine.Random.Range(angleMin * Mathf.Deg2Rad, angleMax * Mathf.Deg2Rad);

		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}

	public static bool Bool(float trueProbability = 0.5f)
	{
		return UnityEngine.Random.value < trueProbability ? true : false;
	}

	public static bool Bool(int trueProbability)
	{
		return UnityEngine.Random.Range(0, 99) < trueProbability ? true : false;
	}

	public static Stack<bool> RandomStack(float chance, int distance = 100)
	{
		return RandomStack(Mathf.RoundToInt(distance * chance), distance);
	}

	public static Stack<bool> RandomStack(int count, int distance)
	{
		// fill first part of the list with false values and second part with true values, randomize list, copy to stack
		// slow but works
		count = Mathf.Clamp(count, 0, distance);

		var stack = new Stack<bool>(distance);
		var values = new List<bool>(distance);
		for (var n = 0; n < count; n++)
			values.Add(true);
		for (var n = count; n < distance; n++)
			values.Add(false);

		RandomizeList(values);
		foreach (var n in values)
			stack.Push(n);

		return stack;
	}

	public static Color Color()
	{
		return new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f),
			UnityEngine.Random.Range(0.0f, 1.0f), 1.0f);
	}

	public static Color Color(float alpha)
	{
		return new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f),
			UnityEngine.Random.Range(0.0f, 1.0f), alpha);
	}

	public static Color Color(bool randomAlpha)
	{
		return new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f),
			UnityEngine.Random.Range(0.0f, 1.0f), randomAlpha ? UnityEngine.Random.Range(0.0f, 1.0f) : 1.0f);
	}

	public static Color Color(Vector2 rRange, Vector2 gRange, Vector2 bRange, Vector2 aRange)
	{
		return new Color(UnityEngine.Random.Range(rRange.x, rRange.y), UnityEngine.Random.Range(gRange.x, gRange.y),
			UnityEngine.Random.Range(bRange.x, bRange.y), UnityEngine.Random.Range(aRange.x, aRange.y));
	}

	public static Vector2 Vector2(Vector2 xRange, Vector2 yRange)
	{
		return new Vector2(UnityEngine.Random.Range(xRange.x, xRange.y), UnityEngine.Random.Range(yRange.x, yRange.y));
	}

	public static Vector2 Vector2(float xRange, float yRange)
	{
		return new Vector2(UnityEngine.Random.Range(0.0f, xRange), UnityEngine.Random.Range(0.0f, yRange));
	}

	public static Vector2 Vector2(float xMin, float xMax, float yMin, float yMax)
	{
		return new Vector2(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
	}
    
    public static Vector3 Vector3(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
    {
        return new Vector3(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax), UnityEngine.Random.Range(zMin, zMax));
    }

	internal static Vector2Int Vector2Int(int xRange, int yRange)
	{
		return new Vector2Int(UnityEngine.Random.Range(0, xRange), UnityEngine.Random.Range(0, yRange));
	}

    public static Vector3 BoundPoint(Bounds bound)
    {
        return new Vector3(
            UnityEngine.Random.Range(bound.min.x, bound.max.x),
            UnityEngine.Random.Range(bound.min.y, bound.max.y),
            UnityEngine.Random.Range(bound.min.z, bound.max.z));
    }

    public static Vector2 Normal2D()
    {
        return Vector2(-1.0f, 1.0f, -1.0f,1.0f).normalized;
    }

    public static Vector2 Normal3D()
    {
        return Vector3(-1.0f, 1.0f, -1.0f,1.0f, -1.0f, 1.0f).normalized;
    }
}