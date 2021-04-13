using System;
using System.Collections.Generic;

namespace dbAPI {
	///<summary>Represents a unique object associated with an ID.</summary>
	public interface IUnique : IEquatable<IUnique>, ITabbedString {
		///<summary>A unique ID to identify the object.</summary>
		int ID { get; }
	}

	///<inheritdoc cref="IUnique"/>
	public class Unique : IUnique {
		///<inheritdoc cref="ID"/>
		protected readonly int id;
		public virtual int ID => id;

		///<summary>Initializes a new unique object with an id.</summary>
		/// <param name="id">A unique ID to identify the object.</param>
		public Unique(int id) => this.id = id;

		public static bool operator ==(Unique left, IUnique right) =>
			left is null ? right is null : left.Equals(right);
		public static bool operator !=(Unique left, IUnique right) =>
			!(left == right);

		public virtual bool Equals(IUnique other) => other?.ID == ID;
		public override bool Equals(object obj) =>
			Object.ReferenceEquals(this, obj) || (!(obj is null) &&
			(obj is IUnique unq ? Equals(unq) : obj is int i && i == id));
		public override int GetHashCode() => HashCode.Combine(id);
		public override string ToString() => ToString(0);
		public virtual string ToString(int tabs) => $"Unique({id})";
	}
}