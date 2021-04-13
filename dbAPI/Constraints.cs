using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dbAPI {
	///<summary>Represents a constraint type.</summary>
	public interface IConstraintType : IUnique, ITabbedString, IEquatable<IConstraintType> {}

	///<inheritdoc cref="IConstraintType"/>
	public class ConstraintType : Unique, IConstraintType {
		///<summary>Check for specified condition before allowing to insert a value.</summary>
		public static readonly ConstraintType CHECK = new ConstraintType(1, "CHECK");
		///<summary>If no value given, assigns a default value.</summary>
		public static readonly ConstraintType DEFAULT = new ConstraintType(2, "Default");
		///<summary>A key from a different table. Has to match an existing value in the other table.</summary>
		public static readonly ConstraintType FOREIGN_KEY = new ConstraintType(3, "FOREIGN KEY");
		///<summary>Used to create and retrieve data from the database very quickly.</summary>
		public static readonly ConstraintType INDEX = new ConstraintType(4, "INDEX");
		///<summary>Don't allow null values.</summary>
		public static readonly ConstraintType NOT_NULL = new ConstraintType(5, "NOT NULL");
		///<summary>UNIQUE and NOT_NULL, used to uniquely identify a row in a table.</summary>
		public static readonly ConstraintType PRIMARY_KEY = new ConstraintType(6, "PRIMARY KEY");
		///<summary>Ensures that the value doesn't exist already.</summary>
		public static readonly ConstraintType UNIQUE = new ConstraintType(7, "UNIQUE");

		///<inheritdoc cref="Name"/>
		protected readonly string name;

		///<summary>The name of the type.</summary>
		public string Name => name;

		///<inheritdoc cref="Unique(int)"/>
		public ConstraintType(int id, string name = null) : base(id) {
			this.name = name;
		}

		public static bool operator ==(ConstraintType left, IConstraintType right) =>
			left is null ? right is null : left.Equals(right);
		public static bool operator !=(ConstraintType left, IConstraintType right) =>
			!(left == right);
		public static bool operator ==(ConstraintType left, IUnique right) =>
			left is null ? right is null : left.Equals(right);
		public static bool operator !=(ConstraintType left, IUnique right) =>
			!(left == right);

		public virtual bool Equals(IConstraintType other) => other?.ID == ID;
		public override bool Equals(IUnique other) => Equals(other as IConstraintType);
		public override bool Equals(object obj) =>
			Object.ReferenceEquals(this, obj) ? true : obj is null ? false :
			obj is IConstraintType type ? Equals(type) : obj is int num ?
			num == id : false;
		public override int GetHashCode() => base.GetHashCode();
		public override string ToString() => ToString(0);
		public override string ToString(int tabs) =>
			name is null ? $"Type({id})" : $"Type({id}, {Name})";
	}

	///<summary>Represents a single column constraint in a table, immutable.</summary>
	public interface IConstraint : IEquatable<IConstraint>, ITabbedString{
		///<summary>The name of the constraint(optional).</summary>
		string Name { get; set; }
		///<summary>The type of the constraint.</summary>
		IConstraintType Type { get; }
		///<summary>The SQL representation of the constraint.</summary>
		string SqlString { get; }
	}

	///<summary>Represents a primary key constraint.</summary>
	public interface IPkConstraint : IConstraint, IReadOnlyList<IColumn>,
			IEquatable<IPkConstraint> {
		///<summary>The column at a specified index.</summary>
		///<param name="index">The index of the column.</param>
		///<returns>The column at the given index.</returns>
		new IColumn this[int index] { get; }
		///<summary>The column with a specified name.</summary>
		///<param name="name">The name of the column.</param>
		///<returns>The column with the given name.</returns>
		IColumn this[string name] { get; }

		///<summary>The columns that are primary keys.</summary>
		IColumn[] KeyColumns { get; }
		///<summary>The amount of columns int the primary key.</summary>
		new int Count { get; }

		///<summary>Check whether a given column exists.</summary>
		///<param name="column">The column.</param>
		///<returns>True if the column is present, false otherwise.</returns>
		bool HasColumn(IColumn column);
		///<summary>Check whether a column with the given name exists.</summary>
		///<param name="column">The column name.</param>
		///<returns>True if the column is present, false otherwise.</returns>
		bool HasColumn(string column);

		IColumn IReadOnlyList<IColumn>.this[int index] => this[index];
		int IReadOnlyCollection<IColumn>.Count => Count;
	}

	///<inheritdoc cref="IConstraint"/>
	public abstract class Constraint : IConstraint {
		///<summary>A nameless basic primary key constraint.</summary>
		public static readonly BasicPrimaryKeyConstraint BASIC_PRIMARY_KEY_CONSTRAINT = new BasicPrimaryKeyConstraint();
		///<summary>A nameless not null constraint.</summary>
		public static readonly NotNullConstraint NOT_NULL = new NotNullConstraint();
		///<summary>A nameless unique constraint.</summary>
		public static readonly UniqueConstraint UNIQUE = new UniqueConstraint();

		///<inheritdoc cref="Type"/>
		protected IConstraintType type;

		public virtual string Name { get; set; }
		public virtual IConstraintType Type => type;
		public abstract string SqlString { get; }

		///<summary>Initializes a new constraint with a type and an optional name.</summary>
		///<param name="type">The type of the constraint.</param>
		///<param name="name">The name of the constraint(optional).</param>
		public Constraint(IConstraintType type, string name = null) =>
			(this.type, Name) = (type, name);

		public static bool operator ==(Constraint left, IConstraint right) =>
			left is null ? right is null : left.Equals(right);
		public static bool operator !=(Constraint left, IConstraint right) =>
			!(left == right);

		public virtual bool Equals(IConstraint other) =>
			Object.ReferenceEquals(this, other) || 
			(Name == other?.Name && Type.Equals(other.Type));
		public override bool Equals(object obj) => Equals(obj as IConstraint);
		public override int GetHashCode() => HashCode.Combine(Name, Type);
		public override string ToString() => ToString(0);
		public virtual string ToString(int tabs) =>
			$"Constraint({Name}, {type.ToString(tabs)})";
	}

	///<summary>Represents a check constraint.</summary>
	public class CheckConstraint : Constraint {
		///<summary>The condition for the check.</summary>
		protected string condition;

		///<inheritdoc cref="condition"/>
		public virtual string Condition => condition;
		///<summary>The SQL representation of the constraint.</summary>
		///<remarks>Return as <c>"CHECK (<i>Condition</i>)"</c>.</remarks>
		public override string SqlString => $"CHECK ({Condition})";

		///<summary>Initialize a new check constraint with a condition.</summary>
		///<param name="condition">The condition for the constraint, for example <c>"Age >= 21"</c>.</param>
		///<param name="name">The name of the constraint(optional).</param>
		public CheckConstraint(string condition, string name = null) :
			base(ConstraintType.CHECK, name) => this.condition = condition;

		public override bool Equals(IConstraint other) =>
			Object.ReferenceEquals(this, other) ||
			(base.Equals(other) && (other as CheckConstraint)?.Condition == Condition);
		public override int GetHashCode() =>
			HashCode.Combine(base.GetHashCode(), Condition);
		public override string ToString(int tabs) => Name is null ?
			$"CheckConstraint({Condition})" : $"CheckConstraint({Name}, {Condition})";
	}

	///<summary>Represents a default constraint.</summary>
	public class DefaultConstraint : Constraint {
		///<summary>The default value.</summary>
		protected string defaultValue;

		///<inheritdoc cref="defaultValue"/>
		public virtual string DefaultValue => default;
		///<summary>The SQL representation of the constraint.</summary>
		///<remarks>Return as <c>"DEFALUT <i>value</i>"</c>.</remarks>
		public override string SqlString => $"DEFAULT {DefaultValue}";

		///<summary>Initialize a new default constraint with a value.</summary>
		///<param name="defaultValue">The default value for the constraint.</param>
		///<param name="name">The name of the constraint(optional).</param>
		public DefaultConstraint(string defaultValue, string name = null) :
			base(ConstraintType.DEFAULT, name) => this.defaultValue = defaultValue;

		public override bool Equals(IConstraint other) =>
			Object.ReferenceEquals(this, other) || (base.Equals(other) &&
			(other as DefaultConstraint)?.DefaultValue == DefaultValue);
		public override int GetHashCode() =>
			HashCode.Combine(base.GetHashCode(), DefaultValue);
		public override string ToString(int tabs) => Name is null ?
			$"DefaultConstraint({DefaultValue})" :
			$"DefaultConstraint({Name}, {DefaultValue})";
	}

	///<summary>Represents a foreign key constraint.</summary>
	public class ForeignKeyConstraint : Constraint {
		///<summary>The table and column the key references.</summary>
		protected string reference;
		///<summary>The name of the column this constraint is in.</summary>
		protected string column;

		///<inheritdoc cref="reference"/>
		public virtual string Reference => reference;
		///<inheritdoc cref="column"/>
		public virtual string Column => column;
		///<summary>The SQL representation of the constraint.</summary>
		///<remarks>Return as <c>"FOREIGN KEY (<i>column</i>) REFERENCES <i>table</i>(<i>column</i>)"</c>.</remarks>
		public override string SqlString => $"FOREIGN KEY ({Column}) REFERENCES {Reference}";

		///<summary>Initialize a new foreign key constraint with a column and a referenced column.</summary>
		///<param name="column">The name of the column this constraint is in.</param>
		///<param name="reference">The column this constraint references, including its table.</param>
		///<param name="name">The name of the constraint(optional).</param>
		public ForeignKeyConstraint(string column, string reference, string name = null) :
			base(ConstraintType.FOREIGN_KEY, name) =>
			(this.column, this.reference) = (column, reference);

		public override bool Equals(IConstraint other) =>
			Object.ReferenceEquals(this, other) ||
			(base.Equals(other) && other is ForeignKeyConstraint key ?
			key.Column == Column && key.Reference == Reference : false);
		public override int GetHashCode() =>
			HashCode.Combine(base.GetHashCode(), Column, Reference);
		public override string ToString(int tabs) => Name is null ?
			$"ForeignKeyConstraint({Column}, {Reference})" :
			$"ForeignKeyConstraint({Name}, {Column}, {Reference})";
	}

	///<summary>Represents an index constraint.</summary>
	public class IndexConstraint : Constraint {
		///<summary>The table and columns of the index.</summary>
		protected string target;
		///<summary>Whether the index rows are unique.</summary>
		protected bool unique;

		///<inheritdoc cref="target"/>
		public virtual string Target => target;
		///<inheritdoc cref="unique"/>
		public virtual bool Unique => unique;
		///<summary>The SQL representation of the constraint.</summary>
		///<remarks>Return as <c>"FOREIGN KEY (<i>column</i>) REFERENCES <i>table</i>(<i>column</i>)"</c>.</remarks>
		public override string SqlString => (unique ? "UNIQUE " : "") +
			$"INDEX ({Name}) ON {Target}";

		///<summary>Initialize a new index constraint with a name, target and whether it's unique.</summary>
		///<param name="target">The target table and columns.</param>
		///<param name="unique">Whether the index rows are unique.</param>
		///<param name="name">The name of the index.</param>
		public IndexConstraint(string target, bool unique, string name) :
			base(ConstraintType.INDEX, name) =>
			(this.target, this.unique) = (target, unique);

		public override bool Equals(IConstraint other) =>
			Object.ReferenceEquals(this, other) || (base.Equals(other)
			&& other is IndexConstraint index && index.Target == Target &&
			index.Unique == Unique);
		public override int GetHashCode() =>
			HashCode.Combine(base.GetHashCode(), Target, Unique);
		public override string ToString(int tabs) =>
			"IndexConstraint(" + (Name is null ? "" : Name + ", " ) + 
			$"{Target}" + (this.Unique ? ", unique" : "") + ")";
	}

	///<summary>Represents a not null constraint.</summary>
	public class NotNullConstraint : Constraint {
		///<summary>The SQL representation of the constraint.</summary>
		///<remarks>Return as <c>"NOT NULL"</c>.</remarks>
		public override string SqlString => $"NOT NULL";

		///<summary>Initialize a new not null constraint.</summary>
		///<param name="name">The name of the constraint(optional).</param>
		public NotNullConstraint(string name = null) :
			base(ConstraintType.NOT_NULL, name) {}

		public override string ToString(int tabs) => $"NotNullConstraint({Name})";
	}

	///<summary>Represents a primary key constraint.</summary>
	public class PrimaryKeyConstraint : Constraint, IPkConstraint,
			IEquatable<PrimaryKeyConstraint> {
		///<summary>The columns that are primary keys.</summary>
		protected IColumn[] keyColumns;
		///<summary>The SQL representation of the constraint.</summary>
		protected string sqlString;

		public virtual IColumn this[int index] => KeyColumns[index];
		public virtual IColumn this[string name] { get {
			foreach (IColumn col in KeyColumns) if (col.Name == name) return col;
			throw new ArgumentException($"No column '{name}' in the constraint", "name");
		} }

		///<inheritdoc cref="keyColumns"/>
		public virtual IColumn[] KeyColumns => (IColumn[])keyColumns.Clone();
		public virtual int Count => keyColumns.Length;
		///<summary>The SQL representation of the constraint.</summary>
		///<remarks>Return as <c>"CONSTRAINT <i>[name]</i> PRIMARY KEY (<i>columns</i>)"</c>.</remarks>
		public override string SqlString => sqlString;

		///<summary>Initialize a new primary key constraint with key columns.</summary>
		///<param name="keyColumns">The columns of the key.</param>
		public PrimaryKeyConstraint(params IColumn[] keyColumns) :
			this(null, keyColumns) {
		}
		///<summary>Initialize a new primary key constraint with a name and key columns.</summary>
		///<param name="name">The name of the constraint(optional).</param>
		///<param name="keyColumns">The columns of the key.</param>
		public PrimaryKeyConstraint(string name, params IColumn[] keyColumns) :
			base(ConstraintType.PRIMARY_KEY, name) {
			this.keyColumns = keyColumns;
			sqlString = GetSqlString();
		}

		public virtual bool HasColumn(IColumn column) {
			foreach (IColumn col in keyColumns) if (col == column) return true;
			return false;
		}
		public virtual bool HasColumn(string column) {
			foreach (IColumn col in keyColumns) if (col.Name == column) return true;
			return false;
		}

		///<summary>The SQL representation of the constraint.</summary>
		///<remarks>Return as <c>"CONSTRAINT <i>[name]</i> PRIMARY KEY (<i>columns</i>)"</c>.</remarks>
		public string GetSqlString() {
			if (!(sqlString is null)) return sqlString;
			StringBuilder builder = new StringBuilder();
			builder.Append("CONSTRAINT ");
			if (!(Name is null)) builder.Append(Name).Append(' ');
			builder.Append("PRIMARY KEY (");

			for (int i = 0; i < KeyColumns.Length; i++) {
				if (i != 0) builder.Append(", ");
				builder.Append(KeyColumns[i].Name);
			}
			builder.Append(")");

			return builder.ToString();
		}

		public virtual IEnumerator<IColumn> GetEnumerator() =>
			((IEnumerable<IColumn>)keyColumns).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => keyColumns.GetEnumerator();
		
		public virtual bool Equals(PrimaryKeyConstraint other) => Equals(other as IPkConstraint);
		public virtual bool Equals(IPkConstraint other) {
			if (Object.ReferenceEquals(this, other)) return true;
			if (other is null || other.KeyColumns.Length != KeyColumns.Length) return false;
			for (int i = 0; i < KeyColumns.Length; i++)
				if (!other.KeyColumns[i].Equals(KeyColumns[i])) return false;
			return true;
		}
		public override bool Equals(IConstraint other) =>
			Object.ReferenceEquals(this, other) ||
			(base.Equals(other) && Equals(other as PrimaryKeyConstraint));
		public override int GetHashCode() {
			HashCode hash = new HashCode();
			hash.Add(Name);
			foreach (IColumn col in KeyColumns) hash.Add(col);
			return hash.ToHashCode();
		}
		public override string ToString(int tabs) {
			StringBuilder builder = new StringBuilder();
			builder.Append("PrimaryKeyConstraint {");
			if (!(Name is null)) builder.Append($" ({Name})");

			foreach (IColumn col in KeyColumns) builder.Append('\n').Append('\t', tabs + 1)
				.Append(col.ToString(tabs + 1));
			builder.Append('\n').Append('\t', tabs).Append('}');

			return builder.ToString();
		}

	}

	///<summary>Represents a basic primary key constraint.</summary>
	///<remarks>This constraint is the basic constraint inside
	///the column object as opposed to <see cref="PrimaryKeyConstraint"/>
	///which is the more general constraint for the table
	///with all the columns inside it.</remarks>
	public class BasicPrimaryKeyConstraint : Constraint {
		///<summary>The SQL representation of the constraint.<br/></summary>
		///<remarks>Return as <c>"PRIMARY KEY"</c>.</remarks>
		public override string SqlString => "PRIMARY KEY";

		///<summary>Initialize a new basic primary key constraint.</summary>
		///<param name="name">The name of the constraint(optional).</param>
		public BasicPrimaryKeyConstraint(string name = null) :
			base(ConstraintType.PRIMARY_KEY, name) {}

		public override string ToString(int tabs) => $"BasicPrimaryKeyConstraint({Name})";
	}

	///<summary>Represents a unique constraint.</summary>
	public class UniqueConstraint : Constraint {
		///<summary>The SQL representation of the constraint.<br/></summary>
		///<remarks>Return as <c>"UNIQUE"</c></remarks>
		public override string SqlString => "UNIQUE";

		///<summary>Initialize a new unique constraint.</summary>
		///<param name="name">The name of the constraint(optional).</param>
		public UniqueConstraint(string name = null) :
			base(ConstraintType.UNIQUE, name) { }

		public override string ToString(int tabs) => $"UniqueConstraint({Name})";
	}
}