using System;

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
namespace org.camunda.bpm.model.bpmn.builder
{
	using org.camunda.bpm.model.bpmn.instance;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using CamundaExecutionListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaExecutionListener;
	using CamundaFailedJobRetryTimeCycle = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFailedJobRetryTimeCycle;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractFlowNodeBuilder<B, E> : AbstractFlowElementBuilder<B, E> where B : AbstractFlowNodeBuilder<B, E> where E : FlowNode
	{

	  private SequenceFlowBuilder currentSequenceFlowBuilder;

	  protected internal bool compensationStarted;
	  protected internal BoundaryEvent compensateBoundaryEvent;

	  protected internal AbstractFlowNodeBuilder(BpmnModelInstance modelInstance, E element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  private SequenceFlowBuilder CurrentSequenceFlowBuilder
	  {
		  get
		  {
			if (currentSequenceFlowBuilder == null)
			{
			  SequenceFlow sequenceFlow = createSibling(typeof(SequenceFlow));
			  currentSequenceFlowBuilder = sequenceFlow.builder();
			}
			return currentSequenceFlowBuilder;
		  }
	  }

	  public virtual B condition(string name, string condition)
	  {
		if (!string.ReferenceEquals(name, null))
		{
		  CurrentSequenceFlowBuilder.name(name);
		}
		ConditionExpression conditionExpression = createInstance(typeof(ConditionExpression));
		conditionExpression.TextContent = condition;
		CurrentSequenceFlowBuilder.condition(conditionExpression);
		return myself;
	  }

	  protected internal virtual void connectTarget(FlowNode target)
	  {
		// check if compensation was started
		if (BoundaryEventWithStartedCompensation)
		{
			// the target activity should be marked for compensation
			if (target is Activity)
			{
			  ((Activity) target).ForCompensation = true;
			}

			// connect the target via association instead of sequence flow
			connectTargetWithAssociation(target);
		}
		else if (CompensationHandler)
		{
		  // cannot connect to a compensation handler
		  throw new BpmnModelException("Only single compensation handler allowed. Call compensationDone() to continue main flow.");
		}
		else
		{
		  // connect as sequence flow by default
		  connectTargetWithSequenceFlow(target);
		}
	  }

	  protected internal virtual void connectTargetWithSequenceFlow(FlowNode target)
	  {
		CurrentSequenceFlowBuilder.from(element).to(target);

		SequenceFlow sequenceFlow = CurrentSequenceFlowBuilder.Element;
		createEdge(sequenceFlow);
		currentSequenceFlowBuilder = null;
	  }

	  protected internal virtual void connectTargetWithAssociation(FlowNode target)
	  {
		Association association = modelInstance.newInstance(typeof(Association));
		association.Target = target;
		association.Source = element;
		association.AssociationDirection = AssociationDirection.One;
		element.ParentElement.addChildElement(association);

		createEdge(association);
	  }

	  public virtual AbstractFlowNodeBuilder compensationDone()
	  {
		if (compensateBoundaryEvent != null)
		{
		  return compensateBoundaryEvent.AttachedTo.builder();
		}
		else
		{
		  throw new BpmnModelException("No compensation in progress. Call compensationStart() first.");
		}
	  }

	  public virtual B sequenceFlowId(string sequenceFlowId)
	  {
		CurrentSequenceFlowBuilder.id(sequenceFlowId);
		return myself;
	  }

	  private T createTarget<T>(Type typeClass) where T : FlowNode
	  {
			  typeClass = typeof(T);
		return createTarget(typeClass, null);
	  }

	  protected internal virtual T createTarget<T>(Type typeClass, string identifier) where T : FlowNode
	  {
			  typeClass = typeof(T);
		T target = createSibling(typeClass, identifier);

		BpmnShape targetBpmnShape = createBpmnShape(target);
		Coordinates = targetBpmnShape;
		connectTarget(target);
		resizeSubProcess(targetBpmnShape);
		return target;
	  }

	  protected internal virtual T createTargetBuilder<T, F>(Type typeClass) where T : AbstractFlowNodeBuilder where F : FlowNode
	  {
			  typeClass = typeof(F);
		return createTargetBuilder(typeClass, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected <T extends AbstractFlowNodeBuilder, F extends FlowNode> T createTargetBuilder(Class<F> typeClass, String id)
	  protected internal virtual T createTargetBuilder<T, F>(Type typeClass, string id) where T : AbstractFlowNodeBuilder where F : FlowNode
	  {
			  typeClass = typeof(F);
		AbstractFlowNodeBuilder builder = createTarget(typeClass, id).builder();

		if (compensationStarted)
		{
		  // pass on current boundary event to return after compensationDone call
		  builder.compensateBoundaryEvent = compensateBoundaryEvent;
		}

		return (T) builder;

	  }

	  public virtual ServiceTaskBuilder serviceTask()
	  {
		return createTargetBuilder(typeof(ServiceTask));
	  }

	  public virtual ServiceTaskBuilder serviceTask(string id)
	  {
		return createTargetBuilder(typeof(ServiceTask), id);
	  }

	  public virtual SendTaskBuilder sendTask()
	  {
		return createTargetBuilder(typeof(SendTask));
	  }

	  public virtual SendTaskBuilder sendTask(string id)
	  {
		return createTargetBuilder(typeof(SendTask), id);
	  }

	  public virtual UserTaskBuilder userTask()
	  {
		return createTargetBuilder(typeof(UserTask));
	  }

	  public virtual UserTaskBuilder userTask(string id)
	  {
		return createTargetBuilder(typeof(UserTask), id);
	  }

	  public virtual BusinessRuleTaskBuilder businessRuleTask()
	  {
		return createTargetBuilder(typeof(BusinessRuleTask));
	  }

	  public virtual BusinessRuleTaskBuilder businessRuleTask(string id)
	  {
		return createTargetBuilder(typeof(BusinessRuleTask), id);
	  }

	  public virtual ScriptTaskBuilder scriptTask()
	  {
		return createTargetBuilder(typeof(ScriptTask));
	  }

	  public virtual ScriptTaskBuilder scriptTask(string id)
	  {
		return createTargetBuilder(typeof(ScriptTask), id);
	  }

	  public virtual ReceiveTaskBuilder receiveTask()
	  {
		return createTargetBuilder(typeof(ReceiveTask));
	  }

	  public virtual ReceiveTaskBuilder receiveTask(string id)
	  {
		return createTargetBuilder(typeof(ReceiveTask), id);
	  }

	  public virtual ManualTaskBuilder manualTask()
	  {
		return createTargetBuilder(typeof(ManualTask));
	  }

	  public virtual ManualTaskBuilder manualTask(string id)
	  {
		return createTargetBuilder(typeof(ManualTask), id);
	  }

	  public virtual EndEventBuilder endEvent()
	  {
		return createTarget(typeof(EndEvent)).builder();
	  }

	  public virtual EndEventBuilder endEvent(string id)
	  {
		return createTarget(typeof(EndEvent), id).builder();
	  }

	  public virtual ParallelGatewayBuilder parallelGateway()
	  {
		return createTarget(typeof(ParallelGateway)).builder();
	  }

	  public virtual ParallelGatewayBuilder parallelGateway(string id)
	  {
		return createTarget(typeof(ParallelGateway), id).builder();
	  }

	  public virtual ExclusiveGatewayBuilder exclusiveGateway()
	  {
		return createTarget(typeof(ExclusiveGateway)).builder();
	  }

	  public virtual InclusiveGatewayBuilder inclusiveGateway()
	  {
		return createTarget(typeof(InclusiveGateway)).builder();
	  }

	  public virtual EventBasedGatewayBuilder eventBasedGateway()
	  {
		return createTarget(typeof(EventBasedGateway)).builder();
	  }

	  public virtual ExclusiveGatewayBuilder exclusiveGateway(string id)
	  {
		return createTarget(typeof(ExclusiveGateway), id).builder();
	  }

	  public virtual InclusiveGatewayBuilder inclusiveGateway(string id)
	  {
		return createTarget(typeof(InclusiveGateway), id).builder();
	  }

	  public virtual IntermediateCatchEventBuilder intermediateCatchEvent()
	  {
		return createTarget(typeof(IntermediateCatchEvent)).builder();
	  }

	  public virtual IntermediateCatchEventBuilder intermediateCatchEvent(string id)
	  {
		return createTarget(typeof(IntermediateCatchEvent), id).builder();
	  }

	  public virtual IntermediateThrowEventBuilder intermediateThrowEvent()
	  {
		return createTarget(typeof(IntermediateThrowEvent)).builder();
	  }

	  public virtual IntermediateThrowEventBuilder intermediateThrowEvent(string id)
	  {
		return createTarget(typeof(IntermediateThrowEvent), id).builder();
	  }

	  public virtual CallActivityBuilder callActivity()
	  {
		return createTarget(typeof(CallActivity)).builder();
	  }

	  public virtual CallActivityBuilder callActivity(string id)
	  {
		return createTarget(typeof(CallActivity), id).builder();
	  }

	  public virtual SubProcessBuilder subProcess()
	  {
		return createTarget(typeof(SubProcess)).builder();
	  }

	  public virtual SubProcessBuilder subProcess(string id)
	  {
		return createTarget(typeof(SubProcess), id).builder();
	  }

	  public virtual TransactionBuilder transaction()
	  {
		Transaction transaction = createTarget(typeof(Transaction));
		return new TransactionBuilder(modelInstance, transaction);
	  }

	  public virtual TransactionBuilder transaction(string id)
	  {
		Transaction transaction = createTarget(typeof(Transaction), id);
		return new TransactionBuilder(modelInstance, transaction);
	  }

	  public virtual Gateway findLastGateway()
	  {
		FlowNode lastGateway = element;
		while (true)
		{
		  try
		  {
			lastGateway = lastGateway.PreviousNodes.singleResult();
			if (lastGateway is Gateway)
			{
			  return (Gateway) lastGateway;
			}
		  }
		  catch (BpmnModelException e)
		  {
			throw new BpmnModelException("Unable to determine an unique previous gateway of " + lastGateway.Id, e);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public AbstractGatewayBuilder moveToLastGateway()
	  public virtual AbstractGatewayBuilder moveToLastGateway()
	  {
		return findLastGateway().builder();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public AbstractFlowNodeBuilder moveToNode(String identifier)
	  public virtual AbstractFlowNodeBuilder moveToNode(string identifier)
	  {
		ModelElementInstance instance = modelInstance.getModelElementById(identifier);
		if (instance != null && instance is FlowNode)
		{
		  return ((FlowNode) instance).builder();
		}
		else
		{
		  throw new BpmnModelException("Flow node not found for id " + identifier);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public <T extends AbstractActivityBuilder> T moveToActivity(String identifier)
	  public virtual T moveToActivity<T>(string identifier) where T : AbstractActivityBuilder
	  {
		ModelElementInstance instance = modelInstance.getModelElementById(identifier);
		if (instance != null && instance is Activity)
		{
		  return (T)((Activity) instance).builder();
		}
		else
		{
		  throw new BpmnModelException("Activity not found for id " + identifier);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public AbstractFlowNodeBuilder connectTo(String identifier)
	  public virtual AbstractFlowNodeBuilder connectTo(string identifier)
	  {
		ModelElementInstance target = modelInstance.getModelElementById(identifier);
		if (target == null)
		{
		  throw new BpmnModelException("Unable to connect " + element.Id + " to element " + identifier + " cause it not exists.");
		}
		else if (!(target is FlowNode))
		{
		  throw new BpmnModelException("Unable to connect " + element.Id + " to element " + identifier + " cause its not a flow node.");
		}
		else
		{
		  FlowNode targetNode = (FlowNode) target;
		  connectTarget(targetNode);
		  return targetNode.builder();
		}
	  }

	  /// <summary>
	  /// Sets the Camunda AsyncBefore attribute for the build flow node.
	  /// </summary>
	  /// <param name="asyncBefore">
	  ///          boolean value to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaAsyncBefore(bool asyncBefore)
	  {
		element.CamundaAsyncBefore = asyncBefore;
		return myself;
	  }

	  /// <summary>
	  /// Sets the Camunda asyncBefore attribute to true.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B camundaAsyncBefore()
	  {
		element.CamundaAsyncBefore = true;
		return myself;
	  }

	  /// <summary>
	  /// Sets the Camunda asyncAfter attribute for the build flow node.
	  /// </summary>
	  /// <param name="asyncAfter">
	  ///          boolean value to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaAsyncAfter(bool asyncAfter)
	  {
		element.CamundaAsyncAfter = asyncAfter;
		return myself;
	  }

	  /// <summary>
	  /// Sets the Camunda asyncAfter attribute to true.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B camundaAsyncAfter()
	  {
		element.CamundaAsyncAfter = true;
		return myself;
	  }

	  /// <summary>
	  /// Sets the Camunda exclusive attribute to true.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B notCamundaExclusive()
	  {
		element.CamundaExclusive = false;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda exclusive attribute for the build flow node.
	  /// </summary>
	  /// <param name="exclusive">
	  ///          boolean value to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaExclusive(bool exclusive)
	  {
		element.CamundaExclusive = exclusive;
		return myself;
	  }

	  public virtual B camundaJobPriority(string jobPriority)
	  {
		element.CamundaJobPriority = jobPriority;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda failedJobRetryTimeCycle attribute for the build flow node.
	  /// </summary>
	  /// <param name="retryTimeCycle">
	  ///          the retry time cycle value to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaFailedJobRetryTimeCycle(string retryTimeCycle)
	  {
		CamundaFailedJobRetryTimeCycle failedJobRetryTimeCycle = createInstance(typeof(CamundaFailedJobRetryTimeCycle));
		failedJobRetryTimeCycle.TextContent = retryTimeCycle;

		addExtensionElement(failedJobRetryTimeCycle);

		return myself;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public B camundaExecutionListenerClass(String eventName, Class listenerClass)
	  public virtual B camundaExecutionListenerClass(string eventName, Type listenerClass)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return camundaExecutionListenerClass(eventName, listenerClass.FullName);
	  }

	  public virtual B camundaExecutionListenerClass(string eventName, string fullQualifiedClassName)
	  {
		CamundaExecutionListener executionListener = createInstance(typeof(CamundaExecutionListener));
		executionListener.CamundaEvent = eventName;
		executionListener.CamundaClass = fullQualifiedClassName;

		addExtensionElement(executionListener);

		return myself;
	  }

	  public virtual B camundaExecutionListenerExpression(string eventName, string expression)
	  {
		CamundaExecutionListener executionListener = createInstance(typeof(CamundaExecutionListener));
		executionListener.CamundaEvent = eventName;
		executionListener.CamundaExpression = expression;

		addExtensionElement(executionListener);

		return myself;
	  }

	  public virtual B camundaExecutionListenerDelegateExpression(string eventName, string delegateExpression)
	  {
		CamundaExecutionListener executionListener = createInstance(typeof(CamundaExecutionListener));
		executionListener.CamundaEvent = eventName;
		executionListener.CamundaDelegateExpression = delegateExpression;

		addExtensionElement(executionListener);

		return myself;
	  }

	  public virtual B compensationStart()
	  {
		if (element is BoundaryEvent)
		{
		  BoundaryEvent boundaryEvent = (BoundaryEvent) element;
		  foreach (EventDefinition eventDefinition in boundaryEvent.EventDefinitions)
		  {
			if (eventDefinition is CompensateEventDefinition)
			{
			  // if the boundary event contains a compensate event definition then
			  // save the boundary event to later return to it and start a compensation

			  compensateBoundaryEvent = boundaryEvent;
			  compensationStarted = true;

			  return myself;
			}
		  }
		}

		throw new BpmnModelException("Compensation can only be started on a boundary event with a compensation event definition");
	  }

	  protected internal virtual bool BoundaryEventWithStartedCompensation
	  {
		  get
		  {
			return compensationStarted && compensateBoundaryEvent != null;
		  }
	  }

	  protected internal virtual bool CompensationHandler
	  {
		  get
		  {
			return !compensationStarted && compensateBoundaryEvent != null;
		  }
	  }

	}

}