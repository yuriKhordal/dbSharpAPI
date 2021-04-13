using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dbAPI {
	///<summary>Represents a table in a database.</summary>
	public interface ITable : IEnumerable<IRow>, IEquatable<ITable>, ITabbedString {
		///<summary>The row at the specified index.</summary>
		///<param name="index">The index of the row.</param>
		///<returns>A row at index.</returns>
		IRow this[int index] { get; }
		///<summary>The row with a specified primary key.</summary>
		///<param name="key">The primary key of the row.</param>
		///<returns>A row with the primary key.</returns>
		IRow this[IPrimaryKey key] { get; }
		///<summary>The name of the table.</summary>
		string Name { get; }
		///<summary>The primary key constraint of the table.</summary>
		IPkConstraint PrimaryKey { get; }
		///<summary>The columns of the table.</summary>
		IColumn[] Columns { get; }
		///<summary>The amount of columns in the table.</summary>
		int ColumnCount { get; }
		///<summary>The index constraints of the table.</summary>
		IConstraint[] Indices { get; }
		///<summary>The amount of indexes in the table.</summary>
		int IndicesCount { get; }
		///<summary>The check constraints of the table.</summary>
		IConstraint[] Checks { get; }
		///<summary>The amount of checks in the table.</summary>
		int ChecksCount { get; }
		///<summary>Get the check constraint at the specified index.</summary>
		///<param name="index">The index of the check.</param>
		///<returns>A check constraint at index.</returns>
		IConstraint GetCheck(int index);
		///<summary>Get the column at the specified index.</summary>
		///<param name="index">The index of the column.</param>
		///<returns>A column at index.</returns>
		IColumn GetColumn(int index);
		///<summary>Get the column with the specified column name.</summary>
		///<param name="column">The name of the column.</param>
		///<returns>A column with the name.</returns>
		IColumn GetColumn(string column);
		///<summary>Get the index constraint at the specified numerical index.</summary>
		///<param name="index">The numerical index of the constraint.</param>
		///<returns>An index constraint at index(location).</returns>
		IConstraint GetIndex(int index);

		///<summary>Execute the INSERT command on the table, with the specified row.</summary>
		///<param name="row">The row to insert.</param>
		void Insert(IRow row);
		///<summary>Execute the INSERT command on the table, with the specified rows.</summary>
		///<param name="rows">The rows to insert.</param>
		void Insert(IEnumerable<IRow> rows);
		///<summary>Execute the INSERT command on the table, with the specified rows.</summary>
		///<param name="rows">The rows to insert.</param>
		void Insert(params IRow[] rows);
		///<summary>Execute the SELECT command on the table with the specified columns.</summary>
		///<param name="columns">The columns to select.</param>
		///<returns>A database reader of the resulting rows.</returns>
		IDbReader Select(params IColumn[] columns);
		///<summary>Execute the SELECT command on the table with the specified columns,
		/// and a WHERE clause.</summary>
		///<param name="where">The WHERE clause of the command.</param>
		///<param name="columns">The columns to select.</param>
		///<returns>A database reader of the resulting rows.</returns>
		IDbReader Select(string where, params IColumn[] columns);
		/// <summary>Execute the SELECT command with *(every) column.</summary>
		/// <returns>A database reader of the resulting rows.</returns>
		IDbReader SelectAll();
		/// <summary>Execute the SELECT command with *(every) column,
		/// and a specified WHERE clause.</summary>
		/// <param name="where">The WHERE clause of the command.</param>
		/// <returns>A database reader of the resulting rows.</returns>
		IDbReader SelectAll(string where);
		///<summary>Execute the UPDATE command with the values from a specified row.</summary>
		///<param name="row">The values for the UPDATE command.</param>
		///<remarks>Use with caution! This command will overwrite every row with
		/// the specified row!</remarks>
		void Update(IRow row);
		///<summary>Execute the UPDATE command with the values from a specified row,
		/// and a WHERE clause.</summary>
		///<param name="row">The values for the UPDATE command.</param>
		///<param name="where">The WHERE clause.</param>
		void Update(IRow row, string where);
		/// <summary>Execute the DELETE command with the specified WHERE clause.</summary>
		/// <param name="where">The WHERE clause.</param>
		void Delete(string where);
	}

	///<inheritdoc cref="ITable"/>
	public abstract class AbsTable : ITable, IEquatable<AbsTable> {
		///<inheritdoc cref="Name"/>
		protected readonly string name;
		///<inheritdoc cref="Columns"/>
		protected readonly IColumn[] columns;
		///<inheritdoc cref="PrimaryKey"/>
		protected readonly IPkConstraint pk;
		///<inheritdoc cref="Indices"/>
		protected readonly IConstraint[] indices;
		///<inheritdoc cref="Checks"/>
		protected readonly IConstraint[] checks;

		///<summary>Initialize a new Table with a specified name, indices,
		/// check constraints, and columns.</summary>
		///<param name="name">The name of the table.</param>
		///<param name="indices">The index constraints of the table.</param>
		///<param name="checks">The check constraints of the table.</param>
		///<param name="columns">The columns of the table.</param>
		public AbsTable(string name, IConstraint[] indices, IConstraint[] checks,
				params IColumn[] columns) {
			this.name = name;
			this.columns = columns is null ? new IColumn[0] : (IColumn[])columns.Clone();
			this.indices = indices is null ? new IConstraint[0] : (IConstraint[])indices.Clone();
			this.checks = checks is null ? new IConstraint[0] : (IConstraint[])checks.Clone();
			/*List<IColumn> keys = new List<IColumn>(columns.Length);
			foreach (IColumn col in columns)
				if (col.HasConstraint(ConstraintType.PRIMARY_KEY)) keys.Add(col);
			pk = new PrimaryKeyConstraint(keys.ToArray());*/

			pk = new PrimaryKeyConstraint(Utilities.ConvertIf(columns, (col) =>
				(col.HasConstraint(ConstraintType.PRIMARY_KEY), col)));
		}
		///<summary>Initialize a new Table with a specified name, and columns.</summary>
		///<param name="name">The name of the table.</param>
		///<param name="columns">The columns of the table.</param>
		public AbsTable(string name, params IColumn[] columns) :
			this(name, null, null, columns) { }

		public static bool operator ==(AbsTable left, ITable right) =>
			Equals(left, right);
		public static bool operator !=(AbsTable left, ITable right) =>
			!(left == right);

		public abstract IRow this[int index] { get; }
		public abstract IRow this[IPrimaryKey key] { get; }
		public virtual string Name => name;
		public virtual IPkConstraint PrimaryKey => pk;
		public virtual IColumn[] Columns => (IColumn[])columns.Clone();
		public virtual int ColumnCount => columns.Length;
		public virtual IConstraint[] Indices => (IConstraint[])indices.Clone();
		public virtual int IndicesCount => indices.Length;
		public virtual IConstraint[] Checks => (IConstraint[])checks.Clone();
		public virtual int ChecksCount => indices.Length;

		public abstract void Delete(string where);
		public virtual IConstraint GetCheck(int index) => checks[index];
		public virtual IColumn GetColumn(int index) => columns[index];
		public virtual IColumn GetColumn(string column) {
			foreach (IColumn col in columns)
				if (col.Name == column) return col;
			throw new ArgumentException($"No column '{column}' in the table", "column");
		}
		public virtual IConstraint GetIndex(int index) => indices[index];
		public abstract void Insert(IRow row);
		public abstract void Insert(IEnumerable<IRow> rows);
		public abstract void Insert(params IRow[] rows);
		public abstract IDbReader Select(params IColumn[] columns);
		public abstract IDbReader Select(string where, params IColumn[] columns);
		public abstract IDbReader SelectAll();
		public abstract IDbReader SelectAll(string where);
		public abstract void Update(IRow row);
		public abstract void Update(IRow row, string where);
		public abstract IEnumerator<IRow> GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public virtual bool Equals(AbsTable other) => Equals(other as ITable);
		public virtual bool Equals(ITable other) {
			Console.WriteLine("Equal");
			if (ReferenceEquals(this, other)) return true;
			if (other.Name != Name || 
				indices.Length != other.IndicesCount ||
				checks.Length != other.ChecksCount ||
				columns.Length != other.ColumnCount) return false;

			for (int i = 0; i < checks.Length; i++)
				if (!checks[i].Equals(other.GetCheck(i))) return false;
			for (int i = 0; i < indices.Length; i++)
				if (!indices[i].Equals(other.GetIndex(i))) return false;
			for (int i = 0; i < columns.Length; i++)
				if (!columns[i].Equals(other.GetColumn(i))) return false;
			//Theoretically useless, if all columns are equal, then the key is too,
			//but just in case.
			return PrimaryKey.Equals(other.PrimaryKey);
		}
		public override bool Equals(object obj) => Equals(obj as ITable);
		public override int GetHashCode() =>
			HashCode.Combine(name, columns.Hash(), pk, indices.Hash(), checks.Hash());
		public override string ToString() => ToString(0);
		public virtual string ToString(int tabs) {
			StringBuilder builder = new StringBuilder();
			builder.Append($"Table '{name}' {{").Append('\n');
			foreach (IConstraint index in indices)
				builder.Append('\t', tabs + 1).Append(index.ToString(tabs + 1)).Append('\n');
			foreach (IConstraint check in checks)
				builder.Append('\t', tabs + 1).Append(check.ToString(tabs + 1)).Append('\n');
			foreach (IColumn col in columns)
				builder.Append('\t', tabs + 1).Append(col.ToString(tabs + 1)).Append('\n');
			builder.Append('\t', tabs).Append('}');
			return builder.ToString();
		}
	}

	///<inheritdoc cref="ITable"/>
	public class Table : AbsTable, IEquatable<Table> {
		///<summary>The handle to the database.</summary>
		protected IDbHelper helper;

		public override IRow this[int index] => SelectAll()[index];
		public override IRow this[IPrimaryKey key] { get {
				StringBuilder where = new StringBuilder();

				foreach (ICell cell in (IEnumerable<ICell>)key)
					where.Append(cell.Column.Name).Append(" = ")
					.Append(helper.DbValueToString(cell.Value))
					.Append(", ");
				if (where.Length > 0) where.Remove(where.Length - 2, 2);

				IDbReader result = SelectAll(where.ToString());
				return result.MoveNext() ? result.Current : null;
		} }

		///<summary>Initialize a new Table with a specified name, indices,
		/// check constraints, and columns.</summary>
		///<param name="name">The name of the table.</param>
		///<param name="indices">The index constraints of the table.</param>
		///<param name="checks">The check constraints of the table.</param>
		///<param name="columns">The columns of the table.</param>
		protected Table(string name, IConstraint[] indices, IConstraint[] checks,
			IColumn[] columns) : base(name, indices, checks, columns) { }
		///<summary>Initialize a new Table with a specified name, indices,
		/// check constraints, a helper, and columns.</summary>
		///<param name="name">The name of the table.</param>
		///<param name="indices">The index constraints of the table.</param>
		///<param name="checks">The check constraints of the table.</param>
		///<param name="helper">The handle to the database.</param>
		///<param name="columns">The columns of the table.</param>
		public Table(string name, IDbHelper helper, IConstraint[] indices,
			IConstraint[] checks, params IColumn[] columns) :
			this(name, indices, checks, columns) => this.helper = helper;
		///<summary>Initialize a new Table with a specified name, a helper,
		/// and columns.</summary>
		///<param name="name">The name of the table.</param>
		///<param name="helper">The handle to the database.</param>
		///<param name="columns">The columns of the table.</param>
		public Table(string name, IDbHelper helper, params IColumn[] columns) :
			this(name, helper, null, null, columns) { }

		public override void Insert(IRow row) =>
			helper.Insert(this, row.Columns, row.Values);
		public override void Insert(IEnumerable<IRow> rows) {
			foreach (IRow row in rows) Insert(row);
		}
		public override void Insert(params IRow[] rows) => Insert((IEnumerable<IRow>)rows);
		public override void Update(IRow row) => helper.Update(this, row);
		public override void Update(IRow row, string where) =>
			helper.Update(this, row, where);
		public override IDbReader Select(params IColumn[] columns) =>
			helper.Select(this, columns);
		public override IDbReader Select(string where, params IColumn[] columns) =>
			helper.Select(this, where, columns);
		public override IDbReader SelectAll() => helper.SelectAll(this);
		public override IDbReader SelectAll(string where) => helper.SelectAll(this, where);
		public override void Delete(string where) => helper.Delete(this, where);
		
		public virtual bool Equals(Table other) => Equals(other as ITable);
		public override IEnumerator<IRow> GetEnumerator() => SelectAll().GetEnumerator();
		public override string ToString(int tabs) {
			StringBuilder builder = new StringBuilder();
			builder.Append($"Table '{name}' {{").Append('\n');
			builder.Append('\t', tabs + 1).Append(helper.ToString(tabs + 1)).Append('\n');
			foreach (IConstraint index in indices)
				builder.Append('\t', tabs + 1).Append(index.ToString(tabs + 1)).Append('\n');
			foreach (IConstraint check in checks)
				builder.Append('\t', tabs + 1).Append(check.ToString(tabs + 1)).Append('\n');
			foreach (IColumn col in columns)
				builder.Append('\t', tabs + 1).Append(col.ToString(tabs + 1)).Append('\n');
			builder.Append('\t', tabs).Append('}');
			return builder.ToString();
		}
	}

	///<summary>Represents a table that holds all data in memory.</summary>
	///<typeparam name="T">The type of rows the table holds.</typeparam>
	public class CacheTable<T> : Table where T : IRow {
		///<summary>A delegate that converts a specified row into the specific row type
		/// of this table.</summary>
		///<param name="row">The row.</param>
		///<returns>The row converted to the specific type of rows in the table.</returns>
		public delegate T converter(IRow row);
		///<summary>All the rows of the table.</summary>
		protected List<T> rows;
		///<summary>A dictionary of rows indexed by their keys.</summary>
		protected Dictionary<IBasicPKey, T> rowsDict;
		///<summary>The converter to convert <see cref="IRow"/> to T.</summary>
		protected converter convert;

		public override IRow this[int index] => rows[index];
		public override IRow this[IPrimaryKey key] => rowsDict[key];

		///<summary>Initialize a new Table with a specified name, indices,
		/// check constraints, and columns.</summary>
		///<param name="name">The name of the table.</param>
		///<param name="indices">The index constraints of the table.</param>
		///<param name="checks">The check constraints of the table.</param>
		///<param name="columns">The columns of the table.</param>
		protected CacheTable(string name, IConstraint[] indices, IConstraint[] checks,
				params IColumn[] columns) : base(name, indices, checks, columns) {
			Init();
		}
		///<summary>Initialize a new Table with a specified name, helper, converter,
		/// indices, check constraints, and columns.</summary>
		///<param name="name">The name of the table.</param>
		///<param name="helper">The handle to the database.</param>
		///<param name="convert">The converter to convert <see cref="IRow"/> to T.</param>
		///<param name="indices">The index constraints of the table.</param>
		///<param name="checks">The check constraints of the table.</param>
		///<param name="columns">The columns of the table.</param>
		public CacheTable(string name, IDbHelper helper, converter convert,
				IConstraint[] indices, IConstraint[] checks, params IColumn[] columns) :
				base(name, helper, indices, checks, columns) {
			Init();
			this.convert = convert;
		}
		///<summary>Initialize a new Table with a specified name, helper, converter,
		/// and columns.</summary>
		///<param name="name">The name of the table.</param>
		///<param name="helper">The handle to the database.</param>
		///<param name="convert">The converter to convert <see cref="IRow"/> to T.</param>
		///<param name="columns">The columns of the table.</param>
		public CacheTable(string name, IDbHelper helper, converter convert,
			params IColumn[] columns) : this(name, helper, convert, null, null, columns) { }

		///<summary>Initialize objects in construction.</summary>
		private void Init() {
			rows = new List<T>();
			rowsDict = new Dictionary<IBasicPKey, T>();
		}

		///<summary>Load all the table from the database to memory.</summary>
		public virtual void Load() {
			foreach (IRow row in SelectAll()) AddIRow(row);
		}

		///<summary>Adds a collection of rows to the table.</summary>
		///<param name="rows">The rows to add.</param>
		public virtual void Add(IEnumerable<T> rows) {
			foreach (T row in rows) Insert(row);
		}
		///<inheritdoc cref="Add(IEnumerable{T})"/>
		public virtual void Add(params T[] rows) => Add(rows as IEnumerable<T>);
		/// <summary>Add a specified row in <see cref="IRow"/> form into the table.</summary>
		/// <param name="row">The row.</param>
		protected virtual void AddIRow(IRow row) {
			T tRow = convert(row);
			rows.Add(tRow);
			rowsDict[tRow.PrimaryKey] = tRow;
		}
		/// <summary>Update a row with a specified key with a specified value.</summary>
		/// <param name="key">The key of the row to update.</param>
		/// <param name="newValue">The new value to set to row with.</param>
		public virtual void Update(IPrimaryKey key, T newValue) {
			StringBuilder where = new StringBuilder();
			
			foreach(ICell cell in (IEnumerable<ICell>)key)
				where.Append(cell.Column.Name).Append(" = ")
					.Append(helper.DbValueToString(cell.Value)).Append(", ");
			if (where.Length > 0) where.Remove(where.Length - 2, 2);

			helper.Update(this, newValue, where.ToString());
			T tRow = rowsDict[key];
			foreach (ICell cell in newValue) tRow.SetValue(cell.Column, cell.Value);
		}

		public override void Insert(IRow row) {
			base.Insert(row);
			AddIRow(row);
		}
		public override void Update(IRow row) {
			base.Update(row);
			foreach (T tRow in rows) foreach (ICell cell in row)
				tRow.SetValue(cell.Column, cell.Value);
		}
		public override void Update(IRow row, string where) {
			/*TODO: Check it, potentially stupid.
			 * How would a row gotten by the reader know it's key???*/
			foreach(IRow select in SelectAll(where)) {
				T tRow = rowsDict[select.PrimaryKey];
				foreach (ICell cell in row) tRow.SetValue(cell.Column, cell.Value);
				// ==== solution 1 ====
				/*T tRow = rowsDict[convert(select).PrimaryKey];*/
				// ==== solution 2 ====
				/*IDbValue[] keys = new IDbValue[PrimaryKey.Count];
				for (int i = 0; i < keys.Length; i++)
					keys[i] = select.GetValue(PrimaryKey[i]);
				BasicPKey key = new BasicPKey(keys);
				T tRow = rowsDict[key];*/
			}
			base.Update(row, where);
		}
		public override void Delete(string where) {
			/*TODO: Check it, potentially stupid.
			 * How would a row gotten by the reader know it's key???
			 * Same as Update.*/
			foreach (IRow row in SelectAll(where)) {
				T tRow = rowsDict[row.PrimaryKey];
				rows.Remove(tRow);
				rowsDict.Remove(tRow.PrimaryKey);
			}
			base.Delete(where);
		}

		public override IEnumerator<IRow> GetEnumerator() => new RowEnumerator<T>(rows);
	}

	public class SinglePKCacheTable<T> : Table where T : IRow {
		///<summary>A delegate that converts a specified row into the specific row type
		/// of this table.</summary>
		///<param name="row">The row.</param>
		///<returns>The row converted to the specific type of rows in the table.</returns>
		public delegate T converter(IRow row);
		///<summary>All the rows of the table.</summary>
		protected List<T> rows;
		///<summary>A dictionary of rows indexed by their keys.</summary>
		///<remarks>Since the table only has one primary key, the dictionary's key
		/// is the the value itself, not a IDbValue.</remarks>
		protected Dictionary<object, T> rowsDict;
		///<summary>The converter to convert <see cref="IRow"/> to T.</summary>
		protected converter convert;

		public override IRow this[int index] => rows[index];
		public override IRow this[IPrimaryKey key] => rowsDict[key.Value.Value];

		///<summary>Initialize a new Table with a specified name, indices,
		/// check constraints, and columns.</summary>
		///<param name="name">The name of the table.</param>
		///<param name="indices">The index constraints of the table.</param>
		///<param name="checks">The check constraints of the table.</param>
		///<param name="columns">The columns of the table.</param>
		protected SinglePKCacheTable(string name, IConstraint[] indices,
				IConstraint[] checks, params IColumn[] columns) :
				base(name, indices, checks, columns) {
			Init();
		}
		///<summary>Initialize a new Table with a specified name, helper, converter,
		/// indices, check constraints, and columns.</summary>
		///<param name="name">The name of the table.</param>
		///<param name="helper">The handle to the database.</param>
		///<param name="convert">The converter to convert <see cref="IRow"/> to T.</param>
		///<param name="indices">The index constraints of the table.</param>
		///<param name="checks">The check constraints of the table.</param>
		///<param name="columns">The columns of the table.</param>
		public SinglePKCacheTable(string name, IDbHelper helper, converter convert,
				IConstraint[] indices, IConstraint[] checks, params IColumn[] columns) :
				base(name, helper, indices, checks, columns) {
			Init();
			this.convert = convert;
		}
		///<summary>Initialize a new Table with a specified name, helper, converter,
		/// and columns.</summary>
		///<param name="name">The name of the table.</param>
		///<param name="helper">The handle to the database.</param>
		///<param name="convert">The converter to convert <see cref="IRow"/> to T.</param>
		///<param name="columns">The columns of the table.</param>
		public SinglePKCacheTable(string name, IDbHelper helper, converter convert,
			params IColumn[] columns) : this(name, helper, convert, null, null, columns) { }

		///<summary>Initialize objects in construction.</summary>
		private void Init() {
			rows = new List<T>();
			rowsDict = new Dictionary<object, T>();
		}

		///<summary>Load all the table from the database to memory.</summary>
		public virtual void Load() {
			foreach (IRow row in SelectAll()) {
				T tRow = convert(row);
				rows.Add(tRow);
				rowsDict[tRow.PrimaryKey.Value.Value] = tRow;
			}
		}

		///<summary>Adds a collection of rows to the table.</summary>
		///<param name="rows">The rows to add.</param>
		public virtual void Add(IEnumerable<T> rows) {
			foreach (T row in rows) Insert(row);
		}
		///<inheritdoc cref="Add(IEnumerable{T})"/>
		public virtual void Add(params T[] rows) => Add(rows as IEnumerable<T>);
		/// <summary>Update a row with a specified key with a specified value.</summary>
		/// <param name="key">The key of the row to update.</param>
		/// <param name="newValue">The new value to set to row with.</param>
		public virtual void Update(IPrimaryKey key, T newValue) {
			StringBuilder where = new StringBuilder();

			foreach (ICell cell in (IEnumerable<ICell>)key)
				where.Append(cell.Column.Name).Append(" = ")
					.Append(helper.DbValueToString(cell.Value)).Append(", ");
			if (where.Length > 0) where.Remove(where.Length - 2, 2);

			helper.Update(this, newValue, where.ToString());
			T tRow = rowsDict[key];
			foreach (ICell cell in newValue) tRow.SetValue(cell.Column, cell.Value);
		}

		public override void Insert(IRow row) {
			base.Insert(row);
			T tRow = convert(row);
			rows.Add(tRow);
			rowsDict[tRow.PrimaryKey.Value.Value] = tRow;
		}
		public override void Update(IRow row) {
			base.Update(row);
			foreach (T tRow in rows) foreach (ICell cell in row)
					tRow.SetValue(cell.Column, cell.Value);
		}
		public override void Update(IRow row, string where) {
			foreach (IRow select in SelectAll(where)) {
				T tRow = rowsDict[select.PrimaryKey.Value.Value];
				foreach (ICell cell in row)
					tRow.SetValue(cell.Column, cell.Value);
			}

			base.Update(row, where);
		}
		public override void Delete(string where) {
			foreach (IRow select in SelectAll(where)) {
				T tRow = rowsDict[select.PrimaryKey.Value.Value];
				rows.Remove(tRow);
				rowsDict.Remove(tRow.PrimaryKey.Value.Value);
			}
			base.Delete(where);
		}
	}
}
