using UnityEngine;
using System.Collections;

public class ArrayUtils {

	public static float[] toFloatArray(double[] darr){
		float[] res = new float[darr.Length];
		for (int i = 0; i<darr.Length; i++) {
			res[i] = (float)darr[i];
		}
		return res;
	}
}
