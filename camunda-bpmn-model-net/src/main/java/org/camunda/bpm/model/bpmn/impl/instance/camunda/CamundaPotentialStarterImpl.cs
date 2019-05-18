﻿/*
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
	using ResourceAssignmentExpression = org.camunda.bpm.model.bpmn.instance.ResourceAssignmentExpression;
	using CamundaPotentialStarter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaPotentialStarter;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ELEMENT_POTENTIAL_STARTER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN potentialStarter camunda extension
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaPotentialStarterImpl : BpmnModelElementInstanceImpl, CamundaPotentialStarter
	{

	  protected internal static ChildElement<ResourceAssignmentExpression> resourceAssignmentExpressionChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CamundaPotentialStarter), CAMUNDA_ELEMENT_POTENTIAL_STARTER).namespaceUri(CAMUNDA_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		resourceAssignmentExpressionChild = sequenceBuilder.element(typeof(ResourceAssignmentExpression)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CamundaPotentialStarter>
	  {
		  public CamundaPotentialStarter newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaPotentialStarterImpl(instanceContext);
		  }
	  }

	  public CamundaPotentialStarterImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual ResourceAssignmentExpression ResourceAssignmentExpression
	  {
		  get
		  {
			return resourceAssignmentExpressionChild.getChild(this);
		  }
		  set
		  {
			resourceAssignmentExpressionChild.setChild(this, value);
		  }
	  }

	}

}