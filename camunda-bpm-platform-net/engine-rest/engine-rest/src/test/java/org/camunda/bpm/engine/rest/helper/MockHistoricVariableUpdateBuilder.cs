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
	  protected internal string id_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string name_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal TypedValue typedValue_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string errorMessage_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int revision_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string time_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string userOperationId_Conflict;

	  public virtual MockHistoricVariableUpdateBuilder id(string id)
	  {
		this.id_Conflict = id;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder name(string name)
	  {
		this.name_Conflict = name;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder variableInstanceId(string variableInstanceId)
	  {
		this.variableInstanceId_Conflict = variableInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder typedValue(TypedValue value)
	  {
		this.typedValue_Conflict = value;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder errorMessage(string errorMessage)
	  {
		this.errorMessage_Conflict = errorMessage;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Conflict = activityInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder executionId(string executionId)
	  {
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder taskId(string taskId)
	  {
		this.taskId_Conflict = taskId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder time(string time)
	  {
		this.time_Conflict = time;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder revision(int revision)
	  {
		this.revision_Conflict = revision;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder caseDefinitionKey(string caseDefinitionKey)
	  {
		this.caseDefinitionKey_Conflict = caseDefinitionKey;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder caseDefinitionId(string caseDefinitionId)
	  {
		this.caseDefinitionId_Conflict = caseDefinitionId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder caseExecutionId(string caseExecutionId)
	  {
		this.caseExecutionId_Conflict = caseExecutionId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder tenantId(string tenantId)
	  {
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual MockHistoricVariableUpdateBuilder userOperationId(string userOperationId)
	  {
		this.userOperationId_Conflict = userOperationId;
		return this;
	  }

	  public virtual HistoricVariableUpdate build()
	  {
		HistoricVariableUpdate mockVariable = mock(typeof(HistoricVariableUpdate));
		when(mockVariable.Id).thenReturn(id_Conflict);
		when(mockVariable.VariableName).thenReturn(name_Conflict);
		when(mockVariable.VariableInstanceId).thenReturn(variableInstanceId_Conflict);
		when(mockVariable.VariableTypeName).thenReturn(typedValue_Conflict.Type.Name);

		if (typedValue_Conflict.GetType().IsAssignableFrom(typeof(ObjectValue)))
		{
		  ObjectValue objectValue = (ObjectValue) typedValue_Conflict;
		  if (objectValue.Deserialized)
		  {
			when(mockVariable.Value).thenReturn(typedValue_Conflict.Value);
		  }
		  else
		  {
			when(mockVariable.Value).thenReturn(null);
		  }
		}
		else
		{
		  when(mockVariable.Value).thenReturn(typedValue_Conflict.Value);
		}

		when(mockVariable.TypedValue).thenReturn(typedValue_Conflict);
		when(mockVariable.ProcessDefinitionKey).thenReturn(processDefinitionKey_Conflict);
		when(mockVariable.ProcessDefinitionId).thenReturn(processDefinitionId_Conflict);
		when(mockVariable.ProcessInstanceId).thenReturn(processInstanceId_Conflict);
		when(mockVariable.ErrorMessage).thenReturn(errorMessage_Conflict);
		when(mockVariable.Revision).thenReturn(revision_Conflict);
		when(mockVariable.Time).thenReturn(DateTimeUtil.parseDate(time_Conflict));
		when(mockVariable.ActivityInstanceId).thenReturn(activityInstanceId_Conflict);
		when(mockVariable.TaskId).thenReturn(taskId_Conflict);
		when(mockVariable.ExecutionId).thenReturn(executionId_Conflict);
		when(mockVariable.TypeName).thenReturn(typedValue_Conflict.Type.Name);
		when(mockVariable.CaseDefinitionKey).thenReturn(caseDefinitionKey_Conflict);
		when(mockVariable.CaseDefinitionId).thenReturn(caseDefinitionId_Conflict);
		when(mockVariable.CaseInstanceId).thenReturn(caseInstanceId_Conflict);
		when(mockVariable.CaseExecutionId).thenReturn(caseExecutionId_Conflict);
		when(mockVariable.TenantId).thenReturn(tenantId_Conflict);
		when(mockVariable.UserOperationId).thenReturn(userOperationId_Conflict);

		return mockVariable;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id_Conflict;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name_Conflict;
		  }
	  }

	  public virtual string VariableInstanceId
	  {
		  get
		  {
			return variableInstanceId_Conflict;
		  }
	  }

	  public virtual object Value
	  {
		  get
		  {
			return typedValue_Conflict.Value;
		  }
	  }

	  public virtual TypedValue TypedValue
	  {
		  get
		  {
			return typedValue_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Conflict;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Conflict;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage_Conflict;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId_Conflict;
		  }
	  }

	  public virtual int Revision
	  {
		  get
		  {
			return revision_Conflict;
		  }
	  }

	  public virtual string Time
	  {
		  get
		  {
			return time_Conflict;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Conflict;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId_Conflict;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Conflict;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId_Conflict;
		  }
	  }

	  public virtual string UserOperationId
	  {
		  get
		  {
			return userOperationId_Conflict;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId_Conflict;
		  }
	  }

	}

}