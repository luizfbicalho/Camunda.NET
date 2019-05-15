using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.qa.upgrade.json.beans
{
	using JsonDeserialize = com.fasterxml.jackson.databind.annotation.JsonDeserialize;

	/// <summary>
	/// @author Svetlana Dorokhova.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonDeserialize(contentAs = RegularCustomer.class) public class ObjectList<T> implements java.util.List<Object>
	public class ObjectList<T> : IList<object>
	{

	  protected internal IList<object> innerList = new List<object>();

	  public override int size()
	  {
		return innerList.Count;
	  }

	  public override bool Empty
	  {
		  get
		  {
			return innerList.Count == 0;
		  }
	  }

	  public override bool contains(object o)
	  {
		return innerList.Contains(o);
	  }

	  public override IEnumerator<object> iterator()
	  {
		return innerList.GetEnumerator();
	  }

	  public override object[] toArray()
	  {
		return innerList.ToArray();
	  }

	  public override object[] toArray<object>(object[] a)
	  {
		return innerList.toArray(a);
	  }

	  public override bool add(object customer)
	  {
		return innerList.Add(customer);
	  }

	  public override bool remove(object o)
	  {
		return innerList.Remove(o);
	  }

	  public override bool containsAll<T1>(ICollection<T1> c)
	  {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		return innerList.containsAll(c);
	  }

	  public override bool addAll<T1>(ICollection<T1> c) where T1 : object
	  {
		return ((IList<object>)innerList).AddRange(c);
	  }

	  public override bool addAll<T1>(int index, ICollection<T1> c) where T1 : object
	  {
		return ((IList<object>)innerList).AddRange(index, c);
	  }

	  public override bool removeAll<T1>(ICollection<T1> c)
	  {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		return innerList.removeAll(c);
	  }

	  public override bool retainAll<T1>(ICollection<T1> c)
	  {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'retainAll' method:
		return innerList.retainAll(c);
	  }

	  public override void clear()
	  {
		innerList.Clear();
	  }

	  public override bool Equals(object o)
	  {
//JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
//ORIGINAL LINE: return innerList.equals(o);
		return innerList.SequenceEqual(o);
	  }

	  public override int GetHashCode()
	  {
		return innerList.GetHashCode();
	  }

	  public override object get(int index)
	  {
		return innerList[index];
	  }

	  public override object set(int index, object element)
	  {
		return innerList[index] = element;
	  }

	  public override void add(int index, object element)
	  {
		innerList.Insert(index, element);
	  }

	  public override object remove(int index)
	  {
		return innerList.RemoveAt(index);
	  }

	  public override int indexOf(object o)
	  {
		return innerList.IndexOf(o);
	  }

	  public override int lastIndexOf(object o)
	  {
		return innerList.LastIndexOf(o);
	  }

	  public override IEnumerator<object> listIterator()
	  {
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
		return innerList.GetEnumerator();
	  }

	  public override IEnumerator<object> listIterator(int index)
	  {
		return innerList.listIterator(index);
	  }

	  public override IList<object> subList(int fromIndex, int toIndex)
	  {
		return innerList.subList(fromIndex, toIndex);
	  }
	}

}