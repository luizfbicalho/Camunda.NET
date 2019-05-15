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
namespace org.camunda.bpm.engine.rest.dto.runtime.modification
{

	using ActivityInstantiationBuilder = org.camunda.bpm.engine.runtime.ActivityInstantiationBuilder;
	using InstantiationBuilder = org.camunda.bpm.engine.runtime.InstantiationBuilder;
	using ProcessInstanceModificationBuilder = org.camunda.bpm.engine.runtime.ProcessInstanceModificationBuilder;

	using JsonSubTypes = com.fasterxml.jackson.annotation.JsonSubTypes;
	using JsonTypeInfo = com.fasterxml.jackson.annotation.JsonTypeInfo;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.As.PROPERTY, property = "type", visible = true) @JsonSubTypes(value = { @JsonSubTypes.Type(value = CancellationInstructionDto.class), @JsonSubTypes.Type(value = StartBeforeInstructionDto.class), @JsonSubTypes.Type(value = StartAfterInstructionDto.class), @JsonSubTypes.Type(value = StartTransitionInstructionDto.class)}) public abstract class ProcessInstanceModificationInstructionDto
	public abstract class ProcessInstanceModificationInstructionDto
	{

	  public const string CANCEL_INSTRUCTION_TYPE = "cancel";
	  public const string START_BEFORE_INSTRUCTION_TYPE = "startBeforeActivity";
	  public const string START_TRANSITION_INSTRUCTION_TYPE = "startTransition";
	  public const string START_AFTER_INSTRUCTION_TYPE = "startAfterActivity";

	  protected internal string type;

	  protected internal IDictionary<string, TriggerVariableValueDto> variables;

	  protected internal string activityId;
	  protected internal string transitionId;
	  protected internal string activityInstanceId;
	  protected internal string transitionInstanceId;
	  protected internal string ancestorActivityInstanceId;
	  protected internal bool cancelCurrentActiveActivityInstances;

	  public virtual IDictionary<string, TriggerVariableValueDto> Variables
	  {
		  get
		  {
			return variables;
		  }
		  set
		  {
			this.variables = value;
		  }
	  }
	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }
	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }
	  public virtual string TransitionId
	  {
		  get
		  {
			return transitionId;
		  }
		  set
		  {
			this.transitionId = value;
		  }
	  }
	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
		  set
		  {
			this.activityInstanceId = value;
		  }
	  }
	  public virtual string TransitionInstanceId
	  {
		  get
		  {
			return transitionInstanceId;
		  }
		  set
		  {
			this.transitionInstanceId = value;
		  }
	  }
	  public virtual string AncestorActivityInstanceId
	  {
		  get
		  {
			return ancestorActivityInstanceId;
		  }
		  set
		  {
			this.ancestorActivityInstanceId = value;
		  }
	  }
	  public virtual bool CancelCurrentActiveActivityInstances
	  {
		  get
		  {
			return cancelCurrentActiveActivityInstances;
		  }
		  set
		  {
			this.cancelCurrentActiveActivityInstances = value;
		  }
	  }

	  public abstract void applyTo(ProcessInstanceModificationBuilder builder, ProcessEngine engine, ObjectMapper mapper);

	  public abstract void applyTo<T1>(InstantiationBuilder<T1> builder, ProcessEngine engine, ObjectMapper mapper);

	  protected internal virtual string buildErrorMessage(string message)
	  {
		return "For instruction type '" + type + "': " + message;
	  }

	  protected internal virtual void applyVariables<T1>(ActivityInstantiationBuilder<T1> builder, ProcessEngine engine, ObjectMapper mapper)
	  {

		if (variables != null)
		{
		  foreach (KeyValuePair<string, TriggerVariableValueDto> variableValue in variables.SetOfKeyValuePairs())
		  {
			TriggerVariableValueDto value = variableValue.Value;

			if (value.Local)
			{
			  builder.setVariableLocal(variableValue.Key, value.toTypedValue(engine, mapper));
			}
			else
			{
			  builder.setVariable(variableValue.Key, value.toTypedValue(engine, mapper));

			}
		  }
		}
	  }

	}

}