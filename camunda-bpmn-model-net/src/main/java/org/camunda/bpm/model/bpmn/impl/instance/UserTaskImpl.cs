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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ATTRIBUTE_IMPLEMENTATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_USER_TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_ASSIGNEE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CANDIDATE_GROUPS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_CANDIDATE_USERS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_DUE_DATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_FOLLOW_UP_DATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_FORM_HANDLER_CLASS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_FORM_KEY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_PRIORITY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;


	using UserTaskBuilder = org.camunda.bpm.model.bpmn.builder.UserTaskBuilder;
	using Rendering = org.camunda.bpm.model.bpmn.instance.Rendering;
	using Task = org.camunda.bpm.model.bpmn.instance.Task;
	using UserTask = org.camunda.bpm.model.bpmn.instance.UserTask;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using StringUtil = org.camunda.bpm.model.xml.impl.util.StringUtil;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	/// <summary>
	/// The BPMN userTask element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class UserTaskImpl : TaskImpl, UserTask
	{

	  protected internal static Attribute<string> implementationAttribute;
	  protected internal static ChildElementCollection<Rendering> renderingCollection;

	  /// <summary>
	  /// camunda extensions </summary>

	  protected internal static Attribute<string> camundaAssigneeAttribute;
	  protected internal static Attribute<string> camundaCandidateGroupsAttribute;
	  protected internal static Attribute<string> camundaCandidateUsersAttribute;
	  protected internal static Attribute<string> camundaDueDateAttribute;
	  protected internal static Attribute<string> camundaFollowUpDateAttribute;
	  protected internal static Attribute<string> camundaFormHandlerClassAttribute;
	  protected internal static Attribute<string> camundaFormKeyAttribute;
	  protected internal static Attribute<string> camundaPriorityAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(UserTask), BPMN_ELEMENT_USER_TASK).namespaceUri(BPMN20_NS).extendsType(typeof(Task)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		implementationAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_IMPLEMENTATION).defaultValue("##unspecified").build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		renderingCollection = sequenceBuilder.elementCollection(typeof(Rendering)).build();

		/// <summary>
		/// camunda extensions </summary>

		camundaAssigneeAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_ASSIGNEE).@namespace(CAMUNDA_NS).build();

		camundaCandidateGroupsAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CANDIDATE_GROUPS).@namespace(CAMUNDA_NS).build();

		camundaCandidateUsersAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CANDIDATE_USERS).@namespace(CAMUNDA_NS).build();

		camundaDueDateAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DUE_DATE).@namespace(CAMUNDA_NS).build();

		camundaFollowUpDateAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_FOLLOW_UP_DATE).@namespace(CAMUNDA_NS).build();

		camundaFormHandlerClassAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_FORM_HANDLER_CLASS).@namespace(CAMUNDA_NS).build();

		camundaFormKeyAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_FORM_KEY).@namespace(CAMUNDA_NS).build();

		camundaPriorityAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_PRIORITY).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<UserTask>
	  {
		  public UserTask newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new UserTaskImpl(instanceContext);
		  }
	  }

	  public UserTaskImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override UserTaskBuilder builder()
	  {
		return new UserTaskBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  public virtual string Implementation
	  {
		  get
		  {
			return implementationAttribute.getValue(this);
		  }
		  set
		  {
			implementationAttribute.setValue(this, value);
		  }
	  }


	  public virtual ICollection<Rendering> Renderings
	  {
		  get
		  {
			return renderingCollection.get(this);
		  }
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  public virtual string CamundaAssignee
	  {
		  get
		  {
			return camundaAssigneeAttribute.getValue(this);
		  }
		  set
		  {
			camundaAssigneeAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaCandidateGroups
	  {
		  get
		  {
			return camundaCandidateGroupsAttribute.getValue(this);
		  }
		  set
		  {
			camundaCandidateGroupsAttribute.setValue(this, value);
		  }
	  }


	  public virtual IList<string> CamundaCandidateGroupsList
	  {
		  get
		  {
			string candidateGroups = camundaCandidateGroupsAttribute.getValue(this);
			return StringUtil.splitCommaSeparatedList(candidateGroups);
		  }
		  set
		  {
			string candidateGroups = StringUtil.joinCommaSeparatedList(value);
			camundaCandidateGroupsAttribute.setValue(this, candidateGroups);
		  }
	  }


	  public virtual string CamundaCandidateUsers
	  {
		  get
		  {
			return camundaCandidateUsersAttribute.getValue(this);
		  }
		  set
		  {
			camundaCandidateUsersAttribute.setValue(this, value);
		  }
	  }


	  public virtual IList<string> CamundaCandidateUsersList
	  {
		  get
		  {
			string candidateUsers = camundaCandidateUsersAttribute.getValue(this);
			return StringUtil.splitCommaSeparatedList(candidateUsers);
		  }
		  set
		  {
			string candidateUsers = StringUtil.joinCommaSeparatedList(value);
			camundaCandidateUsersAttribute.setValue(this, candidateUsers);
		  }
	  }


	  public virtual string CamundaDueDate
	  {
		  get
		  {
			return camundaDueDateAttribute.getValue(this);
		  }
		  set
		  {
			camundaDueDateAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaFollowUpDate
	  {
		  get
		  {
			return camundaFollowUpDateAttribute.getValue(this);
		  }
		  set
		  {
			camundaFollowUpDateAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaFormHandlerClass
	  {
		  get
		  {
			return camundaFormHandlerClassAttribute.getValue(this);
		  }
		  set
		  {
			camundaFormHandlerClassAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaFormKey
	  {
		  get
		  {
			return camundaFormKeyAttribute.getValue(this);
		  }
		  set
		  {
			camundaFormKeyAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaPriority
	  {
		  get
		  {
			return camundaPriorityAttribute.getValue(this);
		  }
		  set
		  {
			camundaPriorityAttribute.setValue(this, value);
		  }
	  }


	}

}