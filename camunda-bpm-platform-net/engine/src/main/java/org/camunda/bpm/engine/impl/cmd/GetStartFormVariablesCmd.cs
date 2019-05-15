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
namespace org.camunda.bpm.engine.impl.cmd
{

	using FormField = org.camunda.bpm.engine.form.FormField;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class GetStartFormVariablesCmd : AbstractGetFormVariablesCmd
	{

	  private const long serialVersionUID = 1L;

	  public GetStartFormVariablesCmd(string resourceId, ICollection<string> formVariableNames, bool deserializeObjectValues) : base(resourceId, formVariableNames, deserializeObjectValues)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.variable.VariableMap execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public override VariableMap execute(CommandContext commandContext)
	  {
		StartFormData startFormData = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));

		ProcessDefinition definition = startFormData.ProcessDefinition;
		checkGetStartFormVariables((ProcessDefinitionEntity) definition, commandContext);

		VariableMap result = new VariableMapImpl();

		foreach (FormField formField in startFormData.FormFields)
		{
		  if (formVariableNames == null || formVariableNames.Contains(formField.Id))
		  {
			result.put(formField.Id, createVariable(formField, null));
		  }
		}

		return result;
	  }

	  private class CallableAnonymousInnerClass : Callable<StartFormData>
	  {
		  private readonly GetStartFormVariablesCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(GetStartFormVariablesCmd outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.form.StartFormData call() throws Exception
		  public StartFormData call()
		  {
			return (new GetStartFormCmd(outerInstance.resourceId)).execute(commandContext);
		  }
	  }

	  protected internal virtual void checkGetStartFormVariables(ProcessDefinitionEntity definition, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadProcessDefinition(definition);
		}
	  }
	}

}