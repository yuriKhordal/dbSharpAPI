using System;
using System.Collections.Generic;

namespace dbAPI {
	///<summary>Represents a data type in a database.</summary>
	public interface IDataType : IUnique, ITabbedString, IEquatable<IDataType> { }

	///<inheritdoc cref="IDataType"/>
	public class DataType : Unique, IDataType {
		///<summary>Represents a <c>bool</c> type.</summary>
		public static readonly DataType BOOLEAN = new DataType(1);
		///<summary>Represents a <c>byte[]</c> type.</summary>
		public static readonly DataType BYTE_ARRAY = new DataType(2);
		///<summary>Represents a type of date and/or time.</summary>
		public static readonly DataType DATE_TIME = new DataType(3);
		///<summary>Represents a <c>double</c> type.</summary>
		public static readonly DataType DOUBLE = new DataType(4);
		///<summary>Represents an <c>int</c> type.</summary>
		public static readonly DataType INTEGER = new DataType(5);
		///<summary>Represents a <c>string</c> type.</summary>
		public static readonly DataType STRING = new DataType(6);

		///<inheritdoc cref="Name"/>
		protected readonly string name;

		///<summary>The name of the type.</summary>
		public string Name => name;

		///<summary>Initializes a new type with a unique ID.</summary>
		///<param name="id">A unique id for the type.</param>
		public DataType(int id, string name = null) : base(id) {
			this.name = name;
		}

		public static bool operator ==(DataType left, IDataType right)
			=> left is null ? right is null : left.Equals(right);
		public static bool operator !=(DataType left, IDataType right)
			=> !(left == right);
		public static bool operator ==(DataType left, IUnique right)
			=> left is null ? right is null : left.Equals(right);
		public static bool operator !=(DataType left, IUnique right)
			=> !(left == right);

		public virtual bool Equals(IDataType other) => ID == other?.ID;
		public override bool Equals(IUnique other) => Equals(other as IDataType);
		public override bool Equals(object obj) =>
			Object.ReferenceEquals(this, obj) ||
			(!(obj is null) && (obj is IDataType type ? Equals(type) :
			obj is int num && num == id));
		public override int GetHashCode() => base.GetHashCode();
		public override string ToString() => ToString(0);
		public override string ToString(int tabs) =>
			name is null ? $"Type({id})" : $"Type({id}, {Name})";
	}
}