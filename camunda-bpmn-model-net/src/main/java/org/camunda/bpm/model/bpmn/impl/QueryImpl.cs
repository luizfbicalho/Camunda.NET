using System;
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
namespace org.camunda.bpm.model.bpmn.impl
{
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class QueryImpl<T> : Query<T> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  private readonly ICollection<T> collection;

	  public QueryImpl(ICollection<T> collection)
	  {
		this.collection = collection;
	  }

	  public virtual IList<T> list()
	  {
		return new List<T>(collection);
	  }

	  public virtual int count()
	  {
		return collection.Count;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <V extends org.camunda.bpm.model.xml.instance.ModelElementInstance> org.camunda.bpm.model.bpmn.Query<V> filterByType(org.camunda.bpm.model.xml.type.ModelElementType elementType)
	  public virtual Query<V> filterByType<V>(ModelElementType elementType) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		Type<V> elementClass = (Type<V>) elementType.InstanceType;
		return filterByType(elementClass);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <V extends org.camunda.bpm.model.xml.instance.ModelElementInstance> org.camunda.bpm.model.bpmn.Query<V> filterByType(Class<V> elementClass)
	  public virtual Query<V> filterByType<V>(Type<V> elementClass) where V : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		IList<V> filtered = new List<V>();
		foreach (T instance in collection)
		{
		  if (elementClass.IsAssignableFrom(instance.GetType()))
		  {
			filtered.Add((V) instance);
		  }
		}
		return new QueryImpl<V>(filtered);
	  }

	  public virtual T singleResult()
	  {
		if (collection.Count == 1)
		{
		  return collection.GetEnumerator().next();
		}
		else
		{
		  throw new BpmnModelException("Collection expected to have <1> entry but has <" + collection.Count + ">");
		}
	  }
	}

}