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
namespace org.camunda.bpm.model.bpmn.impl.instance
{
	using SubProcessBuilder = org.camunda.bpm.model.bpmn.builder.SubProcessBuilder;
	using org.camunda.bpm.model.bpmn.instance;
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
	/// The BPMN subProcess element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class SubProcessImpl : ActivityImpl, SubProcess
	{

	  protected internal static Attribute<bool> triggeredByEventAttribute;
	  protected internal static ChildElementCollection<LaneSet> laneSetCollection;
	  protected internal static ChildElementCollection<FlowElement> flowElementCollection;
	  protected internal static ChildElementCollection<Artifact> artifactCollection;

	  /// <summary>
	  /// camunda extensions </summary>
	  protected internal static Attribute<bool> camundaAsyncAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(SubProcess), BPMN_ELEMENT_SUB_PROCESS).namespaceUri(BPMN20_NS).extendsType(typeof(Activity)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		triggeredByEventAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_TRIGGERED_BY_EVENT).defaultValue(false).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		laneSetCollection = sequenceBuilder.elementCollection(typeof(LaneSet)).build();

		flowElementCollection = sequenceBuilder.elementCollection(typeof(FlowElement)).build();

		artifactCollection = sequenceBuilder.elementCollection(typeof(Artifact)).build();

		/// <summary>
		/// camunda extensions </summary>

		camundaAsyncAttribute = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_ASYNC).@namespace(CAMUNDA_NS).defaultValue(false).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<SubProcess>
	  {
		  public SubProcess newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new SubProcessImpl(instanceContext);
		  }
	  }

	  public SubProcessImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override SubProcessBuilder builder()
	  {
		return new SubProcessBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  public virtual bool triggeredByEvent()
	  {
		return triggeredByEventAttribute.getValue(this);
	  }

	  public virtual bool TriggeredByEvent
	  {
		  set
		  {
			triggeredByEventAttribute.setValue(this, value);
		  }
	  }

	  public virtual ICollection<LaneSet> LaneSets
	  {
		  get
		  {
			return laneSetCollection.get(this);
		  }
	  }

	  public virtual ICollection<FlowElement> FlowElements
	  {
		  get
		  {
			return flowElementCollection.get(this);
		  }
	  }

	  public virtual ICollection<Artifact> Artifacts
	  {
		  get
		  {
			return artifactCollection.get(this);
		  }
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use isCamundaAsyncBefore() instead. 
	  [Obsolete("use isCamundaAsyncBefore() instead.")]
	  public virtual bool CamundaAsync
	  {
		  get
		  {
			return camundaAsyncAttribute.getValue(this);
		  }
		  set
		  {
			camundaAsyncAttribute.setValue(this, value);
		  }
	  }


	}

}