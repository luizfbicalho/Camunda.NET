using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.engine.test.util
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using Execution = org.camunda.bpm.engine.runtime.Execution;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExecutionTree : Execution
	{

	  protected internal ExecutionTree parent;
	  protected internal IList<ExecutionTree> children;
	  protected internal Execution wrappedExecution;

	  protected internal ExecutionTree(Execution execution, IList<ExecutionTree> children)
	  {
		this.wrappedExecution = execution;
		this.children = children;
		foreach (ExecutionTree child in children)
		{
		  child.parent = this;
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static ExecutionTree forExecution(final String executionId, org.camunda.bpm.engine.ProcessEngine processEngine)
	  public static ExecutionTree forExecution(string executionId, ProcessEngine processEngine)
	  {
		ProcessEngineConfigurationImpl configuration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;

		CommandExecutor commandExecutor = configuration.CommandExecutorTxRequired;

		ExecutionTree executionTree = commandExecutor.execute(new CommandAnonymousInnerClass(executionId));

		return executionTree;
	  }

	  private class CommandAnonymousInnerClass : Command<ExecutionTree>
	  {
		  private string executionId;

		  public CommandAnonymousInnerClass(string executionId)
		  {
			  this.executionId = executionId;
		  }

		  public ExecutionTree execute(CommandContext commandContext)
		  {
			ExecutionEntity execution = commandContext.ExecutionManager.findExecutionById(executionId);
			return ExecutionTree.forExecution(execution);
		  }
	  }

	  protected internal static ExecutionTree forExecution(ExecutionEntity execution)
	  {
		IList<ExecutionTree> children = new List<ExecutionTree>();

		foreach (ExecutionEntity child in execution.Executions)
		{
		  children.Add(ExecutionTree.forExecution(child));
		}

		return new ExecutionTree(execution, children);

	  }

	  public virtual IList<ExecutionTree> Executions
	  {
		  get
		  {
			return children;
		  }
	  }

	  public virtual IList<ExecutionTree> getLeafExecutions(string activityId)
	  {
		IList<ExecutionTree> executions = new List<ExecutionTree>();

		foreach (ExecutionTree child in children)
		{
		  if (!child.EventScope.Value)
		  {
			if (!string.ReferenceEquals(child.ActivityId, null))
			{
			  if (activityId.Equals(child.ActivityId))
			  {
				executions.Add(child);
			  }
			}
			else
			{
			  ((IList<ExecutionTree>)executions).AddRange(child.getLeafExecutions(activityId));
			}
		  }
		}

		return executions;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return wrappedExecution.Id;
		  }
	  }

	  public virtual bool Suspended
	  {
		  get
		  {
			return wrappedExecution.Suspended;
		  }
	  }

	  public virtual bool Ended
	  {
		  get
		  {
			return wrappedExecution.Ended;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return wrappedExecution.ProcessInstanceId;
		  }
	  }

	  public virtual ExecutionTree Parent
	  {
		  get
		  {
			return parent;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return ((PvmExecutionImpl) wrappedExecution).ActivityId;
		  }
	  }

	  public virtual bool? Scope
	  {
		  get
		  {
			return ((PvmExecutionImpl) wrappedExecution).Scope;
		  }
	  }

	  public virtual bool? Concurrent
	  {
		  get
		  {
			return ((PvmExecutionImpl) wrappedExecution).Concurrent;
		  }
	  }

	  public virtual bool? EventScope
	  {
		  get
		  {
			return ((PvmExecutionImpl) wrappedExecution).EventScope;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return wrappedExecution.TenantId;
		  }
	  }

	  public virtual Execution Execution
	  {
		  get
		  {
			return wrappedExecution;
		  }
	  }

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		appendString("", sb);
		return sb.ToString();
	  }

	  public virtual void appendString(string prefix, StringBuilder sb)
	  {
		sb.Append(prefix);
		sb.Append(executionTreeToString(this));
		sb.Append("\n");
		foreach (ExecutionTree child in Executions)
		{
		  child.appendString(prefix + "   ", sb);
		}
	  }

	  protected internal static string executionTreeToString(ExecutionTree executionTree)
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append(executionTree.Execution);

		sb.Append("[activityId=");
		sb.Append(executionTree.ActivityId);

		sb.Append(", isScope=");
		sb.Append(executionTree.Scope);

		sb.Append(", isConcurrent=");
		sb.Append(executionTree.Concurrent);

		sb.Append(", isEventScope=");
		sb.Append(executionTree.EventScope);

		sb.Append("]");

		return sb.ToString();
	  }

	}

}