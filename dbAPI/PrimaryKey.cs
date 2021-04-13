using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dbAPI {
	///<summary>Represents a basic primary key, consisting only of values.</summary>
	public interface IBasicPKey : IEnumerable<IDbValue>, IEquatable<IBasicPKey>,
			ITabbedString, ICloneable {
		///<summary>Gets the value at the specified index.</summary>
		///<param name="index">The index of the value.</param>
		///<returns>The value at the index.</returns>
		IDbValue this[int index] { get; }
		///<summary>The value of the key, if the key has multiple values,
		///returns the first value.</summary>
		IDbValue Value { get; }
		///<summary>The values that comprise the key.</summary>
		IDbValue[] Values { get; }
		///<summary>The number of values in the key.</summary>
		int Count { get; }
		///<summary>Is the key singular, i.e. does it have only one value.</summary>
		bool IsSingular { get; }
		///<inheritdoc cref="ICloneable.Clone"/>
		new IBasicPKey Clone();
	}
	///<summary>Represents a primary key.</summary>
	public interface IPrimaryKey : IBasicPKey, IEnumerable<ICell>, IEquatable<IPrimaryKey>,
			ICloneable {
		///<summary>Gets the cell at the specified index.</summary>
		///<param name="index">The index of the cell.</param>
		///<returns>The cell at the index.</returns>
		new ICell this[int index] { get; }
		///<summary>Gets the cell with the specified column.</summary>
		///<param name="column">The column of the cell to return.</param>
		///<returns>The cell with the column.</returns>
		ICell this[IColumn column] { get; }
		///<summary>Gets the cell with the specified column.</summary>
		///<param name="column">The column of the cell to return.</param>
		///<returns>The cell with the column.</returns>
		ICell this[string column] { get; }
		///<summary>The cell of the key, if the key has multiple columns,
		///returns the first column.</summary>
		ICell Cell { get; }
		///<summary>The cells that comprise the key.</summary>
		ICell[] Cells { get; }
		///<summary>The column of the key, if the key has multiple columns,
		///returns the first column.</summary>
		IColumn Column { get; }
		///<summary>The columns of the key cells.</summary>
		IColumn[] Columns { get; }

		///<summary>Get the column at the specified index.</summary>
		///<param name="index">The index of the column.</param>
		///<returns>The column at the index.</returns>
		IColumn GetColumn(int index);
		///<summary>Get the column with the specified column name.</summary>
		///<param name="column">The name of the column.</param>
		///<returns>The column with the name.</returns>
		IColumn GetColumn(string column);
		///<summary>Get the value at the specified index.</summary>
		///<param name="index">The index of the value.</param>
		///<returns>The value at the index.</returns>
		IDbValue GetValue(int index);
		///<summary>Get the value on the specified column.</summary>
		///<param name="column">The column.</param>
		///<returns>The value at the column.</returns>
		IDbValue GetValue(IColumn column);
		///<summary>Get the value on the specified column.</summary>
		///<param name="column">The column.</param>
		///<returns>The value at the column.</returns>
		IDbValue GetValue(string column);
		///<inheritdoc cref="ICloneable.Clone"/>
		new IPrimaryKey Clone();
	}
	///<inheritdoc cref="IBasicPKey"/>
	public class BasicPKey : IBasicPKey, IEquatable<BasicPKey> {
		///<inheritdoc cref="Values"/>
		protected readonly IDbValue[] values;

		public virtual IDbValue this[int index] => values[index];
		public virtual IDbValue Value => values[0];
		public virtual IDbValue[] Values => (IDbValue[])values.Clone();
		public virtual int Count => values.Length;
		public virtual bool IsSingular => Count == 1;

		///<summary>Initialize a new basic primary key with values.</summary>
		///<param name="values">The values of the key.</param>
		public BasicPKey(params IDbValue[] values) => this.values = values.DeepClone();

		public static bool operator ==(BasicPKey left, IBasicPKey right) =>
			left is null ? right is null : left.Equals(right);
		public static bool operator !=(BasicPKey left, IBasicPKey right) =>
			!(left == right);

		///<inheritdoc cref="ICloneable.Clone"/>
		public virtual BasicPKey Clone() => new BasicPKey(values);
		IBasicPKey IBasicPKey.Clone() => this.Clone();
		object ICloneable.Clone() => this.Clone();
		public virtual IEnumerator<IDbValue> GetEnumerator() =>
			((IEnumerable<IDbValue>)Values).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public virtual bool Equals(BasicPKey other) => Equals(other as IBasicPKey);
		public virtual bool Equals(IBasicPKey other) {
			if (Object.ReferenceEquals(this, other)) return true;
			if (other.Count != Count) return false;
			for (int i = 0; i < Count; i++)
				if (!this[i].Equals(other[i])) return false;
			return true;
		}
		public override bool Equals(object obj) => Equals(obj as IBasicPKey);
		public override int GetHashCode() => values.Hash();
		public override string ToString() => ToString(0);
		public virtual string ToString(int tabs) {
			StringBuilder builder = new StringBuilder();
			builder.Append("BasicKey {\n");

			foreach (IDbValue val in Values) builder.Append('\t', tabs + 1)
					.Append(val.ToString(tabs + 1)).Append('\n');
			builder.Append('\t', tabs).Append('}');

			return builder.ToString();
		}
	}
	///<summary>Represents a basic primary key, consisting only of values,
	///this specific implementation represents a key with one value.</summary>
	public class SingleBasicPKey : DbValue, IBasicPKey, IEquatable<SingleBasicPKey> {
		IDbValue IBasicPKey.this[int index] =>
			index == 0 ? this : throw new IndexOutOfRangeException();
		new public virtual IDbValue Value => this;
		IDbValue[] IBasicPKey.Values => new IDbValue[] { this };
		public virtual int Count => 1;
		public virtual bool IsSingular => true;

		///<inheritdoc cref="DbValue.DbValue(object, IDataType)"/>
		public SingleBasicPKey(object value, IDataType type) : base(value, type) { }
		///<summary>Initialize a new basic primary key with a value.</summary>
		///<param name="value">The value of the key.</param>
		public SingleBasicPKey(IDbValue value) : base(value.Value, value.Type) { }

		public static bool operator ==(SingleBasicPKey left, IBasicPKey right) =>
			left is null ? right is null : left.Equals(right);
		public static bool operator !=(SingleBasicPKey left, IBasicPKey right) =>
			!(left == right);

		new public virtual SingleBasicPKey Clone() => new SingleBasicPKey(this);
		IBasicPKey IBasicPKey.Clone() => this.Clone();
		object ICloneable.Clone() => this.Clone();
		public virtual IEnumerator<IDbValue> GetEnumerator() =>
			new SingleEnumerator<IDbValue>(this);
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public bool Equals(SingleBasicPKey other) => Equals(other as IBasicPKey);
		public virtual bool Equals(IBasicPKey other) =>
			Object.ReferenceEquals(this, other) || (!(other is null) &&
			other.IsSingular && other.Value.Equals(Value));
		public override bool Equals(object obj) => Equals(obj as IBasicPKey);
		public override int GetHashCode() => base.GetHashCode();
		public override string ToString(int tabs) =>
			"Single Basic PKey: " + base.ToString(tabs);
	}
	///<inheritdoc cref="IPrimaryKey"/>
	public class PrimaryKey : BasicPKey, IPrimaryKey, IEquatable<PrimaryKey> {
		///<inheritdoc cref="Cells"/>
		protected ICell[] cells;
		///<inheritdoc cref="Columns"/>
		protected IColumn[] columns;

		public virtual ICell this[IColumn column] { get {
			foreach (ICell cell in cells)
				if (cell.Column.Equals(column)) return cell;
			throw new ArgumentException($"No column '{column.Name}' in the key", "column");
		} }
		public virtual ICell this[string column] { get {
			foreach (ICell cell in cells)
				if (cell.Column.Name.Equals(column)) return cell;
			throw new ArgumentException($"No column '{column}' in the key", "column");
		} }
		///<inheritdoc cref="IPrimaryKey.this[int]"/>
		new public virtual ICell this[int index] => cells[index];
		public virtual ICell Cell => cells[0];
		public virtual ICell[] Cells => (ICell[])cells.Clone();
		public virtual IColumn Column => columns[0];
		public virtual IColumn[] Columns => (IColumn[])columns.Clone();

		///<summary>Initialize a new primary key with cells.</summary>
		///<param name="cells">The cells of the key</param>
		public PrimaryKey(params ICell[] cells) :
			base(Utilities.Convert(cells, (cell) => cell.Value)) =>
			(this.cells, columns) =
			(cells.DeepClone(), Utilities.Convert(cells, (cell) => cell.Column));
		/*public PrimaryKey(params ICell[] cells) : base(GetValues(cells)) {
			this.cells = cells.DeepClone();
			columns = new IColumn[cells.Length];
			for (int i = 0; i < cells.Length; i++)
				columns[i] = cells[i].Column;
		}*/

		///<summary>Get the values from an array of cells.</summary>
		///<param name="cells">The cells with the values.</param>
		///<returns>An array of values from the cells.</returns>
		private static IDbValue[] GetValues(ICell[] cells) {
			IDbValue[] values = new IDbValue[cells.Length];
			for (int i = 0; i < cells.Length; i++)
				values[i] = cells[i].Value;
			return values;
		}
		public static bool operator ==(PrimaryKey left, IPrimaryKey right) =>
			left is null ? right is null : left.Equals(right);
		public static bool operator !=(PrimaryKey left, IPrimaryKey right) =>
			!(left == right);

		public virtual IColumn GetColumn(int index) => columns[index];
		public virtual IColumn GetColumn(string column) {
			foreach (IColumn col in columns)
				if (col.Name.Equals(column)) return col;
			throw new ArgumentException($"No column '{column}' in the key", "column");
		}
		public virtual IDbValue GetValue(int index) => values[index];
		public virtual IDbValue GetValue(IColumn column) {
			foreach (ICell cell in cells)
				if (cell.Column.Equals(column)) return cell.Value;
			throw new ArgumentException($"No column '{column.Name}' in the key", "column");
		}
		public virtual IDbValue GetValue(string column) {
			foreach (ICell cell in cells)
				if (cell.Column.Name.Equals(column)) return cell.Value;
			throw new ArgumentException($"No column '{column}' in the key", "column");
		}
		new public virtual PrimaryKey Clone() => new PrimaryKey(cells);
		IPrimaryKey IPrimaryKey.Clone() => this.Clone();
		new public virtual IEnumerator<ICell> GetEnumerator() =>
			((IEnumerable<ICell>)cells).GetEnumerator();
		public virtual bool Equals(PrimaryKey other) => Equals(other as IPrimaryKey);
		public virtual bool Equals(IPrimaryKey other) {
			if (Object.ReferenceEquals(this, other)) return true;
			if (Count != other.Count) return false;
			for (int i = 0; i < cells.Length; i++)
				if (!this[i].Equals(other[i])) return false;
			return true;
		}
		public override bool Equals(object obj) => 
			obj is IPrimaryKey pKey ? Equals(pKey) : Equals(obj as IBasicPKey);
		public override int GetHashCode() => cells.Hash();
		public override string ToString() => ToString(0);
		public override string ToString(int tabs) {
			StringBuilder builder = new StringBuilder();
			builder.Append("Primary Key {\n");

			foreach (ICell cell in cells) builder.Append('\t', tabs + 1)
				.Append(cell.ToString(tabs + 1)).Append('\n');
			builder.Append('\t', tabs).Append('}');

			return builder.ToString();
		}
	}
	///<summary>Represents a primary key that only consists of one cell.</summary>
	public class SinglePKey : Cell, IPrimaryKey, IEquatable<SinglePKey> {
		ICell IPrimaryKey.this[int index] => index == 0 ? this : throw new IndexOutOfRangeException();
		ICell IPrimaryKey.this[IColumn column] => Column.Equals(column) ? this :
			throw new ArgumentException($"Wrong column '{column.Name}'", "column");
		ICell IPrimaryKey.this[string column] => Column.Name.Equals(column) ? this :
			throw new ArgumentException($"Wrong column '{column}'", "column");
		IDbValue IBasicPKey.this[int index] => ((IPrimaryKey)this)[index].Value;

		ICell IPrimaryKey.Cell => this;
		ICell[] IPrimaryKey.Cells => new ICell[] { this };
		IColumn[] IPrimaryKey.Columns => new IColumn[] { Column };
		IDbValue[] IBasicPKey.Values => new IDbValue[] { Value };
		public virtual int Count => 1;
		public virtual bool IsSingular => true;

		///<summary>Initialize a new key with a cell.</summary>
		///<param name="cell">The key as a cell.</param>
		public SinglePKey(ICell cell) : base(cell.Column, cell.Value) { }
		///<summary>Initialize a new key with a column and a value.</summary>
		///<param name="col">The key's column.</param>
		///<param name="val">The key's value</param>
		public SinglePKey(IColumn col, IDbValue val) : base(col, val) { }
		///<summary>Initialize a new key with a column and a value.</summary>
		///<param name="col">The key's column.</param>
		///<param name="val">The key's value</param>
		public SinglePKey(IColumn col, object val) : base(col, val) { }

		public static bool operator ==(SinglePKey left, IPrimaryKey right) =>
			left is null ? right is null : left.Equals(right);
		public static bool operator !=(SinglePKey left, IPrimaryKey right) =>
			!(left == right);

		///<summary>Return the key as a <see cref="PrimaryKey"/>.</summary>
		///<returns>A <see cref="PrimaryKey"/> that is equivalent to this key.</returns>
		public PrimaryKey ToPrimaryKey() => new PrimaryKey(new Cell(Column, Value));

		IColumn IPrimaryKey.GetColumn(int index) => ((IPrimaryKey)this)[index].Column;
		IColumn IPrimaryKey.GetColumn(string column) => ((IPrimaryKey)this)[column].Column;
		IDbValue IPrimaryKey.GetValue(int index) => ((IPrimaryKey)this)[index].Value;
		IDbValue IPrimaryKey.GetValue(IColumn column) => ((IPrimaryKey)this)[column].Value;
		IDbValue IPrimaryKey.GetValue(string column) => ((IPrimaryKey)this)[column].Value;
		///<inheritdoc cref="ICloneable.Clone"/>
		new public virtual SinglePKey Clone() => new SinglePKey(this);
		IPrimaryKey IPrimaryKey.Clone() => this.Clone();
		IBasicPKey IBasicPKey.Clone() => this.Clone();
		IEnumerator<ICell> IEnumerable<ICell>.GetEnumerator() => new SingleEnumerator<ICell>(this);
		IEnumerator<IDbValue> IEnumerable<IDbValue>.GetEnumerator() =>
			new SingleEnumerator<IDbValue>(Value);
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ICell>)this).GetEnumerator();

		public bool Equals(SinglePKey other) => Equals(other as IPrimaryKey);
		public virtual bool Equals(IBasicPKey other) =>
			Object.ReferenceEquals(this, other) || (!(other is null) &&
			(other is IPrimaryKey pKey ? Equals(pKey) :
			other.IsSingular && other.Value.Equals(Value)));
		public virtual bool Equals(IPrimaryKey other) =>
			!(other is null) && other.IsSingular && other.Cell.Equals(this);
		public override bool Equals(object obj) =>
			obj is IPrimaryKey pKey ? Equals(pKey) : Equals(obj as IBasicPKey);
		public override int GetHashCode() => base.GetHashCode();
		public override string ToString(int tabs) => "Single PKey: " + base.ToString(tabs);
	}
}