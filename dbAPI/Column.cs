using System;
using System.Collections.Generic;
using System.Text;

namespace dbAPI {
	///<summary>Represents a column in a database.</summary>
	public interface IColumn : IEquatable<IColumn>, ITabbedString, ICloneable {
		///<summary>The index of this column.</summary>
		uint Index { get; }
		///<summary>The name of this column.</summary>
		string Name { get; }
		///<summary>The type of data that this column accepts.</summary>
		IDataType Type { get; }
		///<summary>All the constraints of this column.</summary>
		IConstraint[] Constraints { get; }
		///<summary>The number of constraints in this column.</summary>
		int ConstraintsCount { get; }
		
		///<summary>Check whether this column has a specific type of constraint.</summary>
		///<param name="type">The type of constraint to search in the column.</param>
		///<returns>True if there exists a constraint of the given type.</returns>
		bool HasConstraint(IConstraintType type);
		/// <summary>Get the constraint at a specified index.</summary>
		/// <param name="index">The index of the constraint inside the column.</param>
		/// <returns>The constraint at index.</returns>
		IConstraint GetConstraint(int index);
		///<inheritdoc cref="ICloneable.Clone"/>
		new IColumn Clone();
	}

	///<inheritdoc cref="IColumn"/>
	public class Column : IColumn, IEquatable<Column> {
		///<inheritdoc cref="Index"/>
		protected readonly uint index;
		///<inheritdoc cref="Name"/>
		protected readonly string name;
		///<inheritdoc cref="Type"/>
		protected readonly IDataType type;
		///<inheritdoc cref="Constraints"/>
		protected readonly IConstraint[] constraints;

		public virtual uint Index => index;
		public virtual string Name => name;
		public virtual IDataType Type => type;
		public virtual IConstraint[] Constraints => constraints;
		public virtual int ConstraintsCount => constraints.Length;

		///<summary>Initialize a new column with a name, index, and data type.</summary>
		///<param name="name">The name of the column.</param>
		///<param name="index">The index of the column.</param>
		///<param name="type">The type of data in the column.</param>
		public Column(string name, uint index, IDataType type) :
			this(name, index, type, new IConstraint[0]) { }
		///<summary>Initialize a new column with a name, index, data type, and constraints.</summary>
		///<param name="name">The name of the column.</param>
		///<param name="index">The index of the column.</param>
		///<param name="type">The type of data in the column.</param>
		public Column(string name, uint index, IDataType type,
				params IConstraint[] constraints) {
			(this.name, this.index, this.type) = (name, index, type);
			this.constraints = new IConstraint[constraints.Length];
			for (int i = 0; i < constraints.Length; i++)
				this.constraints[i] = constraints[i];
		}

		public static bool operator ==(Column left, IColumn right) =>
			left is null ? right is null : left.Equals(right);
		public static bool operator !=(Column left, IColumn right) {
			return !(left == right);
		}

		public virtual bool HasConstraint(IConstraintType type) {
			foreach (IConstraint constraint in Constraints)
				if (constraint.Type == type) return true;
			return false;
		}
		public virtual IConstraint GetConstraint(int index) => constraints[index];
		public virtual Column Clone() => new Column(Name, Index, Type, constraints);
		IColumn IColumn.Clone() => this.Clone();
		object ICloneable.Clone() => this.Clone();

		public virtual bool Equals(Column other) => Equals(other as IColumn);
		public virtual bool Equals(IColumn other) {
			if (Object.ReferenceEquals(this, other)) return true;
			if (other.Index != Index || other.Name != Name ||
				!other.Type.Equals(Type)) return false;
			/*IEnumerator<IConstraint>
				enum1 = ((IEnumerable<IConstraint>)Constraints).GetEnumerator(),
				enum2 = ((IEnumerable<IConstraint>)other.Constraints).GetEnumerator();
			bool move1 = enum1.MoveNext(), move2 = enum2.MoveNext();
			while (move1 && move2) {
				if (!enum1.Current.Equals(enum2.Current)) return false;
				move1 = enum1.MoveNext(); move2 = enum2.MoveNext();
			}
			return move1 == move2;*/
			return Constraints.MemberEquals(other.Constraints);
		}
		public override bool Equals(object obj) => obj is IColumn column ?
			Equals(column) : false;
		public override int GetHashCode() {
			HashCode hash = new HashCode();
			hash.Add(Index);
			hash.Add(Name);
			hash.Add(Type);
			foreach (IConstraint constraint in Constraints)
				hash.Add(constraint);

			return hash.ToHashCode();
		}
		public override string ToString() => ToString(0);
		public virtual string ToString(int tabs) {
			StringBuilder builder = new StringBuilder();
			builder.Append($"Column {{ {Index}, {Name}, {Type.ToString(tabs)}");

			foreach (IConstraint constraint in Constraints)
				builder.Append("\n\t").Append(constraint.ToString(tabs + 1));
			if (constraints.Length != 0) builder.Append('\n').Append('\t', tabs);
			builder.Append('}');

			return builder.ToString();
		}
	}
}
