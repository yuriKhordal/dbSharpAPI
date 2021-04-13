using System;
using System.Collections.Generic;
using System.Text;

namespace dbAPI {
	///<summary>Represents a cell in the database.</summary>
	public interface ICell : IEquatable<ICell>, ITabbedString, ICloneable {
		///<summary>The column of the cell.</summary>
		IColumn Column { get; }
		///<summary>The value of the cell.</summary>
		IDbValue Value { get; set; }

		///<inheritdoc cref="ICloneable.Clone"/>
		new ICell Clone();
	}

	///<inheritdoc cref="ICell"/>
	public class Cell : ICell, IEquatable<Cell> {
		///<inheritdoc cref="ICell.Column"/>
		protected readonly IColumn column;

		public virtual IColumn Column => column;
		public virtual IDbValue Value { get; set; }

		///<summary>Initializes a new empty cell with a column.</summary>
		///<param name="column">The column of the cell.</param>
		public Cell(IColumn column) => this.column = column;
		///<summary>Initializes a new empty cell with a column.</summary>
		///<param name="column">The column of the cell.</param>
		///<param name="value">The value of the cell.</param>
		public Cell(IColumn column, IDbValue value) =>
			(this.column, Value) = (column, value.Clone());
		///<summary>Initializes a new empty cell with a column.</summary>
		///<param name="column">The column of the cell.</param>
		///<param name="value">The value of the cell.</param>
		public Cell(IColumn column, object value) =>
			(this.column, Value) = (column, new DbValue(value, column.Type));

		public static bool operator ==(Cell left, ICell right) =>
			left is null ? right is null : left.Equals(right);
		public static bool operator !=(Cell left, ICell right) => !(left == right);

		public virtual Cell Clone() => new Cell(Column, Value.Clone());
		ICell ICell.Clone() => this.Clone();
		object ICloneable.Clone() => this.Clone();

		public virtual bool Equals(Cell other) => Equals(other as ICell);
		public virtual bool Equals(ICell other) =>
			Object.ReferenceEquals(this, other) ? true : !(other is null) &&
			other.Column.Equals(Column) && other.Value.Equals(Value);
		public override bool Equals(object obj) => Equals(obj as ICell);
		public override int GetHashCode() => HashCode.Combine(Column, Value);
		public override string ToString() => ToString(0);//$"Cell {{\n\t{Value}\n\t{Column}\n}}";
		public virtual string ToString(int tabs) {
			StringBuilder builder = new StringBuilder();
			builder.Append("Cell {\n");
			builder.Append('\t', tabs + 1).Append(Column.ToString(tabs + 1)).Append('\n');
			builder.Append('\t', tabs + 1).Append(Value.ToString(tabs + 1)).Append('\n');
			builder.Append('\t', tabs).Append('}');
			return builder.ToString();
		}
	}
}
