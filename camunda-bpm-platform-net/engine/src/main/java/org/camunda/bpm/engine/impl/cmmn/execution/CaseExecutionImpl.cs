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
namespace org.camunda.bpm.engine.impl.cmmn.execution
{

	using CmmnModelExecutionContext = org.camunda.bpm.engine.@delegate.CmmnModelExecutionContext;
	using ProcessEngineServicesAware = org.camunda.bpm.engine.@delegate.ProcessEngineServicesAware;
	using CmmnBehaviorLogger = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnBehaviorLogger;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CoreVariableInstance = org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance;
	using SimpleVariableInstance = org.camunda.bpm.engine.impl.core.variable.scope.SimpleVariableInstance;
	using SimpleVariableInstanceFactory = org.camunda.bpm.engine.impl.core.variable.scope.SimpleVariableInstance.SimpleVariableInstanceFactory;
	using VariableInstanceFactory = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceFactory;
	using VariableInstanceLifecycleListener = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceLifecycleListener;
	using VariableStore = org.camunda.bpm.engine.impl.core.variable.scope.VariableStore;
	using PvmProcessDefinition = org.camunda.bpm.engine.impl.pvm.PvmProcessDefinition;
	using ExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.ExecutionImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class CaseExecutionImpl : CmmnExecution
	{

	  protected internal new static readonly CmmnBehaviorLogger LOG = ProcessEngineLogger.CMNN_BEHAVIOR_LOGGER;

	  private const long serialVersionUID = 1L;

	  // current position /////////////////////////////////////////////////////////

	  protected internal IList<CaseExecutionImpl> caseExecutions;

	  protected internal IList<CaseSentryPartImpl> caseSentryParts;

	  protected internal CaseExecutionImpl caseInstance;

	  protected internal CaseExecutionImpl parent;

	  protected internal ExecutionImpl subProcessInstance;

	  protected internal ExecutionImpl superExecution;

	  protected internal CaseExecutionImpl subCaseInstance;

	  protected internal CaseExecutionImpl superCaseExecution;

	  // variables ////////////////////////////////////////////////////////////////

	  protected internal VariableStore<SimpleVariableInstance> variableStore = new VariableStore<SimpleVariableInstance>();

	  public CaseExecutionImpl()
	  {
	  }

	  // case definition id ///////////////////////////////////////////////////////

	  public override string CaseDefinitionId
	  {
		  get
		  {
			return CaseDefinition.Id;
		  }
	  }

	  // parent ////////////////////////////////////////////////////////////////////

	  public override CaseExecutionImpl getParent()
	  {
		return parent;
	  }

	  public override void setParent(CmmnExecution parent)
	  {
		this.parent = (CaseExecutionImpl) parent;
	  }

	  public override string ParentId
	  {
		  get
		  {
			return getParent().Id;
		  }
	  }

	  // activity //////////////////////////////////////////////////////////////////

	  public override string ActivityId
	  {
		  get
		  {
			return Activity.Id;
		  }
	  }

	  public override string ActivityName
	  {
		  get
		  {
			return Activity.Name;
		  }
	  }

	  // case executions ////////////////////////////////////////////////////////////////

	  public override IList<CaseExecutionImpl> CaseExecutions
	  {
		  get
		  {
			return new List<CaseExecutionImpl>(CaseExecutionsInternal);
		  }
	  }

	  protected internal override IList<CaseExecutionImpl> CaseExecutionsInternal
	  {
		  get
		  {
			if (caseExecutions == null)
			{
			  caseExecutions = new List<CaseExecutionImpl>();
			}
			return caseExecutions;
		  }
	  }

	  // case instance /////////////////////////////////////////////////////////////

	  public override CaseExecutionImpl getCaseInstance()
	  {
		return caseInstance;
	  }

	  public override void setCaseInstance(CmmnExecution caseInstance)
	  {
		this.caseInstance = (CaseExecutionImpl) caseInstance;
	  }

	  // super execution /////////////////////////////////////////////////////////////

	  public override ExecutionImpl getSuperExecution()
	  {
		return superExecution;
	  }

	  public override void setSuperExecution(PvmExecutionImpl superExecution)
	  {
		this.superExecution = (ExecutionImpl) superExecution;
	  }

	  // sub process instance ////////////////////////////////////////////////////////

	  public override ExecutionImpl getSubProcessInstance()
	  {
		return subProcessInstance;
	  }

	  public override void setSubProcessInstance(PvmExecutionImpl subProcessInstance)
	  {
		this.subProcessInstance = (ExecutionImpl) subProcessInstance;
	  }

	  public override PvmExecutionImpl createSubProcessInstance(PvmProcessDefinition processDefinition)
	  {
		return createSubProcessInstance(processDefinition, null);
	  }

	  public override PvmExecutionImpl createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey)
	  {
		return createSubProcessInstance(processDefinition, businessKey, CaseInstanceId);
	  }

	  public override PvmExecutionImpl createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey, string caseInstanceId)
	  {
		ExecutionImpl subProcessInstance = (ExecutionImpl) processDefinition.createProcessInstance(businessKey, caseInstanceId);

		// manage bidirectional super-subprocess relation
		subProcessInstance.setSuperCaseExecution(this);
		setSubProcessInstance(subProcessInstance);

		return subProcessInstance;
	  }

	  // sub-/super- case instance ////////////////////////////////////////////////////

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

		// manage bidirectional super-sub-case-instances relation
		subCaseInstance.setSuperCaseExecution(this);
		setSubCaseInstance(subCaseInstance);

		return caseInstance;
	  }

	  public override CaseExecutionImpl getSuperCaseExecution()
	  {
		return superCaseExecution;
	  }

	  public override void setSuperCaseExecution(CmmnExecution superCaseExecution)
	  {
		this.superCaseExecution = (CaseExecutionImpl) superCaseExecution;
	  }

	  // sentry /////////////////////////////////////////////////////////////////////////

	  public override IList<CaseSentryPartImpl> CaseSentryParts
	  {
		  get
		  {
			if (caseSentryParts == null)
			{
			  caseSentryParts = new List<CaseSentryPartImpl>();
			}
			return caseSentryParts;
		  }
	  }

	  protected internal override IDictionary<string, IList<CmmnSentryPart>> Sentries
	  {
		  get
		  {
			IDictionary<string, IList<CmmnSentryPart>> sentries = new Dictionary<string, IList<CmmnSentryPart>>();
    
			foreach (CaseSentryPartImpl sentryPart in CaseSentryParts)
			{
    
			  string sentryId = sentryPart.SentryId;
			  IList<CmmnSentryPart> parts = sentries[sentryId];
    
			  if (parts == null)
			  {
				parts = new List<CmmnSentryPart>();
				sentries[sentryId] = parts;
			  }
    
			  parts.Add(sentryPart);
			}
    
			return sentries;
		  }
	  }

	  protected internal override IList<CaseSentryPartImpl> findSentry(string sentryId)
	  {
		IList<CaseSentryPartImpl> result = new List<CaseSentryPartImpl>();

		foreach (CaseSentryPartImpl sentryPart in CaseSentryParts)
		{
		  if (sentryPart.SentryId.Equals(sentryId))
		  {
			result.Add(sentryPart);
		  }
		}

		return result;
	  }

	  protected internal override void addSentryPart(CmmnSentryPart sentryPart)
	  {
		CaseSentryParts.Add((CaseSentryPartImpl) sentryPart);
	  }

	  protected internal override CmmnSentryPart newSentryPart()
	  {
		return new CaseSentryPartImpl();
	  }

	  // new case executions ////////////////////////////////////////////////////////////

	  protected internal override CaseExecutionImpl createCaseExecution(CmmnActivity activity)
	  {
		CaseExecutionImpl child = newCaseExecution();

		// set activity to execute
		child.Activity = activity;

		// handle child/parent-relation
		child.setParent(this);
		CaseExecutionsInternal.Add(child);

		// set case instance
		child.setCaseInstance(getCaseInstance());

		// set case definition
		child.CaseDefinition = CaseDefinition;

		return child;
	  }

	  protected internal override CaseExecutionImpl newCaseExecution()
	  {
		return new CaseExecutionImpl();
	  }

	  // variables //////////////////////////////////////////////////////////////

	  protected internal override VariableStore<CoreVariableInstance> VariableStore
	  {
		  get
		  {
			return (VariableStore) variableStore;
		  }
	  }

	  protected internal override VariableInstanceFactory<CoreVariableInstance> VariableInstanceFactory
	  {
		  get
		  {
			return (VariableInstanceFactory) SimpleVariableInstance.SimpleVariableInstanceFactory.INSTANCE;
		  }
	  }

	  protected internal override IList<VariableInstanceLifecycleListener<CoreVariableInstance>> VariableInstanceLifecycleListeners
	  {
		  get
		  {
			return Collections.emptyList();
		  }
	  }

	  // toString /////////////////////////////////////////////////////////////////

	  public override string ToString()
	  {
		if (CaseInstanceExecution)
		{
		  return "CaseInstance[" + ToStringIdentity + "]";
		}
		else
		{
		  return "CmmnExecution[" + ToStringIdentity + "]";
		}
	  }

	  protected internal override string ToStringIdentity
	  {
		  get
		  {
			return Convert.ToString(System.identityHashCode(this));
		  }
	  }

	  public override string Id
	  {
		  get
		  {
			return System.identityHashCode(this).ToString();
		  }
	  }

	  public override ProcessEngineServices ProcessEngineServices
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw LOG.unsupportedTransientOperationException(typeof(ProcessEngineServicesAware).FullName);
		  }
	  }

	  public override ProcessEngine ProcessEngine
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw LOG.unsupportedTransientOperationException(typeof(ProcessEngineServicesAware).FullName);
		  }
	  }

	  public override CmmnElement CmmnModelElementInstance
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw LOG.unsupportedTransientOperationException(typeof(CmmnModelExecutionContext).FullName);
		  }
	  }

	  public override CmmnModelInstance CmmnModelInstance
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw LOG.unsupportedTransientOperationException(typeof(CmmnModelExecutionContext).FullName);
		  }
	  }
	}

}