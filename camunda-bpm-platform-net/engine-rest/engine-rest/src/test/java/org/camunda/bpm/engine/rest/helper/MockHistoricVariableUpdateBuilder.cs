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
namespace org.camunda.bpm.engine.rest.helper
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MockHistoricVariableUpdateBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string name_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal TypedValue typedValue_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string errorMessage_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int revision_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string time_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string userOperationId_Renamed;

	  public virtual MockHistoricVariableUpdateBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder name(string name)
	  {
		this.name_Renamed = name;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder variableInstanceId(string variableInstanceId)
	  {
		this.variableInstanceId_Renamed = variableInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder typedValue(TypedValue value)
	  {
		this.typedValue_Renamed = value;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder errorMessage(string errorMessage)
	  {
		this.errorMessage_Renamed = errorMessage;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Renamed = activityInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder executionId(string executionId)
	  {
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder taskId(string taskId)
	  {
		this.taskId_Renamed = taskId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder time(string time)
	  {
		this.time_Renamed = time;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder revision(int revision)
	  {
		this.revision_Renamed = revision;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder caseDefinitionKey(string caseDefinitionKey)
	  {
		this.caseDefinitionKey_Renamed = caseDefinitionKey;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder caseDefinitionId(string caseDefinitionId)
	  {
		this.caseDefinitionId_Renamed = caseDefinitionId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Renamed = caseInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder caseExecutionId(string caseExecutionId)
	  {
		this.caseExecutionId_Renamed = caseExecutionId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder tenantId(string tenantId)
	  {
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder userOperationId(string userOperationId)
	  {
		this.userOperationId_Renamed = userOperationId;
		return this;
	  }

	  public virtual HistoricVariableUpdate build()
	  {
		HistoricVariableUpdate mockVariable = mock(typeof(HistoricVariableUpdate));
		when(mockVariable.Id).thenReturn(id_Renamed);
		when(mockVariable.VariableName).thenReturn(name_Renamed);
		when(mockVariable.VariableInstanceId).thenReturn(variableInstanceId_Renamed);
		when(mockVariable.VariableTypeName).thenReturn(typedValue_Renamed.Type.Name);

		if (typedValue_Renamed.GetType().IsAssignableFrom(typeof(ObjectValue)))
		{
		  ObjectValue objectValue = (ObjectValue) typedValue_Renamed;
		  if (objectValue.Deserialized)
		  {
			when(mockVariable.Value).thenReturn(typedValue_Renamed.Value);
		  }
		  else
		  {
			when(mockVariable.Value).thenReturn(null);
		  }
		}
		else
		{
		  when(mockVariable.Value).thenReturn(typedValue_Renamed.Value);
		}

		when(mockVariable.TypedValue).thenReturn(typedValue_Renamed);
		when(mockVariable.ProcessDefinitionKey).thenReturn(processDefinitionKey_Renamed);
		when(mockVariable.ProcessDefinitionId).thenReturn(processDefinitionId_Renamed);
		when(mockVariable.ProcessInstanceId).thenReturn(processInstanceId_Renamed);
		when(mockVariable.ErrorMessage).thenReturn(errorMessage_Renamed);
		when(mockVariable.Revision).thenReturn(revision_Renamed);
		when(mockVariable.Time).thenReturn(DateTimeUtil.parseDate(time_Renamed));
		when(mockVariable.ActivityInstanceId).thenReturn(activityInstanceId_Renamed);
		when(mockVariable.TaskId).thenReturn(taskId_Renamed);
		when(mockVariable.ExecutionId).thenReturn(executionId_Renamed);
		when(mockVariable.TypeName).thenReturn(typedValue_Renamed.Type.Name);
		when(mockVariable.CaseDefinitionKey).thenReturn(caseDefinitionKey_Renamed);
		when(mockVariable.CaseDefinitionId).thenReturn(caseDefinitionId_Renamed);
		when(mockVariable.CaseInstanceId).thenReturn(caseInstanceId_Renamed);
		when(mockVariable.CaseExecutionId).thenReturn(caseExecutionId_Renamed);
		when(mockVariable.TenantId).thenReturn(tenantId_Renamed);
		when(mockVariable.UserOperationId).thenReturn(userOperationId_Renamed);

		return mockVariable;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id_Renamed;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name_Renamed;
		  }
	  }

	  public virtual string VariableInstanceId
	  {
		  get
		  {
			return variableInstanceId_Renamed;
		  }
	  }

	  public virtual object Value
	  {
		  get
		  {
			return typedValue_Renamed.Value;
		  }
	  }

	  public virtual TypedValue TypedValue
	  {
		  get
		  {
			return typedValue_Renamed;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Renamed;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Renamed;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Renamed;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage_Renamed;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId_Renamed;
		  }
	  }

	  public virtual int Revision
	  {
		  get
		  {
			return revision_Renamed;
		  }
	  }

	  public virtual string Time
	  {
		  get
		  {
			return time_Renamed;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Renamed;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Renamed;
		  }
	  }

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey_Renamed;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId_Renamed;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Renamed;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId_Renamed;
		  }
	  }

	  public virtual string UserOperationId
	  {
		  get
		  {
			return userOperationId_Renamed;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId_Renamed;
		  }
	  }

	}

}