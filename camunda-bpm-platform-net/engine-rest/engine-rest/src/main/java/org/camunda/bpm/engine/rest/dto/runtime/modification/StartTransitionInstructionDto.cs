﻿/*
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

	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using InstantiationBuilder = org.camunda.bpm.engine.runtime.InstantiationBuilder;
	using ModificationBuilder = org.camunda.bpm.engine.runtime.ModificationBuilder;
	using ProcessInstanceModificationBuilder = org.camunda.bpm.engine.runtime.ProcessInstanceModificationBuilder;
	using ProcessInstanceModificationInstantiationBuilder = org.camunda.bpm.engine.runtime.ProcessInstanceModificationInstantiationBuilder;
	using ProcessInstantiationBuilder = org.camunda.bpm.engine.runtime.ProcessInstantiationBuilder;

	using JsonTypeName = com.fasterxml.jackson.annotation.JsonTypeName;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonTypeName(ProcessInstanceModificationInstructionDto.START_TRANSITION_INSTRUCTION_TYPE) public class StartTransitionInstructionDto extends ProcessInstanceModificationInstructionDto
	public class StartTransitionInstructionDto : ProcessInstanceModificationInstructionDto
	{
		public override void applyTo(ProcessInstanceModificationBuilder builder, ProcessEngine engine, ObjectMapper mapper)
		{
		checkValidity();

		ProcessInstanceModificationInstantiationBuilder activityBuilder = null;

		if (!string.ReferenceEquals(ancestorActivityInstanceId, null))
		{
		  activityBuilder = builder.startTransition(transitionId, ancestorActivityInstanceId);
		}
		else
		{
		  activityBuilder = builder.startTransition(transitionId);
		}

		applyVariables(activityBuilder, engine, mapper);
		}

	  public override void applyTo<T1>(InstantiationBuilder<T1> builder, ProcessEngine engine, ObjectMapper mapper)
	  {
		checkValidity();

		builder.startTransition(transitionId);
		if (builder is ProcessInstantiationBuilder)
		{
		  applyVariables((ProcessInstantiationBuilder) builder, engine, mapper);
		}
	  }

	  protected internal virtual void checkValidity()
	  {
		if (string.ReferenceEquals(transitionId, null))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, buildErrorMessage("'transitionId' must be set"));
		}
	  }
	}

}