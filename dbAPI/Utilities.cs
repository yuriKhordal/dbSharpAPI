using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dbAPI {
	public static class Utilities {
		///<summary>Hash the contents of the collection.</summary>
		///<typeparam name="T">The type of objects in the collection.</typeparam>
		///<param name="enumerable">The collection to hash.</param>
		///<returns>The calculated hash code of all the elements in the collection.</returns>
		public static int Hash<T>(this IEnumerable<T> enumerable) {
			HashCode hash = new HashCode();
			foreach (T t in enumerable) hash.Add(t);
			return hash.ToHashCode();
		}
		///<summary>Create a new array, and clone every member into it.</summary>
		///<typeparam name="T">The type of the members of the array.</typeparam>
		///<param name="arr">The array to clone.</param>
		///<returns>A new array that has a copy of each member.</returns>
		public static T[] DeepClone<T>(this T[] arr) where T : ICloneable {
			return DeepClone(arr, 0, arr.Length);
		}
		///<summary>Create a new array with a specified size, and clone that amount
		/// of elements from a specified starting point.</summary>
		///<remarks>If the size is larger then the amount of elements from the starting
		/// point, the new array will still be of specified length, but the rest of it
		/// will be left uninitialized, e.g. if the array length is 7, the starting point
		/// is 5, and the size is 4, the new array will be of length 4, the first two
		/// elements will be copies of elements 5,6 and the rest uninitialized.</remarks>
		///<typeparam name="T">The type of the members of the array.</typeparam>
		///<param name="arr">The array to clone.</param>
		///<param name="start">The first element from which to start cloning.</param>
		///<param name="size">The size of the new array, and the amount of elements
		/// to clone.</param>
		///<returns>A new array with elements cloned from this array.</returns>
		///<exception cref=""></exception>
		public static T[] DeepClone<T>(this T[] arr, int start, int size)
				where T : ICloneable {
			T[] copy = new T[size];
			if (arr.Length < start + size) size = arr.Length - start;
			for (int i = 0; i < size; i++)
				copy[i] = (T)arr[start + i].Clone();
			return copy;
		}
		///<summary>Convert an array of one type to another.</summary>
		///<typeparam name="TIn">The type of the input array.</typeparam>
		///<typeparam name="TOut">The type of the output array.</typeparam>
		///<param name="arr">The array to convert.</param>
		///<param name="converter">A function that converts an object of input type,
		/// to an object of output type.</param>
		///<returns>An array of output type.</returns>
		public static TOut[] Convert<TIn, TOut>(TIn[] arr, Func<TIn, TOut> converter) {
			TOut[] arr2 = new TOut[arr.Length];
			for (int i = 0; i < arr.Length; i++) arr2[i] = converter(arr[i]);
			return arr2;
		}
		///<summary>Convert members of an array that satisfy a condition from one type
		/// to another.</summary>
		///<remarks>The converted value is only used in the condition that the converter
		/// returns true, thus, if it returns false, the other value can be returned as
		/// anything without consequences.</remarks>
		///<typeparam name="TIn">The type of the input array.</typeparam>
		///<typeparam name="TOut">The type of the output array.</typeparam>
		///<param name="arr">The array to convert.</param>
		///<param name="converter">A function that returns whether to convert a member,
		/// and if true, converts an object of input type, to an object of output type.</param>
		///<returns>An array of output type.</returns>
		public static TOut[] ConvertIf<TIn, TOut>(TIn[] arr, Func<TIn, (bool, TOut)> converter) {
			List<TOut> list = new List<TOut>(arr.Length);
			for (int i = 0; i < arr.Length; i++) {
				(bool assign, TOut t) = converter(arr[i]);
				if (assign) list.Add(t);
			}
			return list.ToArray();
		}
		///<summary>Check equality of two enumerables' members.</summary>
		///<param name="self">One enumerable.</param>
		///<param name="other">Other enumerable.</param>
		///<returns>True if all the members of both enumerables are equal.
		/// False otherwise.</returns>
		public static bool MemberEquals(this IEnumerable self, IEnumerable other) {
			IEnumerator enum1 = self.GetEnumerator(), enum2 = other.GetEnumerator();
			bool has1 = enum1.MoveNext(), has2 = enum2.MoveNext();
			while (has1 && has2) {
				if (!Object.Equals(enum1.Current, enum2.Current)) return false;
				has1 = enum1.MoveNext();
				has2 = enum2.MoveNext();
			}
			return has1 == has2;
		}
	}
	///<summary>Defines a method for an indented ToString.</summary>
	public interface ITabbedString {
		///<summary>Returns a string representation of the object with the specified
		/// amount of tabs before the start of each line, except for the first line.</summary>
		///<param name="tabs">The amount of tabs to insert before a line.</param>
		///<returns>A string with tabs before each line.</returns>
		string ToString(int tabs);
	}
	///<summary>Represents an enumerator for a single value.</summary>
	///<typeparam name="T">The type of the value.</typeparam>
	internal class SingleEnumerator<T> : IEnumerator<T> {
		///<summary>Whether <see cref="MoveNext"/> has been called.</summary>
		protected bool moved = false;
		///<summary>The value in the enumerator.</summary>
		protected readonly T val;

		public T Current => val;
		object IEnumerator.Current => val;

		///<summary>Initialize a new enumerator with a value.</summary>
		///<param name="val">The value for the enumerator.</param>
		public SingleEnumerator(T val) => this.val = val;

		public bool MoveNext() => moved ? false : moved = true;
		public void Reset() => moved = false;
		public void Dispose() {
			if (val is IDisposable d) d.Dispose();
		}
	}
	public class RowEnumerator<T> : IEnumerator<IRow> where T : IRow {
		IEnumerator<T> numer;

		public RowEnumerator(IEnumerable<T> numer) => this.numer = numer.GetEnumerator();

		public IRow Current => numer.Current;
		object IEnumerator.Current => ((IEnumerator)numer).Current;

		public void Dispose() => numer.Dispose();
		public bool MoveNext() => numer.MoveNext();
		public void Reset() => numer.Reset();
	}
}
