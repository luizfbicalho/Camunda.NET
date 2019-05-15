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
namespace org.camunda.bpm.engine.impl.externaltask
{

	using ExternalTaskQueryTopicBuilder = org.camunda.bpm.engine.externaltask.ExternalTaskQueryTopicBuilder;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using FetchExternalTasksCmd = org.camunda.bpm.engine.impl.cmd.FetchExternalTasksCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// 
	/// </summary>
	public class ExternalTaskQueryTopicBuilderImpl : ExternalTaskQueryTopicBuilder
	{

	  protected internal CommandExecutor commandExecutor;

	  protected internal string workerId;
	  protected internal int maxTasks;
	  /// <summary>
	  /// Indicates that priority is enabled.
	  /// </summary>
	  protected internal bool usePriority;

	  protected internal IDictionary<string, TopicFetchInstruction> instructions;

	  protected internal TopicFetchInstruction currentInstruction;

	  public ExternalTaskQueryTopicBuilderImpl(CommandExecutor commandExecutor, string workerId, int maxTasks, bool usePriority)
	  {
		this.commandExecutor = commandExecutor;
		this.workerId = workerId;
		this.maxTasks = maxTasks;
		this.usePriority = usePriority;
		this.instructions = new Dictionary<string, TopicFetchInstruction>();
	  }

	  public virtual IList<LockedExternalTask> execute()
	  {
		submitCurrentInstruction();
		return commandExecutor.execute(new FetchExternalTasksCmd(workerId, maxTasks, instructions, usePriority));
	  }

	  public virtual ExternalTaskQueryTopicBuilder topic(string topicName, long lockDuration)
	  {
		submitCurrentInstruction();
		currentInstruction = new TopicFetchInstruction(topicName, lockDuration);
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder variables(params string[] variables)
	  {
		// don't use plain Arrays.asList since this returns an instance of a different list class
		// that is private and may mess mybatis queries up
		if (variables != null)
		{
		  currentInstruction.VariablesToFetch = new List<string>(Arrays.asList(variables));
		}
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder variables(IList<string> variables)
	  {
		currentInstruction.VariablesToFetch = variables;
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder processInstanceVariableEquals(IDictionary<string, object> variables)
	  {
		currentInstruction.setFilterVariables(variables);
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder processInstanceVariableEquals(string name, object value)
	  {
		currentInstruction.addFilterVariable(name, value);
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder businessKey(string businessKey)
	  {
		currentInstruction.BusinessKey = businessKey;
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder processDefinitionId(string processDefinitionId)
	  {
		currentInstruction.ProcessDefinitionId = processDefinitionId;
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder processDefinitionIdIn(params string[] processDefinitionIds)
	  {
		currentInstruction.ProcessDefinitionIds = processDefinitionIds;
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder processDefinitionKey(string processDefinitionKey)
	  {
		currentInstruction.ProcessDefinitionKey = processDefinitionKey;
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder processDefinitionKeyIn(params string[] processDefinitionKeys)
	  {
		currentInstruction.ProcessDefinitionKeys = processDefinitionKeys;
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder withoutTenantId()
	  {
		currentInstruction.TenantIds = null;
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder tenantIdIn(params string[] tenantIds)
	  {
		currentInstruction.TenantIds = tenantIds;
		return this;
	  }

	  protected internal virtual void submitCurrentInstruction()
	  {
		if (currentInstruction != null)
		{
		  this.instructions[currentInstruction.TopicName] = currentInstruction;
		}
	  }

	  public virtual ExternalTaskQueryTopicBuilder enableCustomObjectDeserialization()
	  {
		currentInstruction.DeserializeVariables = true;
		return this;
	  }

	  public virtual ExternalTaskQueryTopicBuilder localVariables()
	  {
		currentInstruction.LocalVariables = true;
		return this;
	  }


	}

}