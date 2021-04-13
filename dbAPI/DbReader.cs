using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dbAPI {
	///<summary>Represents a reader that reads rows from the database.</summary>
	public interface IDbReader : IEnumerable<IRow>, IEnumerator<IRow> {
		///<summary>The row at the specified index of the reader.</summary>
		///<param name="index">The index of the row in the reader.</param>
		///<returns>The row at the reader index.</returns>
		IRow this[int index] { get; }
		///<summary>Get All the rows in the reader.</summary>
		///<returns>An array of </returns>
		IRow[] GetAllRows();
		
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
