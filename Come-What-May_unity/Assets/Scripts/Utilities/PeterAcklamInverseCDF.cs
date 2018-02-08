 /* Peter John Acklam Inverse CDF
 An very cheap and accurate algorithm for computing the inverse normal cumulative distribution function.
 
 Description: 
 This algorithm returns the x value of the inverse cumulative distribution function (AKA the quantile function or inverse CDF). The end result is an error less than 1.15E-9
 for all |x| < 38. (x < -38 has a probability 2.885E-136, which is smaller than Unity’s floating point precision.
 
 Designed by Peter John Acklam. Ported to Unity C# by James Armstrong.
 
 Source: http://home.online.no/~pjacklam/notes/invnorm/
 WayBack Archive: https://web.archive.org/web/20151030215612/http://home.online.no/~pjacklam/notes/invnorm/
 
 This software is distributed under the MIT License: 
     MIT License
     Copyright (c) 2016 James Armstrong
 
     Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software 
     without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
     permit persons to whom the Software is furnished to do so, subject to the following conditions:
 
         The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 
         THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
         PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
         OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
         */
 
 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 
public class PeterAcklamInverseCDF {
 
	// NormInv has two forms. This form allows for an offset normal distribution with a standard deviation different than 1.
	// Three variables are required for this variant: probability, mean and standard deviation (sigma).
	public static float NormInv(float probability, float mean, float sigma){
		float x = NormInv(probability);
		return sigma * x + mean;
	} // End of NormInv().
 
	// NormInv has two forms. This form allows for an offset normal distribution with a standard deviation different than 1.
	// One variable is required for this varient: only the probability. The mean is assumed to be 0 and the standard deviation is assumed to be 1.
	public static float NormInv(float probability){
		// Define variables used in intermediate steps
		float q = 0f;
		float r = 0f;
		float x = 0f;
 
		// Coefficients in rational approsimations.
		float[] a = new float[]{-3.969683028665376e+01f, 2.209460984245205e+02f, -2.759285104469687e+02f, 1.383577518672690e+02f, -3.066479806614716e+01f, 2.506628277459239e+00f};
		float[] b = new float[]{-5.447609879822406e+01f, 1.615858368580409e+02f, -1.556989798598866e+02f, 6.680131188771972e+01f, -1.328068155288572e+01f}; 
		float[] c = new float[]{-7.784894002430293e-03f, -3.223964580411365e-01f, -2.400758277161838e+00f, -2.549732539343734e+00f, 4.374664141464968e+00f, 2.938163982698783e+00f};
		float[] d = new float[]{ 7.784695709041462e-03f, 3.224671290700398e-01f, 2.445134137142996e+00f, 3.754408661907416e+00f};
 
		// Define break-points
		float pLow = 0.02425f;
		float pHigh = 1f - pLow;
 
		// Verify that probability is between 0 and 1 (noninclusinve), and if not, make between 0 and 1
 
		if (probability <= 0f) {
			probability = Mathf.Epsilon;
		} else if (probability >= 1f) {
			probability = 1f - Mathf.Epsilon;
		}
 
		// Rational approximation for lower region.
		if (probability < pLow){
			q = Mathf.Sqrt (-2f * Mathf.Log (probability));
			x = (((((c [0] * q + c [1]) * q + c [2]) * q + c [3]) * q + c [4]) * q + c [5]) / ((((d [0] * q + d [1]) * q + d [2]) * q + d [3]) * q + 1f);
		}
 
		// Rational approximation for central region.
		if (pLow <= probability && probability <= pHigh){
			q = probability - 0.5f;
			r = q * q;
			x = (((((a [0] * r + a [1]) * r + a [2]) * r + a [3]) * r + a [4]) * r + a [5]) * q / (((((b [0] * r + b [1]) * r + b [2]) * r + b [3]) * r + b [4]) * r + 1f);
		}
 
		// Rational approximation for upper region.
		if (pHigh < probability){
			q = Mathf.Sqrt(-2*Mathf.Log(1f - probability));
			x = -(((((c [0] * q + c [1]) * q + c [2]) * q + c [3]) * q + c [4]) * q + c [5]) / ((((d [0] * q + d [1]) * q + d [2]) * q + d [3]) * q + 1f);
		}
 
		return x;
	} // End of NormInv().

} // End of PeterAcklamInverseCDF.