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
namespace org.camunda.bpm.engine.impl.cmd
{


	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ConditionHandler = org.camunda.bpm.engine.impl.runtime.ConditionHandler;
	using ConditionHandlerResult = org.camunda.bpm.engine.impl.runtime.ConditionHandlerResult;
	using ConditionSet = org.camunda.bpm.engine.impl.runtime.ConditionSet;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// Evaluates the conditions to start processes by conditional start events
	/// 
	/// @author Yana Vasileva
	/// 
	/// </summary>
	public class EvaluateStartConditionCmd : Command<IList<ProcessInstance>>
	{

	  protected internal ConditionEvaluationBuilderImpl builder;

	  public EvaluateStartConditionCmd(ConditionEvaluationBuilderImpl builder)
	  {
		this.builder = builder;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public java.util.List<org.camunda.bpm.engine.runtime.ProcessInstance> execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual IList<ProcessInstance> execute(CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.runtime.ConditionHandler conditionHandler = commandContext.getProcessEngineConfiguration().getConditionHandler();
		ConditionHandler conditionHandler = commandContext.ProcessEngineConfiguration.ConditionHandler;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.runtime.ConditionSet conditionSet = new org.camunda.bpm.engine.impl.runtime.ConditionSet(builder);
		ConditionSet conditionSet = new ConditionSet(builder);

		IList<ConditionHandlerResult> results = conditionHandler.evaluateStartCondition(commandContext, conditionSet);

		foreach (ConditionHandlerResult ConditionHandlerResult in results)
		{
		  checkAuthorization(commandContext, ConditionHandlerResult);
		}

		IList<ProcessInstance> processInstances = new List<ProcessInstance>();
		foreach (ConditionHandlerResult ConditionHandlerResult in results)
		{
		  processInstances.Add(instantiateProcess(commandContext, ConditionHandlerResult));
		}

		return processInstances;
	  }

	  protected internal virtual void checkAuthorization(CommandContext commandContext, ConditionHandlerResult result)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  ProcessDefinitionEntity definition = result.ProcessDefinition;
		  checker.checkCreateProcessInstance(definition);
		}
	  }

	  protected internal virtual ProcessInstance instantiateProcess(CommandContext commandContext, ConditionHandlerResult result)
	  {
		ProcessDefinitionEntity processDefinitionEntity = result.ProcessDefinition;

		ActivityImpl startEvent = processDefinitionEntity.findActivity(result.Activity.ActivityId);
		ExecutionEntity processInstance = processDefinitionEntity.createProcessInstance(builder.BusinessKey, startEvent);
		processInstance.start(builder.getVariables());

		return processInstance;
	  }

	}

}