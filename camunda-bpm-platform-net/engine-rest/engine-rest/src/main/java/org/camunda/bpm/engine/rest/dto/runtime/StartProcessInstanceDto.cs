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

	using ProcessInstanceModificationInstructionDto = org.camunda.bpm.engine.rest.dto.runtime.modification.ProcessInstanceModificationInstructionDto;

	public class StartProcessInstanceDto
	{

	  protected internal IDictionary<string, VariableValueDto> variables;
	  protected internal string businessKey;
	  protected internal string caseInstanceId;
	  protected internal IList<ProcessInstanceModificationInstructionDto> startInstructions;
	  protected internal bool skipCustomListeners;
	  protected internal bool skipIoMappings;
	  protected internal bool withVariablesInReturn = false;

	  public virtual IDictionary<string, VariableValueDto> Variables
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


	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
		  set
		  {
			this.businessKey = value;
		  }
	  }


	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }


	  public virtual IList<ProcessInstanceModificationInstructionDto> StartInstructions
	  {
		  get
		  {
			return startInstructions;
		  }
		  set
		  {
			this.startInstructions = value;
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


	  public virtual bool WithVariablesInReturn
	  {
		  get
		  {
			return withVariablesInReturn;
		  }
		  set
		  {
			this.withVariablesInReturn = value;
		  }
	  }

	}

}