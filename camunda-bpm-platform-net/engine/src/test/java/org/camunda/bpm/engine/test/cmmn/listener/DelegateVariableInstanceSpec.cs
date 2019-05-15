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
namespace org.camunda.bpm.engine.test.cmmn.listener
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using DelegateCaseVariableInstance = org.camunda.bpm.engine.@delegate.DelegateCaseVariableInstance;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class DelegateVariableInstanceSpec
	{

	  protected internal string expectedEventName;
	  protected internal string expectedVariableName;
	  protected internal string expectedVariableValue;

	  protected internal string expectedProcessInstanceId;
	  protected internal string expectedExecutionId;
	  protected internal string expectedCaseInstanceId;
	  protected internal string expectedCaseExecutionId;
	  protected internal string expectedTaskId;
	  protected internal string expectedActivityInstanceId;

	  protected internal CaseExecution expectedSourceExecution;

	  public virtual void matches(DelegateCaseVariableInstance instance)
	  {
		assertEquals(expectedEventName, instance.EventName);
		assertEquals(expectedVariableName, instance.Name);
		assertEquals(expectedVariableValue, instance.Value);
		assertEquals(expectedProcessInstanceId, instance.ProcessInstanceId);
		assertEquals(expectedExecutionId, instance.ExecutionId);
		assertEquals(expectedCaseInstanceId, instance.CaseInstanceId);
		assertEquals(expectedCaseExecutionId, instance.CaseExecutionId);
		assertEquals(expectedTaskId, instance.TaskId);
		assertEquals(expectedActivityInstanceId, instance.ActivityInstanceId);

		assertEquals(expectedSourceExecution.Id, instance.SourceExecution.Id);
		assertEquals(expectedSourceExecution.ActivityId, instance.SourceExecution.ActivityId);
		assertEquals(expectedSourceExecution.ActivityName, instance.SourceExecution.ActivityName);
		assertEquals(expectedSourceExecution.CaseDefinitionId, instance.SourceExecution.CaseDefinitionId);
		assertEquals(expectedSourceExecution.CaseInstanceId, instance.SourceExecution.CaseInstanceId);
		assertEquals(expectedSourceExecution.ParentId, instance.SourceExecution.ParentId);
	  }

	  public static DelegateVariableInstanceSpec fromCaseExecution(CaseExecution caseExecution)
	  {
		DelegateVariableInstanceSpec spec = new DelegateVariableInstanceSpec();
		spec.expectedCaseExecutionId = caseExecution.Id;
		spec.expectedCaseInstanceId = caseExecution.CaseInstanceId;
		spec.expectedSourceExecution = caseExecution;
		return spec;
	  }

	  public virtual DelegateVariableInstanceSpec sourceExecution(CaseExecution sourceExecution)
	  {
		this.expectedSourceExecution = sourceExecution;
		return this;
	  }

	  public virtual DelegateVariableInstanceSpec @event(string eventName)
	  {
		this.expectedEventName = eventName;
		return this;
	  }

	  public virtual DelegateVariableInstanceSpec name(string variableName)
	  {
		this.expectedVariableName = variableName;
		return this;
	  }

	  public virtual DelegateVariableInstanceSpec value(string variableValue)
	  {
		this.expectedVariableValue = variableValue;
		return this;
	  }

	  public virtual DelegateVariableInstanceSpec activityInstanceId(string activityInstanceId)
	  {
		this.expectedActivityInstanceId = activityInstanceId;
		return this;
	  }
	}

}