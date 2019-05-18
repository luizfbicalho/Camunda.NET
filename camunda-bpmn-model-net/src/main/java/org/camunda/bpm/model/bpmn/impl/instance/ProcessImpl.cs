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
	using ProcessBuilder = org.camunda.bpm.model.bpmn.builder.ProcessBuilder;
	using org.camunda.bpm.model.bpmn.instance;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using StringUtil = org.camunda.bpm.model.xml.impl.util.StringUtil;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;


	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN process element
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
	public class ProcessImpl : CallableElementImpl, Process
	{

	  protected internal static Attribute<ProcessType> processTypeAttribute;
	  protected internal static Attribute<bool> isClosedAttribute;
	  protected internal static Attribute<bool> isExecutableAttribute;
	  // TODO: definitionalCollaborationRef
	  protected internal static ChildElement<Auditing> auditingChild;
	  protected internal static ChildElement<Monitoring> monitoringChild;
	  protected internal static ChildElementCollection<Property> propertyCollection;
	  protected internal static ChildElementCollection<LaneSet> laneSetCollection;
	  protected internal static ChildElementCollection<FlowElement> flowElementCollection;
	  protected internal static ChildElementCollection<Artifact> artifactCollection;
	  protected internal static ChildElementCollection<ResourceRole> resourceRoleCollection;
	  protected internal static ChildElementCollection<CorrelationSubscription> correlationSubscriptionCollection;
	  protected internal static ElementReferenceCollection<Process, Supports> supportsCollection;

	  /// <summary>
	  /// camunda extensions </summary>

	  protected internal static Attribute<string> camundaCandidateStarterGroupsAttribute;
	  protected internal static Attribute<string> camundaCandidateStarterUsersAttribute;
	  protected internal static Attribute<string> camundaJobPriorityAttribute;
	  protected internal static Attribute<string> camundaTaskPriorityAttribute;
	  protected internal static Attribute<string> camundaHistoryTimeToLiveAttribute;
	  protected internal static Attribute<bool> camundaIsStartableInTasklistAttribute;
	  protected internal static Attribute<string> camundaVersionTagAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Process), BPMN_ELEMENT_PROCESS).namespaceUri(BPMN20_NS).extendsType(typeof(CallableElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		processTypeAttribute = typeBuilder.enumAttribute(BPMN_ATTRIBUTE_PROCESS_TYPE, typeof(ProcessType)).defaultValue(ProcessType.None).build();

		isClosedAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_IS_CLOSED).defaultValue(false).build();

		isExecutableAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_IS_EXECUTABLE).build();

		// TODO: definitionalCollaborationRef

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		auditingChild = sequenceBuilder.element(typeof(Auditing)).build();

		monitoringChild = sequenceBuilder.element(typeof(Monitoring)).build();

		propertyCollection = sequenceBuilder.elementCollection(typeof(Property)).build();

		laneSetCollection = sequenceBuilder.elementCollection(typeof(LaneSet)).build();

		flowElementCollection = sequenceBuilder.elementCollection(typeof(FlowElement)).build();

		artifactCollection = sequenceBuilder.elementCollection(typeof(Artifact)).build();

		resourceRoleCollection = sequenceBuilder.elementCollection(typeof(ResourceRole)).build();

		correlationSubscriptionCollection = sequenceBuilder.elementCollection(typeof(CorrelationSubscription)).build();

		supportsCollection = sequenceBuilder.elementCollection(typeof(Supports)).qNameElementReferenceCollection(typeof(Process)).build();

		/// <summary>
		/// camunda extensions </summary>

		camundaCandidateStarterGroupsAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CANDIDATE_STARTER_GROUPS).@namespace(CAMUNDA_NS).build();

		camundaCandidateStarterUsersAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CANDIDATE_STARTER_USERS).@namespace(CAMUNDA_NS).build();

		camundaJobPriorityAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_JOB_PRIORITY).@namespace(CAMUNDA_NS).build();

		camundaTaskPriorityAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_TASK_PRIORITY).@namespace(CAMUNDA_NS).build();

		camundaHistoryTimeToLiveAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_HISTORY_TIME_TO_LIVE).@namespace(CAMUNDA_NS).build();

		camundaIsStartableInTasklistAttribute = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_IS_STARTABLE_IN_TASKLIST).defaultValue(true).@namespace(CAMUNDA_NS).build();

		camundaVersionTagAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_VERSION_TAG).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<Process>
	  {
		  public Process newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ProcessImpl(instanceContext);
		  }
	  }

	  public ProcessImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override ProcessBuilder builder()
	  {
		return new ProcessBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  public virtual ProcessType ProcessType
	  {
		  get
		  {
			return processTypeAttribute.getValue(this);
		  }
		  set
		  {
			processTypeAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool Closed
	  {
		  get
		  {
			return isClosedAttribute.getValue(this);
		  }
		  set
		  {
			isClosedAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool Executable
	  {
		  get
		  {
			return isExecutableAttribute.getValue(this);
		  }
		  set
		  {
			isExecutableAttribute.setValue(this, value);
		  }
	  }


	  public virtual Auditing Auditing
	  {
		  get
		  {
			return auditingChild.getChild(this);
		  }
		  set
		  {
			auditingChild.setChild(this, value);
		  }
	  }


	  public virtual Monitoring Monitoring
	  {
		  get
		  {
			return monitoringChild.getChild(this);
		  }
		  set
		  {
			monitoringChild.setChild(this, value);
		  }
	  }


	  public virtual ICollection<Property> Properties
	  {
		  get
		  {
			return propertyCollection.get(this);
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

	  public virtual ICollection<CorrelationSubscription> CorrelationSubscriptions
	  {
		  get
		  {
			return correlationSubscriptionCollection.get(this);
		  }
	  }

	  public virtual ICollection<ResourceRole> ResourceRoles
	  {
		  get
		  {
			return resourceRoleCollection.get(this);
		  }
	  }

	  public virtual ICollection<Process> Supports
	  {
		  get
		  {
			return supportsCollection.getReferenceTargetElements(this);
		  }
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  public virtual string CamundaCandidateStarterGroups
	  {
		  get
		  {
			return camundaCandidateStarterGroupsAttribute.getValue(this);
		  }
		  set
		  {
			camundaCandidateStarterGroupsAttribute.setValue(this, value);
		  }
	  }


	  public virtual IList<string> CamundaCandidateStarterGroupsList
	  {
		  get
		  {
			string groupsString = camundaCandidateStarterGroupsAttribute.getValue(this);
			return StringUtil.splitCommaSeparatedList(groupsString);
		  }
		  set
		  {
			string candidateStarterGroups = StringUtil.joinCommaSeparatedList(value);
			camundaCandidateStarterGroupsAttribute.setValue(this, candidateStarterGroups);
		  }
	  }


	  public virtual string CamundaCandidateStarterUsers
	  {
		  get
		  {
			return camundaCandidateStarterUsersAttribute.getValue(this);
		  }
		  set
		  {
			camundaCandidateStarterUsersAttribute.setValue(this, value);
		  }
	  }


	  public virtual IList<string> CamundaCandidateStarterUsersList
	  {
		  get
		  {
			string candidateStarterUsers = camundaCandidateStarterUsersAttribute.getValue(this);
			return StringUtil.splitCommaSeparatedList(candidateStarterUsers);
		  }
		  set
		  {
			string candidateStarterUsers = StringUtil.joinCommaSeparatedList(value);
			camundaCandidateStarterUsersAttribute.setValue(this, candidateStarterUsers);
		  }
	  }


	  public virtual string CamundaJobPriority
	  {
		  get
		  {
			return camundaJobPriorityAttribute.getValue(this);
		  }
		  set
		  {
			camundaJobPriorityAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaTaskPriority
	  {
		  get
		  {
			return camundaTaskPriorityAttribute.getValue(this);
		  }
		  set
		  {
			camundaTaskPriorityAttribute.setValue(this, value);
		  }
	  }


	  public virtual int? CamundaHistoryTimeToLive
	  {
		  get
		  {
			string ttl = CamundaHistoryTimeToLiveString;
			if (!string.ReferenceEquals(ttl, null))
			{
			  return int.Parse(ttl);
			}
			return null;
		  }
		  set
		  {
			CamundaHistoryTimeToLiveString = value.ToString();
		  }
	  }


	  public virtual string CamundaHistoryTimeToLiveString
	  {
		  get
		  {
			return camundaHistoryTimeToLiveAttribute.getValue(this);
		  }
		  set
		  {
			camundaHistoryTimeToLiveAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool? CamundaStartableInTasklist
	  {
		  get
		  {
			return camundaIsStartableInTasklistAttribute.getValue(this);
		  }
	  }

	  public virtual bool? CamundaIsStartableInTasklist
	  {
		  set
		  {
			camundaIsStartableInTasklistAttribute.setValue(this, value);
		  }
	  }

	  public virtual string CamundaVersionTag
	  {
		  get
		  {
			return camundaVersionTagAttribute.getValue(this);
		  }
		  set
		  {
			camundaVersionTagAttribute.setValue(this, value);
		  }
	  }

	}

}