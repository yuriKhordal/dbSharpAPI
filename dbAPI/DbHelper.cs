using System;
using System.Collections.Generic;
using System.Text;

namespace dbAPI {
	///<summary>An interface that represents a generic database for specific database APIs.
	/// It's supposed to link between a specific database type and it's API,
	/// and this generic API. Should be implemented for each database type.</summary>
	public interface IDbHelper : ITabbedString {
		///<summary>Check if the database is open.</summary>
		bool IsOpen { get; }
		
		///<summary>Opens the connection to the database.</summary>
		void Open();
		///<summary>Create a specified table in the database.</summary>
		///<param name="table">The table to create.</param>
		void Create(ITable table);
		///<summary>Insert the specified values into specified columns of a specified
		/// table.</summary>
		///<param name="table">The table to insert into.</param>
		///<param name="columns">The columns to insert into.</param>
		///<param name="values">The values to insert.</param>
		void Insert(ITable table, IColumn[] columns, IDbValue[] values);
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
		///<summary>Execute a specified sql statement.</summary>
		///<param name="sql">The sql statement.</param>
		public void ExecuteSql(string sql);
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
		///<summary>Execute an sql statement and return results in an
		/// <see cref="IDbReader"/>.</summary>
		///<param name="sql">The sql statement to execute.</param>
		///<returns>An <see cref="IDbReader"/> with the results.</returns>
		public IDbReader ReadSql(string sql);
		///<summary>Convert an <see cref="IDbValue"/> to the string format that can be
		/// used in sql statements.</summary>
		///<param name="value">The database value to convert.</param>
		///<returns>A string format of the value.</returns>
		public string DbValueToString(IDbValue value);
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
}
