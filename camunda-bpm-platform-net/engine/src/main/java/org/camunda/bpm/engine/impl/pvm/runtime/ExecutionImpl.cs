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
namespace org.camunda.bpm.engine.impl.pvm.runtime
{

	using BpmnModelExecutionContext = org.camunda.bpm.engine.@delegate.BpmnModelExecutionContext;
	using ProcessEngineServicesAware = org.camunda.bpm.engine.@delegate.ProcessEngineServicesAware;
	using CaseExecutionImpl = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionImpl;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CoreVariableInstance = org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance;
	using SimpleVariableInstanceFactory = org.camunda.bpm.engine.impl.core.variable.scope.SimpleVariableInstance.SimpleVariableInstanceFactory;
	using VariableInstanceFactory = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceFactory;
	using VariableInstanceLifecycleListener = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceLifecycleListener;
	using VariableStore = org.camunda.bpm.engine.impl.core.variable.scope.VariableStore;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using FlowElement = org.camunda.bpm.model.bpmn.instance.FlowElement;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public class ExecutionImpl : PvmExecutionImpl, ActivityExecution, PvmProcessInstance
	{

	  private const long serialVersionUID = 1L;

	  private static AtomicInteger idGenerator = new AtomicInteger();

	  // current position /////////////////////////////////////////////////////////

	  /// <summary>
	  /// the process instance.  this is the root of the execution tree.
	  /// the processInstance of a process instance is a self reference. 
	  /// </summary>
	  protected internal ExecutionImpl processInstance;

	  /// <summary>
	  /// the parent execution </summary>
	  protected internal ExecutionImpl parent;

	  /// <summary>
	  /// nested executions representing scopes or concurrent paths </summary>
	  protected internal IList<ExecutionImpl> executions;

	  /// <summary>
	  /// super execution, not-null if this execution is part of a subprocess </summary>
	  protected internal ExecutionImpl superExecution;

	  /// <summary>
	  /// reference to a subprocessinstance, not-null if currently subprocess is started from this execution </summary>
	  protected internal ExecutionImpl subProcessInstance;

	  /// <summary>
	  /// super case execution, not-null if this execution is part of a case execution </summary>
	  protected internal CaseExecutionImpl superCaseExecution;

	  /// <summary>
	  /// reference to a subcaseinstance, not-null if currently subcase is started from this execution </summary>
	  protected internal CaseExecutionImpl subCaseInstance;

	  // variables/////////////////////////////////////////////////////////////////

	  protected internal VariableStore<CoreVariableInstance> variableStore = new VariableStore<CoreVariableInstance>();

	  // lifecycle methods ////////////////////////////////////////////////////////

	  public ExecutionImpl()
	  {
	  }

	  /// <summary>
	  /// creates a new execution. properties processDefinition, processInstance and activity will be initialized. </summary>
	  public override ExecutionImpl createExecution(bool initializeExecutionStartContext)
	  {
		// create the new child execution
		ExecutionImpl createdExecution = newExecution();

		// initialize sequence counter
		createdExecution.SequenceCounter = SequenceCounter;

		// manage the bidirectional parent-child relation
		createdExecution.Parent = this;

		// initialize the new execution
		createdExecution.ProcessDefinition = ProcessDefinition;
		createdExecution.setProcessInstance(getProcessInstance());
		createdExecution.Activity = Activity;

		// make created execution start in same activity instance
		createdExecution.activityInstanceId = activityInstanceId;

		// with the fix of CAM-9249 we presume that the parent and the child have the same startContext
		if (initializeExecutionStartContext)
		{
		  createdExecution.StartContext = new ExecutionStartContext();
		}
		else if (startContext != null)
		{
		  createdExecution.StartContext = startContext;
		}

		createdExecution.skipCustomListeners = this.skipCustomListeners;
		createdExecution.skipIoMapping = this.skipIoMapping;

		return createdExecution;
	  }

	  /// <summary>
	  /// instantiates a new execution.  can be overridden by subclasses </summary>
	  protected internal override ExecutionImpl newExecution()
	  {
		return new ExecutionImpl();
	  }

	  public override void initialize()
	  {
		return;
	  }

	  public override void initializeTimerDeclarations()
	  {
		return;
	  }

	  // parent ///////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the parent </summary>
	  public override ExecutionImpl Parent
	  {
		  get
		  {
			return parent;
		  }
	  }

	  public override PvmExecutionImpl ParentExecution
	  {
		  set
		  {
			this.parent = (ExecutionImpl) value;
		  }
	  }

	  // executions ///////////////////////////////////////////////////////////////

	  public override IList<ExecutionImpl> ExecutionsAsCopy
	  {
		  get
		  {
			return new List<ExecutionImpl>(Executions);
		  }
	  }

	  /// <summary>
	  /// ensures initialization and returns the non-null executions list </summary>
	  public override IList<ExecutionImpl> Executions
	  {
		  get
		  {
			if (executions == null)
			{
			  executions = new List<ExecutionImpl>();
			}
			return executions;
		  }
		  set
		  {
			this.executions = value;
		  }
	  }

	  public override ExecutionImpl getSuperExecution()
	  {
		return superExecution;
	  }

	  public override void setSuperExecution(PvmExecutionImpl superExecution)
	  {
		this.superExecution = (ExecutionImpl) superExecution;
		if (superExecution != null)
		{
		  superExecution.SubProcessInstance = null;
		}
	  }

	  public override ExecutionImpl getSubProcessInstance()
	  {
		return subProcessInstance;
	  }

	  public override void setSubProcessInstance(PvmExecutionImpl subProcessInstance)
	  {
		this.subProcessInstance = (ExecutionImpl) subProcessInstance;
	  }

	  // super case execution /////////////////////////////////////////////////////

	  public override CaseExecutionImpl getSuperCaseExecution()
	  {
		return superCaseExecution;
	  }

	  public override void setSuperCaseExecution(CmmnExecution superCaseExecution)
	  {
		this.superCaseExecution = (CaseExecutionImpl) superCaseExecution;
	  }

	  // sub case execution ////////////////////////////////////////////////////////

	  public override CaseExecutionImpl getSubCaseInstance()
	  {
		return subCaseInstance;
	  }

	  public override void setSubCaseInstance(CmmnExecution subCaseInstance)
	  {
		this.subCaseInstance = (CaseExecutionImpl) subCaseInstance;
	  }

	  public override CaseExecutionImpl createSubCaseInstance(CmmnCaseDefinition caseDefinition)
	  {
		return createSubCaseInstance(caseDefinition, null);
	  }

	  public override CaseExecutionImpl createSubCaseInstance(CmmnCaseDefinition caseDefinition, string businessKey)
	  {
		CaseExecutionImpl caseInstance = (CaseExecutionImpl) caseDefinition.createCaseInstance(businessKey);

		// manage bidirectional super-process-sub-case-instances relation
		subCaseInstance.setSuperExecution(this);
		setSubCaseInstance(subCaseInstance);

		return caseInstance;
	  }

	  // process definition ///////////////////////////////////////////////////////

	  public override string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinition.Id;
		  }
	  }

	  // process instance /////////////////////////////////////////////////////////

	  public override void start(IDictionary<string, object> variables)
	  {
		if (ProcessInstanceExecution)
		{
		  if (startContext == null)
		  {
			startContext = new ProcessInstanceStartContext(processDefinition.Initial);
		  }
		}

		base.start(variables);
	  }


	  /// <summary>
	  /// ensures initialization and returns the process instance. </summary>
	  public override ExecutionImpl getProcessInstance()
	  {
		return processInstance;
	  }

	  public override string ProcessInstanceId
	  {
		  get
		  {
			return getProcessInstance().Id;
		  }
	  }

	  public override string BusinessKey
	  {
		  get
		  {
			return getProcessInstance().BusinessKey;
		  }
		  set
		  {
			this.businessKey = value;
		  }
	  }


	  public override string ProcessBusinessKey
	  {
		  get
		  {
			return getProcessInstance().BusinessKey;
		  }
	  }

	  /// <summary>
	  /// for setting the process instance, this setter must be used as subclasses can override </summary>
	  public override void setProcessInstance(PvmExecutionImpl processInstance)
	  {
		this.processInstance = (ExecutionImpl) processInstance;
	  }

	  // activity /////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// generates an activity instance id
	  /// </summary>
	  protected internal override string generateActivityInstanceId(string activityId)
	  {
		int nextId = idGenerator.incrementAndGet();
		string compositeId = activityId + ":" + nextId;
		if (compositeId.Length > 64)
		{
		  return nextId.ToString();
		}
		else
		{
		  return compositeId;
		}
	  }

	  // toString /////////////////////////////////////////////////////////////////

	  public override string ToString()
	  {
		if (ProcessInstanceExecution)
		{
		  return "ProcessInstance[" + ToStringIdentity + "]";
		}
		else
		{
		  return (isEventScope? "EventScope":"") + (isConcurrent? "Concurrent" : "") + (Scope ? "Scope" : "") + "Execution[" + ToStringIdentity + "]";
		}
	  }

	  protected internal override string ToStringIdentity
	  {
		  get
		  {
			return Convert.ToString(System.identityHashCode(this));
		  }
	  }

	  // allow for subclasses to expose a real id /////////////////////////////////

	  public override string Id
	  {
		  get
		  {
			return System.identityHashCode(this).ToString();
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  protected internal override VariableStore<CoreVariableInstance> VariableStore
	  {
		  get
		  {
			return variableStore;
		  }
	  }

	  protected internal override VariableInstanceFactory<CoreVariableInstance> VariableInstanceFactory
	  {
		  get
		  {
			return (VariableInstanceFactory) SimpleVariableInstanceFactory.INSTANCE;
		  }
	  }

	  protected internal override IList<VariableInstanceLifecycleListener<CoreVariableInstance>> VariableInstanceLifecycleListeners
	  {
		  get
		  {
			return Collections.emptyList();
		  }
	  }

	  public override ExecutionImpl ReplacedBy
	  {
		  get
		  {
			return (ExecutionImpl) replacedBy;
		  }
	  }


	  public override string CurrentActivityName
	  {
		  get
		  {
			string currentActivityName = null;
			if (this.activity != null)
			{
			  currentActivityName = (string) activity.getProperty("name");
			}
			return currentActivityName;
		  }
	  }

	  public override FlowElement BpmnModelElementInstance
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new System.NotSupportedException(typeof(BpmnModelExecutionContext).FullName + " is unsupported in transient ExecutionImpl");
		  }
	  }

	  public override BpmnModelInstance BpmnModelInstance
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new System.NotSupportedException(typeof(BpmnModelExecutionContext).FullName + " is unsupported in transient ExecutionImpl");
		  }
	  }

	  public override ProcessEngineServices ProcessEngineServices
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new System.NotSupportedException(typeof(ProcessEngineServicesAware).FullName + " is unsupported in transient ExecutionImpl");
		  }
	  }

	  public override ProcessEngine ProcessEngine
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new System.NotSupportedException(typeof(ProcessEngineServicesAware).FullName + " is unsupported in transient ExecutionImpl");
		  }
	  }

	  public override void forceUpdate()
	  {
		// nothing to do
	  }

	  public override void fireHistoricProcessStartEvent()
	  {
		// do nothing
	  }

	  protected internal override void removeVariablesLocalInternal()
	  {
		// do nothing
	  }

	}

}