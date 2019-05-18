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
namespace org.camunda.bpm.model.bpmn.impl.instance.camunda
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ELEMENT_LIST;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;


	using BpmnModelElementInstance = org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance;
	using CamundaList = org.camunda.bpm.model.bpmn.instance.camunda.CamundaList;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using UnsupportedModelOperationException = org.camunda.bpm.model.xml.UnsupportedModelOperationException;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaListImpl : BpmnModelElementInstanceImpl, CamundaList
	{

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaList), CAMUNDA_ELEMENT_LIST).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<CamundaList>
	  {
		  public CamundaList newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaListImpl(instanceContext);
		  }
	  }

	  public CamundaListImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance> java.util.Collection<T> getValues()
	  public virtual ICollection<T> getValues<T>() where T : org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance
	  {
		  get
		  {
    
			return new CollectionAnonymousInnerClass(this);
		  }
	  }

	  private class CollectionAnonymousInnerClass : ICollection<T>
	  {
		  private readonly CamundaListImpl outerInstance;

		  public CollectionAnonymousInnerClass(CamundaListImpl outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  protected internal ICollection<T> Elements
		  {
			  get
			  {
				return ModelUtil.getModelElementCollection(DomElement.ChildElements, ModelInstance);
			  }
		  }

		  public int size()
		  {
			return Elements.size();
		  }

		  public bool Empty
		  {
			  get
			  {
				return Elements.Empty;
			  }
		  }

		  public bool contains(object o)
		  {
			return Elements.contains(o);
		  }

		  public IEnumerator<T> iterator()
		  {
			return (IEnumerator<T>) Elements.GetEnumerator();
		  }

		  public object[] toArray()
		  {
			return Elements.toArray();
		  }

		  public T1[] toArray<T1>(T1[] a)
		  {
			return Elements.toArray(a);
		  }

		  public bool add(T t)
		  {
			DomElement.appendChild(t.DomElement);
			return true;
		  }

		  public bool remove(object o)
		  {
			ModelUtil.ensureInstanceOf(o, typeof(BpmnModelElementInstance));
			return DomElement.removeChild(((BpmnModelElementInstance) o).DomElement);
		  }

		  public bool containsAll<T1>(ICollection<T1> c)
		  {
			foreach (object o in c)
			{
			  if (!contains(o))
			  {
				return false;
			  }
			}
			return true;
		  }

		  public bool addAll<T1>(ICollection<T1> c) where T1 : T
		  {
			foreach (T element in c)
			{
			  add(element);
			}
			return true;
		  }

		  public bool removeAll<T1>(ICollection<T1> c)
		  {
			bool result = false;
			foreach (object o in c)
			{
			  result |= remove(o);
			}
			return result;
		  }

		  public bool retainAll<T1>(ICollection<T1> c)
		  {
			throw new UnsupportedModelOperationException("retainAll()", "not implemented");
		  }

		  public void clear()
		  {
			DomElement domElement = DomElement;
			IList<DomElement> childElements = domElement.ChildElements;
			foreach (DomElement childElement in childElements)
			{
			  domElement.removeChild(childElement);
			}
		  }
	  }

	}

}