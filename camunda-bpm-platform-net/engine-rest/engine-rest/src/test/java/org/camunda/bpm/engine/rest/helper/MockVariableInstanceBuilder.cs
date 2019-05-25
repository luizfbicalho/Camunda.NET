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

	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Creates variable mocks and simultaneously provides a ResponseSpecification
	/// to assert a result of this mock in json; expectations can be overriden using the
	/// methods <seealso cref="JsonSpec.setMatcher(string, org.hamcrest.Matcher)"/> and
	/// <seealso cref="JsonSpec.setEnclosedJsonSpec(string, JsonSpec)"/>.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class MockVariableInstanceBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string name_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal TypedValue typedValue_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string errorMessage_Conflict;

	  public virtual MockVariableInstanceBuilder id(string id)
	  {
		this.id_Conflict = id;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder name(string name)
	  {
		this.name_Conflict = name;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder typedValue(TypedValue value)
	  {
		this.typedValue_Conflict = value;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder executionId(string executionId)
	  {
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder caseExecutionId(string caseExecutionId)
	  {
		this.caseExecutionId_Conflict = caseExecutionId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder taskId(string taskId)
	  {
		this.taskId_Conflict = taskId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Conflict = activityInstanceId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder tenantId(string tenantId)
	  {
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder errorMessage(string errorMessage)
	  {
		this.errorMessage_Conflict = errorMessage;
		return this;
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

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Conflict;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Conflict;
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

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Conflict;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId_Conflict;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId_Conflict;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage_Conflict;
		  }
	  }

	  public virtual string TypeName
	  {
		  get
		  {
			return typedValue_Conflict.Type.Name;
		  }
	  }

	  public virtual VariableInstance build()
	  {
		VariableInstance mockVariable = mock(typeof(VariableInstance));
		return build(mockVariable);
	  }

	  public virtual VariableInstanceEntity buildEntity()
	  {
		VariableInstanceEntity mockVariable = mock(typeof(VariableInstanceEntity));
		if (!string.ReferenceEquals(taskId_Conflict, null))
		{
		  when(mockVariable.VariableScopeId).thenReturn(taskId_Conflict);
		}
		else if (!string.ReferenceEquals(executionId_Conflict, null))
		{
		  when(mockVariable.VariableScopeId).thenReturn(executionId_Conflict);
		}
		else
		{
		  when(mockVariable.VariableScopeId).thenReturn(caseExecutionId_Conflict);
		}
		return build(mockVariable);
	  }

	  protected internal virtual T build<T>(T mockVariable) where T : org.camunda.bpm.engine.runtime.VariableInstance
	  {
		when(mockVariable.Id).thenReturn(id_Conflict);
		when(mockVariable.Name).thenReturn(name_Conflict);
		when(mockVariable.TypeName).thenReturn(typedValue_Conflict.Type.Name);

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
		when(mockVariable.ProcessInstanceId).thenReturn(processInstanceId_Conflict);
		when(mockVariable.ExecutionId).thenReturn(executionId_Conflict);
		when(mockVariable.CaseInstanceId).thenReturn(caseInstanceId_Conflict);
		when(mockVariable.CaseExecutionId).thenReturn(caseExecutionId_Conflict);
		when(mockVariable.TaskId).thenReturn(taskId_Conflict);
		when(mockVariable.ActivityInstanceId).thenReturn(activityInstanceId_Conflict);
		when(mockVariable.TenantId).thenReturn(tenantId_Conflict);
		when(mockVariable.ErrorMessage).thenReturn(errorMessage_Conflict);

		return mockVariable;
	  }

	}

}