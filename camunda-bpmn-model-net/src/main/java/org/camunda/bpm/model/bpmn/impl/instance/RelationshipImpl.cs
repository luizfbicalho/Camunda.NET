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
namespace org.camunda.bpm.model.bpmn.impl.instance
{
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using Relationship = org.camunda.bpm.model.bpmn.instance.Relationship;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN relationship element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class RelationshipImpl : BaseElementImpl, Relationship
	{

	  protected internal static Attribute<string> typeAttribute;
	  protected internal static Attribute<RelationshipDirection> directionAttribute;
	  protected internal static ChildElementCollection<Source> sourceCollection;
	  protected internal static ChildElementCollection<Target> targetCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Relationship), BPMN_ELEMENT_RELATIONSHIP).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_TYPE).required().build();

		directionAttribute = typeBuilder.enumAttribute(BPMN_ATTRIBUTE_DIRECTION, typeof(RelationshipDirection)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		sourceCollection = sequenceBuilder.elementCollection(typeof(Source)).minOccurs(1).build();

		targetCollection = sequenceBuilder.elementCollection(typeof(Target)).minOccurs(1).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Relationship>
	  {
		  public Relationship newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new RelationshipImpl(instanceContext);
		  }
	  }

	  public RelationshipImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Type
	  {
		  get
		  {
			return typeAttribute.getValue(this);
		  }
		  set
		  {
			typeAttribute.setValue(this, value);
		  }
	  }


	  public virtual RelationshipDirection Direction
	  {
		  get
		  {
			return directionAttribute.getValue(this);
		  }
		  set
		  {
			directionAttribute.setValue(this, value);
		  }
	  }


	  public virtual ICollection<Source> Sources
	  {
		  get
		  {
			return sourceCollection.get(this);
		  }
	  }

	  public virtual ICollection<Target> Targets
	  {
		  get
		  {
			return targetCollection.get(this);
		  }
	  }
	}

}