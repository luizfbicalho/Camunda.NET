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
namespace org.camunda.bpm.model.bpmn.impl.instance.di
{
	using Diagram = org.camunda.bpm.model.bpmn.instance.di.Diagram;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The DI Diagram element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public abstract class DiagramImpl : BpmnModelElementInstanceImpl, Diagram
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static Attribute<string> documentationAttribute;
	  protected internal static Attribute<double> resolutionAttribute;
	  protected internal static Attribute<string> idAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Diagram), DI_ELEMENT_DIAGRAM).namespaceUri(DI_NS).abstractType();

		nameAttribute = typeBuilder.stringAttribute(DI_ATTRIBUTE_NAME).build();

		documentationAttribute = typeBuilder.stringAttribute(DI_ATTRIBUTE_DOCUMENTATION).build();

		resolutionAttribute = typeBuilder.doubleAttribute(DI_ATTRIBUTE_RESOLUTION).build();

		idAttribute = typeBuilder.stringAttribute(DI_ATTRIBUTE_ID).idAttribute().build();

		typeBuilder.build();
	  }

	  public DiagramImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Name
	  {
		  get
		  {
			return nameAttribute.getValue(this);
		  }
		  set
		  {
			nameAttribute.setValue(this, value);
		  }
	  }


	  public virtual string Documentation
	  {
		  get
		  {
			return documentationAttribute.getValue(this);
		  }
		  set
		  {
			documentationAttribute.setValue(this, value);
		  }
	  }


	  public virtual double Resolution
	  {
		  get
		  {
			return resolutionAttribute.getValue(this);
		  }
		  set
		  {
			resolutionAttribute.setValue(this, value);
		  }
	  }


	  public virtual string Id
	  {
		  get
		  {
			return idAttribute.getValue(this);
		  }
		  set
		  {
			idAttribute.setValue(this, value);
		  }
	  }

	}

}