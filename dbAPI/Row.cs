using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dbAPI {
	///<summary>Represents a row in the database.</summary>
	public interface IRow : IEnumerable<ICell>, IEquatable<IRow>, ITabbedString, ICloneable {
		/// <summary>Gets the cell at the specified index.</summary>
		/// <param name="index">The index of the cell.</param>
		/// <returns>The cell at the index.</returns>
		ICell this[int index] { get; }
		/// <summary>Gets the cell with the specified column.</summary>
		/// <param name="column">The column of the cell.</param>
		/// <returns>The cell with the column.</returns>
		ICell this[string column] { get; }
		/// <summary>Gets the cell with the specified column.</summary>
		/// <param name="column">The column of the cell.</param>
		/// <returns>The cell with the column.</returns>
		ICell this[IColumn column] { get; }

		///<summary>The number of cells in the row.</summary>
		int Count { get; }
		///<summary>Whether the row has a primary key.</summary>
		bool HasPrimaryKey { get; }
		///<summary>The primary key of the row.</summary>
		IPrimaryKey PrimaryKey { get; }
		///<summary>The cells in the row.</summary>
		ICell[] Cells { get; }
		///<summary>The columns in the row.</summary>
		IColumn[] Columns { get; }
		///<summary>The values in the row.</summary>
		IDbValue[] Values { get; }

		///<summary>Get the index of the specified column name.</summary>
		///<param name="column">The name of the column to search for.</param>
		///<returns>The index of the column.</returns>
		int GetIndex(string column);
		///<summary>Get the index of the specified column.</summary>
		///<param name="column">The column to search for.</param>
		///<returns>The index of the column.</returns>
		int GetIndex(IColumn column);
		///<summary>Get the column at the specified index.</summary>
		///<param name="index">The index of the column.</param>
		///<returns>The column at the index.</returns>
		IColumn GetColumn(int index);
		///<summary>Get the column with the specified name.</summary>
		///<param name="name">The name of the column.</param>
		///<returns>The column with the name.</returns>
		IColumn GetColumn(string name);
		///<summary>Get the value at the specified index.</summary>
		///<param name="index">The index of the value.</param>
		///<returns>The value at the index.</returns>
		IDbValue GetValue(int index);
		///<summary>Get the value at the specified column.</summary>
		///<param name="column">The name of the column.</param>
		///<returns>The value at the column.</returns>
		IDbValue GetValue(string column);
		///<summary>Get the value at the specified column.</summary>
		///<param name="column">The column.</param>
		///<returns>The value at the column.</returns>
		IDbValue GetValue(IColumn column);
		///<summary>Set the value at the specified index.</summary>
		///<param name="index">The index of the value.</param>
		///<param name="value">The new value to set.</param>
		void SetValue(int index, IDbValue value);
		///<summary>Set the value at the specified column.</summary>
		///<param name="column">The name of the column at which to set value.</param>
		///<param name="value">The new value to set.</param>
		void SetValue(string column, IDbValue value);
		///<summary>Set the value at the specified column.</summary>
		///<param name="column">The column at which to set value.</param>
		///<param name="value">The new value to set.</param>
		void SetValue(IColumn column, IDbValue value);
	}
	///<inheritdoc cref="IRow"/>
	public class Row : IRow {
		///<inheritdoc cref="IRow.PrimaryKey"/>
		protected readonly IPrimaryKey pKey;
		///<inheritdoc cref="IRow.Cells"/>
		protected readonly ICell[] cells;

		public virtual ICell this[int index] => cells[index];
		public virtual ICell this[string column] => cells[GetIndex(column)];
		public virtual ICell this[IColumn column] => cells[GetIndex(column)];
		public virtual int Count => cells.Length;
		public virtual bool HasPrimaryKey => !(pKey is null);
		public virtual IPrimaryKey PrimaryKey => pKey;
		public virtual ICell[] Cells => (ICell[])cells.Clone();
		public virtual IColumn[] Columns { get {
				IColumn[] cols = new IColumn[cells.Length];
				for (int i = 0; i < cells.Length; i++)
					cols[i] = cells[i].Column.Clone();
				return cols;
		} }
		public virtual IDbValue[] Values { get {
				IDbValue[] vals = new IDbValue[cells.Length];
				for (int i = 0; i < cells.Length; i++)
					vals[i] = cells[i].Value.Clone();
				return vals;
		} }

		///<summary>Initialize a new row with cells.</summary>
		///<param name="cells">The cells for the row.</param>
		public Row(params ICell[] cells) {
			this.cells = cells.DeepClone();
			List<ICell> pKeys = new List<ICell>();
			foreach (ICell cell in cells)
				if (cell.Column.HasConstraint(ConstraintType.PRIMARY_KEY))
					pKeys.Add(cell);
			if (pKeys.Count == 1) pKey = new SinglePKey(pKeys[0]);
			else if (pKeys.Count == 0) pKey = null;
			else pKey = new PrimaryKey(pKeys.ToArray());
		}

		public static bool operator ==(Row left, IRow right) =>
			left is null ? right is null : left.Equals(right);
		public static bool operator !=(Row left, IRow right) =>
			!(left == right);

		public virtual IColumn GetColumn(int index) => this[index].Column;
		public virtual IColumn GetColumn(string name) => this[name].Column;
		///<inheritdoc cref="IRow.GetIndex(string)"/>
		///<exception cref="ArgumentException">If there's no such column in the row.</exception>
		public virtual int GetIndex(string column) {
			for (int i = 0; i < cells.Length; i++)
				if (cells[i].Column.Name.Equals(column)) return i;
			throw new ArgumentException($"No column '{column}' in the row", "column");
		}
		///<inheritdoc cref="IRow.GetIndex(IColumn)"/>
		///<exception cref="ArgumentException">If there's no such column in the row.</exception>
		public virtual int GetIndex(IColumn column) {
			for (int i = 0; i < cells.Length; i++)
				if (cells[i].Column.Equals(column)) return i;
			throw new ArgumentException($"No column '{column.Name}' in the row", "column");
		}
		public virtual IDbValue GetValue(int index) => this[index].Value;
		public virtual IDbValue GetValue(string column) => this[column].Value;
		public virtual IDbValue GetValue(IColumn column) => this[column].Value;
		public virtual void SetValue(int index, IDbValue value) => this[index].Value = value;
		public virtual void SetValue(string column, IDbValue value) => this[column].Value = value;
		public virtual void SetValue(IColumn column, IDbValue value) => this[column].Value = value;

		public virtual IEnumerator<ICell> GetEnumerator() => ((IEnumerable<ICell>)cells).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public virtual Row Clone() => new Row(cells);
		object ICloneable.Clone() => Clone();

		public virtual bool Equals(IRow other) {
			if (Object.ReferenceEquals(this, other)) return true;
			if (Count != other.Count) return false;
			for (int i = 0; i < Count; i++)
				if (!cells[i].Equals(other[i])) return false;
			//Theoretically useless, if all cells are equal, then the keys are too,
			//but just in case.
			return HasPrimaryKey ? PrimaryKey.Equals(other.PrimaryKey) : !other.HasPrimaryKey;
		}
		public override bool Equals(object obj) => Equals(obj as IRow);
		public override int GetHashCode() => cells.Hash();
		public override string ToString() => ToString(0);
		public virtual string ToString(int tabs) {
			StringBuilder builder = new StringBuilder();
			builder.Append("Row {\n");
			builder.Append('\t', tabs + 1).Append(pKey.ToString(tabs + 1)).Append('\n');
			foreach (ICell cell in cells)
				builder.Append('\t', tabs + 1).Append(cell.ToString(tabs + 1)).Append('\n');
			builder.Append('\t', tabs).Append('}');
			return builder.ToString();
		}
	}
}
