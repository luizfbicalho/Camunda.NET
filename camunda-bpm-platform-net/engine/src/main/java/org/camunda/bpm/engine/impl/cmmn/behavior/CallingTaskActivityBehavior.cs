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
namespace org.camunda.bpm.engine.impl.cmmn.behavior
{
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using BaseCallableElement = org.camunda.bpm.engine.impl.core.model.BaseCallableElement;
	using CallableElementBinding = org.camunda.bpm.engine.impl.core.model.BaseCallableElement.CallableElementBinding;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class CallingTaskActivityBehavior : TaskActivityBehavior
	{

	  protected internal new static readonly CmmnBehaviorLogger LOG = ProcessEngineLogger.CMNN_BEHAVIOR_LOGGER;

	  protected internal BaseCallableElement callableElement;

	  public override void onManualCompletion(CmmnActivityExecution execution)
	  {
		// Throw always an exception!
		// It should not be possible to complete a calling
		// task manually. If the called instance has
		// been completed, the associated task will
		// be notified to complete automatically.
		string id = execution.Id;
		throw LOG.forbiddenManualCompletitionException("complete", id, TypeName);
	  }

	  public virtual BaseCallableElement CallableElement
	  {
		  get
		  {
			return callableElement;
		  }
		  set
		  {
			this.callableElement = value;
		  }
	  }


	  protected internal virtual string getDefinitionKey(CmmnActivityExecution execution)
	  {
		CmmnExecution caseExecution = (CmmnExecution) execution;
		return CallableElement.getDefinitionKey(caseExecution);
	  }

	  protected internal virtual int? getVersion(CmmnActivityExecution execution)
	  {
		CmmnExecution caseExecution = (CmmnExecution) execution;
		return CallableElement.getVersion(caseExecution);
	  }

	  protected internal virtual string getDeploymentId(CmmnActivityExecution execution)
	  {
		return CallableElement.DeploymentId;
	  }

	  protected internal virtual BaseCallableElement.CallableElementBinding Binding
	  {
		  get
		  {
			return CallableElement.Binding;
		  }
	  }

	  protected internal virtual bool LatestBinding
	  {
		  get
		  {
			return CallableElement.LatestBinding;
		  }
	  }

	  protected internal virtual bool DeploymentBinding
	  {
		  get
		  {
			return CallableElement.DeploymentBinding;
		  }
	  }

	  protected internal virtual bool VersionBinding
	  {
		  get
		  {
			return CallableElement.VersionBinding;
		  }
	  }

	  protected internal virtual bool VersionTagBinding
	  {
		  get
		  {
			return CallableElement.VersionTagBinding;
		  }
	  }

	}

}