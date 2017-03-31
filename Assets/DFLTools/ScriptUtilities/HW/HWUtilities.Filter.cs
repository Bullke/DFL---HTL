using System;
using UnityEngine;

namespace HWTools.Filter
{
	/// <summary>
	///   A lazily evaluated filter. 
	///   <para/>
	///   Computes a filtering function <typeparamref name="TIn"/> -&gt; <typeparamref name="TOut"/>
	///   upon accessing the output property.
	/// </summary>
	/// <typeparam name="TIn"> Input type </typeparam>
	/// <typeparam name="TOut"> Output type </typeparam>
	[Serializable]
	public class Filter<TIn, TOut>
	{
		#region Protected Fields
		
		[SerializeField, HideInInspector]
		protected TOut _filteredValue;

		protected Func<TIn, TOut> _onAccess;

		[SerializeField, HideInInspector]
		protected TIn _sourceValue;

		#endregion

		#region Public Properties

		/// <summary>
		///   The dirty state of the Filter. 
		///   <para/>
		///   Determines whether the filter needs to be computed. 
		/// </summary>
		public bool Dirty { get; protected set; }

		/// <summary>
		///   Filtering function. 
		///   <para/>
		///   Must be a function that processes a <typeparamref name="TIn"/> value into a
		///   <typeparamref name="TOut"/> value.
		/// </summary>
		public virtual Func<TIn, TOut> FilterFunction
		{
			get { return _onAccess; }
			set
			{
				if (_onAccess != value)
				{
					SetDirty();
					_onAccess = value;
				}
			}
		}

		/// <summary>
		///   The output value of the filter. 
		///   <para/>
		///   Accessor will compute the filter function if the filter is dirty. 
		/// </summary>
		public TOut FilteredValue
		{
			get
			{
				if (Dirty)
				{
					Dirty = false;
					_filteredValue = FilterFunction(_sourceValue);
				}

				return _filteredValue;
			}
		}

		/// <summary>
		///   The input value of the filter. 
		///   <para/>
		///   Setter causes the filter to become dirty if there has been a change. 
		/// </summary>
		public TIn SourceValue
		{
			get { return _sourceValue; }
			set
			{
				// Try to avoid dirty state if reasonable.
				if ((_sourceValue == null && value != null)
					|| !_sourceValue.Equals(value))
				{
					SetDirty();
					_sourceValue = value;
				}
			}
		}

		#endregion

		#region Public Methods

		public static explicit operator TOut(Filter<TIn, TOut> filt)
		{
			return filt.FilteredValue;
		}

		/// <summary>
		///   Marks the filter as dirty. 
		///   <para/>
		///   The filter will be computed the next time FilteredValue is accessed. 
		/// </summary>
		public void SetDirty()
		{ Dirty = true; }

		#endregion
	}

	/// <summary>
	///   A lazily evaluated filter. 
	///   <para/>
	///   Computes filtering function <typeparamref name="T"/> -&gt; <typeparamref name="T"/> upon
	///   accessing the output property.
	/// </summary>
	/// <typeparam name="T"> Value type </typeparam>
	[Serializable]
	public class Filter<T> : Filter<T, T>
	{
		#region Public Properties

		/// <summary>
		///   Filtering function. 
		///   <para/>
		///   Must be a function that processes a <typeparamref name="T"/> value into another
		///   <typeparamref name="T"/> value.
		///   <para/>
		///   Defaults to a 1:1 filter if null. 
		/// </summary>
		public override Func<T, T> FilterFunction
		{
			get { return base.FilterFunction ?? DefaultFilter; }
			set { base.FilterFunction = value; }
		}

		public T Value
		{
			get { return FilteredValue; }
			set { SourceValue = value; }
		}

		#endregion

		#region Private Methods

		static T DefaultFilter(T t)
		{ return t; }

		#endregion

	}

	/// <summary>
	/// A Filter&lt;int&gt;.
	/// <para/>
	/// Unity doesn't like serializing generic classes. So, welp!
	/// </summary>
	[Serializable]
	public class FilteredInt : Filter<int> { }

	/// <summary>
	/// A Filter&lt;string&gt;.
	/// <para/>
	/// Unity doesn't like serializing generic classes. So, welp!
	/// </summary>
	[Serializable]
	public class FilteredString : Filter<string> { }

	/// <summary>
	/// A Filter&lt;Vector2&gt;.
	/// <para/>
	/// Unity doesn't like serializing generic classes. So, welp!
	/// </summary>
	[Serializable]
	public class FilteredV2 : Filter<Vector2> { }


}
