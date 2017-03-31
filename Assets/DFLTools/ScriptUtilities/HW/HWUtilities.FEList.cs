using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HWTools
{
	/// <summary>
	///   For Each List. Mostly functionally identical to a normal List. 
	///   <para/>
	///   When an iterator is requested, if changes have been made to the list, the FEList
	///   instantiates a shallow copy to allow modifications of the list during foreach loops.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FEList<T> : IList<T>
	{
		#region Private Fields

		readonly List<T> _contents;

		bool _dirty = true;
		IEnumerable<T> _snapshot;

		#endregion

		#region Public Properties

		public int Count
		{
			get
			{
				return ((IList<T>)_contents).Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return ((IList<T>)_contents).IsReadOnly;
			}
		}

		#endregion
		
		#region Public Constructors

		public FEList()
		{
			_dirty = true;
			_contents = new List<T>();
		}

		public FEList(int capacity)
		{
			_dirty = true;
			_contents = new List<T>(capacity);
		}

		public FEList(IEnumerable<T> collection)
		{
			_dirty = true;
			_contents = new List<T>(collection);
		}

		#endregion

		#region Public Indexers

		public T this[int index]
		{
			get
			{
				return (_contents)[index];
			}

			set
			{
				_dirty = true;
				((IList<T>)_contents)[index] = value;
			}
		}

		#endregion

		#region Public Methods

		public void Add(T item)
		{
			_dirty = true;
			((IList<T>)_contents).Add(item);
		}

		public void Clear()
		{
			_dirty = true;
			((IList<T>)_contents).Clear();
		}

		public bool Contains(T item)
		{
			return ((IList<T>)_contents).Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			((IList<T>)_contents).CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (_dirty)
			{
				_snapshot = new List<T>(_contents);
			}
			_dirty = false;

			return ((IList<T>)_snapshot).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (_dirty)
			{
				_snapshot = new List<T>(_contents);
			}
			_dirty = false;

			return ((IList<T>)_contents).GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return ((IList<T>)_contents).IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_dirty = true;
			((IList<T>)_contents).Insert(index, item);
		}

		public bool Remove(T item)
		{
			_dirty = true;
			return ((IList<T>)_contents).Remove(item);
		}

		public void RemoveAt(int index)
		{
			_dirty = true;
			((IList<T>)_contents).RemoveAt(index);
		}

		#endregion
	}
}
