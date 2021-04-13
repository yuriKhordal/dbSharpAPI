using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dbAPI {
	///<summary>Represents the database.</summary>
	public interface IDatabase : IEnumerable<ITable>, IEquatable<IDatabase>, ITabbedString {
		///<summary>Get the table at the specified index.</summary>
		///<param name="index">The index of the table in the database.</param>
		///<returns>The table with the name.</returns>
		ITable this[int index] { get; }
		///<summary>Get the table with the specified name.</summary>
		///<param name="name">The name of the table.</param>
		///<returns>The table with the name.</returns>
		ITable this[string name] { get; }
		///<summary>Whether a connection to the database is open.</summary>
		bool IsOpen { get; }
		///<summary>The tables of the database.</summary>
		ITable[] Tables { get; }
		///<summary>The amount of tables in the database.</summary>
		int Count { get; }

		///<summary>Open a connection to the database.</summary>
		void Open();
		///<summary>Create a specified table in the database.</summary>
		///<param name="table">The table to create.</param>
		void Create(ITable table);
		///<summary>Insert the specified row into a specified table.</summary>
		///<param name="table">The table to insert into.</param>
		///<param name="row">The row to insert.</param>
		void Insert(ITable table, IRow row);
		///<summary>Insert the specified rows into a specified table.</summary>
		///<param name="table">The table to insert into.</param>
		///<param name="rows">The rows to insert.</param>
		void Insert(ITable table, params IRow[] rows);
		///<summary>Update the specified table with column/value pairs from a specified row.</summary>
		///<remarks>Be careful, this will update all of the rows in the database with
		/// the values from the specified row.</remarks>
		///<param name="table">The table to update.</param>
		///<param name="row">A row with the columns and values to set.</param>
		void Update(ITable table, IRow row);
		///<summary>Update the specified table with column/value pairs from a specified
		/// row, where a specified condition is met.</summary>
		///<param name="table">The table to update.</param>
		///<param name="row">A row with the columns and values to set.</param>
		///<param name="whereCondition">The WHERE condition.</param>
		public void Update(ITable table, IRow row, string whereCondition);
		///<summary>Select all the rows from a specified table and read the specified
		/// columns.</summary>
		///<param name="table">The table to select from.</param>
		///<param name="columns">The columns to select.</param>
		///<returns>An <see cref="IDbReader"/> with the selected results.</returns>
		public IDbReader Select(ITable table, params IColumn[] columns);
		///<summary>Select rows in a specified table and read the specified columns where
		/// a specified condition is met.</summary>
		///<param name="table">The table to select from.</param>
		///<param name="whereCondition">The WHERE condition.</param>
		///<param name="columns">The columns to select.</param>
		///<returns>An <see cref="IDbReader"/> with the selected results.</returns>
		public IDbReader Select(ITable table, string whereCondition, params IColumn[] columns);
		///<summary>Select all the rows from specified tables and read the specified
		/// columns.</summary>
		///<param name="tables">The tables to select from.</param>
		///<param name="columns">The columns to select.</param>
		///<returns>An <see cref="IDbReader"/> with the selected results.</returns>
		public IDbReader Select(ITable[] tables, params IColumn[] columns);
		///<summary>Select rows in specified tables and read the specified columns
		/// where a specified condition is met.</summary>
		///<param name="tables">The tables to select from.</param>
		///<param name="whereCondition">The WHERE condition.</param>
		///<param name="columns">The columns to select.</param>
		///<returns>An <see cref="IDbReader"/> with the selected results.</returns>
		public IDbReader Select(ITable[] tables, string whereCondition, params IColumn[] columns);
		///<summary>Select all columns from a specified table.</summary>
		///<param name="table">The table to select from.</param>
		///<returns>An <see cref="IDbReader"/> with the selected results.</returns>
		public IDbReader SelectAll(ITable table);
		///<summary>Select all columns from the specified table where a specified
		/// condition is met.</summary>
		///<param name="table">The table to select from.</param>
		///<param name="whereCondition">The WHERE condition.</param>
		///<returns>An <see cref="IDbReader"/> with the selected results.</returns>
		public IDbReader SelectAll(ITable table, string whereCondition);
		///<summary>Select all columns from the specified tables.</summary>
		///<param name="tables">The tables to select from.</param>
		///<returns>An <see cref="IDbReader"/> with the selected results.</returns>
		public IDbReader SelectAll(params ITable[] tables);
		///<summary>Select all columns from the specified tables where a specified
		/// condition is met.</summary>
		///<param name="whereCondition">The WHERE condition.</param>
		///<param name="tables">The tables to select from.</param>
		///<returns>An <see cref="IDbReader"/> with the selected results.</returns>
		public IDbReader SelectAll(string whereCondition, params ITable[] tables);
		///<summary>Delete from a specified table where a specified condition is met.</summary>
		///<param name="table">The table from which to delete.</param>
		///<param name="whereCondition">The WHERE condition.</param>
		public void Delete(ITable table, string whereCondition);
		///<summary>Drop a specified table.</summary>
		///<param name="table">The table to drop.</param>
		public void Drop(ITable table);
		///<summary>Close the connection to the database.</summary>
		void Close();
	}

	///<inheritdoc cref="IDatabase"/>
	public abstract class AbsDatabase : IDatabase, IEquatable<AbsDatabase> {
		///<inheritdoc cref="Tables"/>
		protected List<ITable> tables;
		///<summary>The handle to the database.</summary>
		protected IDbHelper helper;

		public virtual ITable this[int index] => tables[index];
		public virtual ITable this[string name] =>
			tables.Find((table) => table.Name == name);
		public virtual bool IsOpen => helper.IsOpen;
		public virtual ITable[] Tables => tables.ToArray();
		public virtual int Count => tables.Count;

		///<inheritdoc cref="AbsDatabase(IDbHelper, IEnumerable{ITable})"/>
		public AbsDatabase(IDbHelper helper) =>
			(this.helper, this.tables) = (helper, new List<ITable>());
		/// <summary>Initialize a new database with a helper and tables.</summary>
		/// <param name="helper">The helper to the database.</param>
		/// <param name="tables">The tables for the database.</param>
		public AbsDatabase(IDbHelper helper, IEnumerable<ITable> tables) =>
			(this.helper, this.tables) = (helper, new List<ITable>(tables));
		///<inheritdoc cref="AbsDatabase(IDbHelper, IEnumerable{ITable})"/>
		public AbsDatabase(IDbHelper helper, params ITable[] tables) :
			this(helper, (IEnumerable<ITable>)tables) { }

		public static bool operator ==(AbsDatabase left, IDatabase right) =>
			Object.Equals(left, right);
		public static bool operator !=(AbsDatabase left, IDatabase right) =>
			!(left == right);

		public virtual void Open() { if (IsOpen) helper.Open(); }
		public virtual void Insert(ITable table, IRow row) {
			IColumn[] cols = new IColumn[row.Count];
			IDbValue[] vals = new IDbValue[cols.Length];
			for (int i = 0; i < cols.Length; i++) {
				cols[i] = row.GetColumn(i);
				vals[i] = row.GetValue(i);
			}
			helper.Insert(table, cols, vals);
		}
		public virtual void Insert(ITable table, params IRow[] rows) {
			foreach (IRow row in rows) this.Insert(table, row);
		}
		public virtual void Create(ITable table) {
			helper.Create(table);
			tables.Add(table);
		}
		public virtual void Update(ITable table, IRow row) => helper.Update(table, row);
		public virtual void Update(ITable table, IRow row, string whereCondition) =>
			helper.Update(table, row, whereCondition);
		public virtual IDbReader Select(ITable table, params IColumn[] columns) =>
			helper.Select(table, columns);
		public virtual IDbReader Select(ITable table, string whereCondition, params IColumn[] columns) =>
			helper.Select(table, whereCondition, columns);
		public virtual IDbReader Select(ITable[] tables, params IColumn[] columns) =>
			helper.Select(tables, columns);
		public virtual IDbReader Select(ITable[] tables, string whereCondition, params IColumn[] columns) =>
			helper.Select(tables, whereCondition, columns);
		public virtual IDbReader SelectAll(ITable table) => helper.SelectAll(table);
		public virtual IDbReader SelectAll(ITable table, string whereCondition) =>
			helper.SelectAll(table, whereCondition);
		public virtual IDbReader SelectAll(params ITable[] tables) => helper.SelectAll(tables);
		public virtual IDbReader SelectAll(string whereCondition, params ITable[] tables) =>
			helper.SelectAll(whereCondition, tables);
		public virtual void Delete(ITable table, string whereCondition) =>
			helper.Delete(table, whereCondition);
		public virtual void Drop(ITable table) {
			helper.Drop(table);
			tables.Remove(table);
		}
		public virtual void Close() => helper.Close();

		public virtual IEnumerator<ITable> GetEnumerator() => tables.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public virtual bool Equals(AbsDatabase other) => Equals(other as IDatabase);
		public virtual bool Equals(IDatabase other) =>
			!(other is null) && tables.MemberEquals(other);
		public override bool Equals(object obj) => Equals(obj as IDatabase);
		public override int GetHashCode() => HashCode.Combine(tables.Hash(), helper);
		public override string ToString() => ToString(0);
		public virtual string ToString(int tabs) {
			StringBuilder builder = new StringBuilder();
			builder.Append("Database {\n");
			builder.Append('\t', tabs + 1).Append(helper.ToString(tabs + 1)).Append('\n');
			foreach (ITable table in tables)
				builder.Append('\t', tabs + 1).Append(table.ToString(tabs + 1))
					.Append('\n');
			builder.Append('\t').Append('}');
			return builder.ToString();
		}
	}
}
