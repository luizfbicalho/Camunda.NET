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
namespace org.camunda.bpm.engine.impl.cmmn
{

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using CaseExecutionNotFoundException = org.camunda.bpm.engine.exception.cmmn.CaseExecutionNotFoundException;
	using GetCaseExecutionVariableCmd = org.camunda.bpm.engine.impl.cmmn.cmd.GetCaseExecutionVariableCmd;
	using GetCaseExecutionVariableTypedCmd = org.camunda.bpm.engine.impl.cmmn.cmd.GetCaseExecutionVariableTypedCmd;
	using GetCaseExecutionVariablesCmd = org.camunda.bpm.engine.impl.cmmn.cmd.GetCaseExecutionVariablesCmd;
	using CaseExecutionQueryImpl = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionQueryImpl;
	using CaseInstanceQueryImpl = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseInstanceQueryImpl;
	using CaseExecutionCommandBuilder = org.camunda.bpm.engine.runtime.CaseExecutionCommandBuilder;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceBuilder = org.camunda.bpm.engine.runtime.CaseInstanceBuilder;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseServiceImpl : ServiceImpl, CaseService
	{

	  public virtual CaseInstanceBuilder withCaseDefinitionByKey(string caseDefinitionKey)
	  {
		return new CaseInstanceBuilderImpl(commandExecutor, caseDefinitionKey, null);
	  }

	  public virtual CaseInstanceBuilder withCaseDefinition(string caseDefinitionId)
	  {
		return new CaseInstanceBuilderImpl(commandExecutor, null, caseDefinitionId);
	  }

	  public virtual CaseInstanceQuery createCaseInstanceQuery()
	  {
		return new CaseInstanceQueryImpl(commandExecutor);
	  }

	  public virtual CaseExecutionQuery createCaseExecutionQuery()
	  {
		return new CaseExecutionQueryImpl(commandExecutor);
	  }

	  public virtual CaseExecutionCommandBuilder withCaseExecution(string caseExecutionId)
	  {
		return new CaseExecutionCommandBuilderImpl(commandExecutor, caseExecutionId);
	  }

	  public virtual VariableMap getVariables(string caseExecutionId)
	  {
		return getVariablesTyped(caseExecutionId);
	  }

	  public virtual VariableMap getVariablesTyped(string caseExecutionId)
	  {
		return getVariablesTyped(caseExecutionId, true);
	  }

	  public virtual VariableMap getVariablesTyped(string caseExecutionId, bool deserializeValues)
	  {
		return getCaseExecutionVariables(caseExecutionId, null, false, deserializeValues);
	  }

	  public virtual VariableMap getVariablesLocal(string caseExecutionId)
	  {
		return getVariablesLocalTyped(caseExecutionId);
	  }

	  public virtual VariableMap getVariablesLocalTyped(string caseExecutionId)
	  {
		return getVariablesLocalTyped(caseExecutionId, true);
	  }

	  public virtual VariableMap getVariablesLocalTyped(string caseExecutionId, bool deserializeValues)
	  {
		return getCaseExecutionVariables(caseExecutionId, null, true, deserializeValues);
	  }

	  public virtual VariableMap getVariables(string caseExecutionId, ICollection<string> variableNames)
	  {
		return getVariablesTyped(caseExecutionId, variableNames, true);
	  }

	  public virtual VariableMap getVariablesTyped(string caseExecutionId, ICollection<string> variableNames, bool deserializeValues)
	  {
		return getCaseExecutionVariables(caseExecutionId, variableNames, false, deserializeValues);
	  }

	  public virtual VariableMap getVariablesLocal(string caseExecutionId, ICollection<string> variableNames)
	  {
		return getVariablesLocalTyped(caseExecutionId, variableNames, true);
	  }

	  public virtual VariableMap getVariablesLocalTyped(string caseExecutionId, ICollection<string> variableNames, bool deserializeValues)
	  {
		return getCaseExecutionVariables(caseExecutionId, variableNames, true, deserializeValues);
	  }

	  protected internal virtual VariableMap getCaseExecutionVariables(string caseExecutionId, ICollection<string> variableNames, bool isLocal, bool deserializeValues)
	  {
		try
		{
		  return commandExecutor.execute(new GetCaseExecutionVariablesCmd(caseExecutionId, variableNames, isLocal, deserializeValues));
		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);
		}
		catch (CaseExecutionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
	  }

	  public virtual object getVariable(string caseExecutionId, string variableName)
	  {
		return getCaseExecutionVariable(caseExecutionId, variableName, false);
	  }

	  public virtual object getVariableLocal(string caseExecutionId, string variableName)
	  {
		return getCaseExecutionVariable(caseExecutionId, variableName, true);
	  }

	  protected internal virtual object getCaseExecutionVariable(string caseExecutionId, string variableName, bool isLocal)
	  {
		try
		{
		  return commandExecutor.execute(new GetCaseExecutionVariableCmd(caseExecutionId, variableName, isLocal));
		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);
		}
		catch (CaseExecutionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
	  }

	  public virtual T getVariableTyped<T>(string caseExecutionId, string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableTyped(caseExecutionId, variableName, true);
	  }

	  public virtual T getVariableTyped<T>(string caseExecutionId, string variableName, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getCaseExecutionVariableTyped(caseExecutionId, variableName, false, deserializeValue);
	  }

	  public virtual T getVariableLocalTyped<T>(string caseExecutionId, string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableLocalTyped(caseExecutionId, variableName, true);
	  }

	  public virtual T getVariableLocalTyped<T>(string caseExecutionId, string variableName, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getCaseExecutionVariableTyped(caseExecutionId, variableName, true, deserializeValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected <T extends org.camunda.bpm.engine.variable.value.TypedValue> T getCaseExecutionVariableTyped(String caseExecutionId, String variableName, boolean isLocal, boolean deserializeValue)
	  protected internal virtual T getCaseExecutionVariableTyped<T>(string caseExecutionId, string variableName, bool isLocal, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		try
		{
		  return (T) commandExecutor.execute(new GetCaseExecutionVariableTypedCmd(caseExecutionId, variableName, isLocal, deserializeValue));
		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);
		}
		catch (CaseExecutionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
	  }

	  public virtual void setVariables(string caseExecutionId, IDictionary<string, object> variables)
	  {
		withCaseExecution(caseExecutionId).setVariables(variables).execute();
	  }

	  public virtual void setVariablesLocal(string caseExecutionId, IDictionary<string, object> variables)
	  {
		withCaseExecution(caseExecutionId).setVariablesLocal(variables).execute();
	  }

	  public virtual void setVariable(string caseExecutionId, string variableName, object value)
	  {
		withCaseExecution(caseExecutionId).setVariable(variableName, value).execute();
	  }

	  public virtual void setVariableLocal(string caseExecutionId, string variableName, object value)
	  {
		withCaseExecution(caseExecutionId).setVariableLocal(variableName, value).execute();
	  }

	  public virtual void removeVariables(string caseExecutionId, ICollection<string> variableNames)
	  {
		withCaseExecution(caseExecutionId).removeVariables(variableNames).execute();
	  }

	  public virtual void removeVariablesLocal(string caseExecutionId, ICollection<string> variableNames)
	  {
		withCaseExecution(caseExecutionId).removeVariablesLocal(variableNames).execute();
	  }

	  public virtual void removeVariable(string caseExecutionId, string variableName)
	  {
		withCaseExecution(caseExecutionId).removeVariable(variableName).execute();
	  }

	  public virtual void removeVariableLocal(string caseExecutionId, string variableName)
	  {
		withCaseExecution(caseExecutionId).removeVariableLocal(variableName).execute();
	  }

	  public virtual CaseInstance createCaseInstanceByKey(string caseDefinitionKey)
	  {
		return withCaseDefinitionByKey(caseDefinitionKey).create();
	  }

	  public virtual CaseInstance createCaseInstanceByKey(string caseDefinitionKey, string businessKey)
	  {
		return withCaseDefinitionByKey(caseDefinitionKey).businessKey(businessKey).create();
	  }

	  public virtual CaseInstance createCaseInstanceByKey(string caseDefinitionKey, IDictionary<string, object> variables)
	  {
		return withCaseDefinitionByKey(caseDefinitionKey).setVariables(variables).create();
	  }

	  public virtual CaseInstance createCaseInstanceByKey(string caseDefinitionKey, string businessKey, IDictionary<string, object> variables)
	  {
		return withCaseDefinitionByKey(caseDefinitionKey).businessKey(businessKey).setVariables(variables).create();
	  }

	  public virtual CaseInstance createCaseInstanceById(string caseDefinitionId)
	  {
		return withCaseDefinition(caseDefinitionId).create();
	  }

	  public virtual CaseInstance createCaseInstanceById(string caseDefinitionId, string businessKey)
	  {
		return withCaseDefinition(caseDefinitionId).businessKey(businessKey).create();
	  }

	  public virtual CaseInstance createCaseInstanceById(string caseDefinitionId, IDictionary<string, object> variables)
	  {
		return withCaseDefinition(caseDefinitionId).setVariables(variables).create();
	  }

	  public virtual CaseInstance createCaseInstanceById(string caseDefinitionId, string businessKey, IDictionary<string, object> variables)
	  {
		return withCaseDefinition(caseDefinitionId).businessKey(businessKey).setVariables(variables).create();
	  }

	  public virtual void manuallyStartCaseExecution(string caseExecutionId)
	  {
		withCaseExecution(caseExecutionId).manualStart();
	  }

	  public virtual void manuallyStartCaseExecution(string caseExecutionId, IDictionary<string, object> variables)
	  {
		withCaseExecution(caseExecutionId).setVariables(variables).manualStart();
	  }

	  public virtual void disableCaseExecution(string caseExecutionId)
	  {
		withCaseExecution(caseExecutionId).disable();
	  }

	  public virtual void disableCaseExecution(string caseExecutionId, IDictionary<string, object> variables)
	  {
		withCaseExecution(caseExecutionId).setVariables(variables).disable();
	  }

	  public virtual void reenableCaseExecution(string caseExecutionId)
	  {
		withCaseExecution(caseExecutionId).reenable();
	  }

	  public virtual void reenableCaseExecution(string caseExecutionId, IDictionary<string, object> variables)
	  {
		withCaseExecution(caseExecutionId).setVariables(variables).reenable();
	  }

	  public virtual void completeCaseExecution(string caseExecutionId)
	  {
		withCaseExecution(caseExecutionId).complete();
	  }

	  public virtual void completeCaseExecution(string caseExecutionId, IDictionary<string, object> variables)
	  {
		withCaseExecution(caseExecutionId).setVariables(variables).complete();
	  }

	  public virtual void closeCaseInstance(string caseInstanceId)
	  {
		withCaseExecution(caseInstanceId).close();
	  }

	  public virtual void terminateCaseExecution(string caseExecutionId)
	  {
		withCaseExecution(caseExecutionId).terminate();
	  }

	  public virtual void terminateCaseExecution(string caseExecutionId, IDictionary<string, object> variables)
	  {
		withCaseExecution(caseExecutionId).setVariables(variables).terminate();
	  }
	}
}