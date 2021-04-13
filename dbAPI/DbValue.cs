using System;
using System.Collections.Generic;
using System.Text;

namespace dbAPI {
	///<summary>Represents a value in a database.</summary>
	public interface IDbValue : IEquatable<IDbValue>, ITabbedString, ICloneable {
		///<summary>The value.</summary>
		object Value { get; set; }
		///<summary>The data type of the value.</summary>
		IDataType Type { get; }

		///<inheritdoc cref="ICloneable.Clone"/>
		new IDbValue Clone();
	}
	///<inheritdoc cref="IDbValue"/>
	public interface IDbValue<T> : IDbValue, IEquatable<IDbValue<T>>, ITabbedString,
			ICloneable {
		///<inheritdoc cref="IDbValue.Value"/>
		new T Value { get; set; }

		///<inheritdoc cref="ICloneable.Clone"/>
		new IDbValue<T> Clone();
	}

	///<inheritdoc cref="IDbValue"/>
	public class DbValue : IDbValue {
		///<inheritdoc cref="Type"/>
		protected readonly IDataType type;
		public virtual object Value { get; set; }
		public virtual IDataType Type => type;

		///<summary>Initializes a new value.</summary>
		///<param name="value">A value.</param>
		///<param name="type">The data type of the value.</param>
		public DbValue(object value, IDataType type)
			=> (Value, this.type) = (value, type);

		public static bool operator ==(DbValue left, IDbValue right)
			=> left is null ? false : left.Equals(right);
		public static bool operator !=(DbValue left, IDbValue right)
			=> !(left == right);

		///<inheritdoc cref="ICloneable.Clone"/>
		public virtual DbValue Clone() =>
			new DbValue(Value is ICloneable val ? val.Clone() : Value, type);
		IDbValue IDbValue.Clone() => this.Clone();
		object ICloneable.Clone() => this.Clone();

		public virtual bool Equals(IDbValue other) =>
			Object.ReferenceEquals(this, other) || (!(other is null) &&
			type.Equals(other.Type) && Value.Equals(other.Value));
		public override bool Equals(object obj) =>
			Object.ReferenceEquals(this, obj) || (!(obj is null) &&
			(obj is IDbValue val ? Equals(val) : Equals(Value, obj)));
		public override int GetHashCode() => HashCode.Combine(Value, type);
		public override string ToString() => ToString(0);
		public virtual string ToString(int tabs) { //$"Value({Value}, {type})";
			string val = Value is ITabbedString tab ? tab.ToString(tabs) :
				Value.ToString();
			string type = Type is ITabbedString tab1 ? tab1.ToString(tabs) :
				Type.ToString();
			return $"Value({val}, {type})";
		}
	}

	///<inheritdoc cref="DbValue"/>
	///<typeparam name="T">The type of the value</typeparam>
	public class DbValue<T> : IDbValue<T> {
		protected DbValue val;
		public virtual T Value { get => (T)val.Value; set => val.Value = value; }
		object IDbValue.Value { get => Value; set => Value = (T)value; }
		public virtual IDataType Type => val.Type;

		///<inheritdoc cref="DbValue.DbValue(object, IDataType)"/>
		public DbValue(T value, IDataType type) =>
			val = new DbValue(value, type);

		public static implicit operator DbValue(DbValue<T> val)
			=> val.val;
		public static bool operator ==(DbValue<T> left, IDbValue right)
			=> left is null ? false : left.Equals(right);
		public static bool operator !=(DbValue<T> left, IDbValue right)
			=> !(left == right);

		///<inheritdoc cref="ICloneable.Clone"/>
		public virtual DbValue<T> Clone() =>
			new DbValue<T>(Value, Type);
		IDbValue<T> IDbValue<T>.Clone() => this.Clone();
		IDbValue IDbValue.Clone() => this.Clone();
		object ICloneable.Clone() => this.Clone();

		public virtual bool Equals(IDbValue other) => val.Equals(other);
		public virtual bool Equals(IDbValue<T> other) => val.Equals(other);
		public override bool Equals(object obj) => val.Equals(obj);
		public override int GetHashCode() => val.GetHashCode();
		public override string ToString() => ToString(0);
		public virtual string ToString(int tabs) { //$"Value({Value}, {type})";
			string val = Value is ITabbedString tab ? tab.ToString(tabs) :
				Value.ToString();
			string type = Type is ITabbedString tab1 ? tab1.ToString(tabs) :
				Type.ToString();
			return $"Value({val}, {type})";
		}
	}




	///<summary>Represents a boolean value in a database.</summary>
	public class DbBoolean : DbValue<bool> {
		///<inheritdoc cref="DbValue.DbValue(object, IDataType)"/>
		public DbBoolean(bool value) : base(value, DataType.BOOLEAN) { }
	}
	///<summary>Represents a byte array value in a database.</summary>
	public class DbByteArray : DbValue<byte[]> {
		///<inheritdoc cref="DbValue.DbValue(object, IDataType)"/>
		public DbByteArray(byte[] value) : base(value, DataType.BYTE_ARRAY) { }
	}
	///<summary>Represents a date and time value in a database.</summary>
	public class DbDateTime : DbValue<DateTime> {
		///<inheritdoc cref="DbValue.DbValue(object, IDataType)"/>
		public DbDateTime(DateTime value) : base(value, DataType.DATE_TIME) { }
	}
	///<summary>Represents a double value in a database.</summary>
	public class DbDouble : DbValue<double> {
		///<inheritdoc cref="DbValue.DbValue(object, IDataType)"/>
		public DbDouble(double value) : base(value, DataType.DOUBLE) { }
	}
	///<summary>Represents an integer value in a database.</summary>
	public class DbInteger : DbValue<int> {
		///<inheritdoc cref="DbValue.DbValue(object, IDataType)"/>
		public DbInteger(int value) : base(value, DataType.INTEGER) {}
	}
	///<summary>Represents a string value in a database.</summary>
	public class DbString : DbValue<string> {
		///<inheritdoc cref="DbValue.DbValue(object, IDataType)"/>
		public DbString(string value) : base(value, DataType.STRING) { }
	}
}
