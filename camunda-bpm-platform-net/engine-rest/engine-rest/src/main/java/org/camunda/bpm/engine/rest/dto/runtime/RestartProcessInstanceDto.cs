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
namespace org.camunda.bpm.engine.rest.dto.runtime
{

	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;
	using ProcessInstanceModificationInstructionDto = org.camunda.bpm.engine.rest.dto.runtime.modification.ProcessInstanceModificationInstructionDto;
	using RestartProcessInstanceBuilder = org.camunda.bpm.engine.runtime.RestartProcessInstanceBuilder;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
	public class RestartProcessInstanceDto
	{

	  protected internal IList<string> processInstanceIds;
	  protected internal IList<ProcessInstanceModificationInstructionDto> instructions;
	  protected internal HistoricProcessInstanceQueryDto historicProcessInstanceQuery;
	  protected internal bool initialVariables;
	  protected internal bool skipCustomListeners;
	  protected internal bool skipIoMappings;
	  protected internal bool withoutBusinessKey;

	  public virtual IList<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds;
		  }
		  set
		  {
			this.processInstanceIds = value;
		  }
	  }


	  public virtual IList<ProcessInstanceModificationInstructionDto> Instructions
	  {
		  get
		  {
			return instructions;
		  }
		  set
		  {
			this.instructions = value;
		  }
	  }


	  public virtual HistoricProcessInstanceQueryDto HistoricProcessInstanceQuery
	  {
		  get
		  {
			return historicProcessInstanceQuery;
		  }
		  set
		  {
			this.historicProcessInstanceQuery = value;
		  }
	  }


	  public virtual bool InitialVariables
	  {
		  get
		  {
			return initialVariables;
		  }
		  set
		  {
			this.initialVariables = value;
		  }
	  }


	  public virtual bool SkipCustomListeners
	  {
		  get
		  {
			return skipCustomListeners;
		  }
		  set
		  {
			this.skipCustomListeners = value;
		  }
	  }


	  public virtual bool SkipIoMappings
	  {
		  get
		  {
			return skipIoMappings;
		  }
		  set
		  {
			this.skipIoMappings = value;
		  }
	  }


	  public virtual bool WithoutBusinessKey
	  {
		  get
		  {
			return withoutBusinessKey;
		  }
		  set
		  {
			this.withoutBusinessKey = value;
		  }
	  }


	  public virtual void applyTo(RestartProcessInstanceBuilder builder, ProcessEngine processEngine, ObjectMapper objectMapper)
	  {
		foreach (ProcessInstanceModificationInstructionDto instruction in instructions)
		{

		  instruction.applyTo(builder, processEngine, objectMapper);
		}
	  }
	}

}