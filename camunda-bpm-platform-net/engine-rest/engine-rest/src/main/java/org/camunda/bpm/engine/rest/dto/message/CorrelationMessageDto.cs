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
namespace org.camunda.bpm.engine.rest.dto.message
{

	public class CorrelationMessageDto
	{

	  private string messageName;
	  private string businessKey;
	  private IDictionary<string, VariableValueDto> correlationKeys;
	  private IDictionary<string, VariableValueDto> localCorrelationKeys;
	  private IDictionary<string, VariableValueDto> processVariables;
	  private IDictionary<string, VariableValueDto> processVariablesLocal;
	  private string tenantId;
	  private bool withoutTenantId;
	  private string processInstanceId;

	  private bool all = false;
	  private bool resultEnabled = false;
	  private bool variablesInResultEnabled = false;

	  public virtual string MessageName
	  {
		  get
		  {
			return messageName;
		  }
		  set
		  {
			this.messageName = value;
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


	  public virtual IDictionary<string, VariableValueDto> CorrelationKeys
	  {
		  get
		  {
			return correlationKeys;
		  }
		  set
		  {
			this.correlationKeys = value;
		  }
	  }


	  public virtual IDictionary<string, VariableValueDto> LocalCorrelationKeys
	  {
		  get
		  {
			return localCorrelationKeys;
		  }
		  set
		  {
			this.localCorrelationKeys = value;
		  }
	  }


	  public virtual IDictionary<string, VariableValueDto> ProcessVariables
	  {
		  get
		  {
			return processVariables;
		  }
		  set
		  {
			this.processVariables = value;
		  }
	  }


	  public virtual IDictionary<string, VariableValueDto> ProcessVariablesLocal
	  {
		  get
		  {
			return processVariablesLocal;
		  }
		  set
		  {
			this.processVariablesLocal = value;
		  }
	  }


	  public virtual bool All
	  {
		  get
		  {
			return all;
		  }
		  set
		  {
			this.all = value;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual bool WithoutTenantId
	  {
		  get
		  {
			return withoutTenantId;
		  }
		  set
		  {
			this.withoutTenantId = value;
		  }
	  }


	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }


	  public virtual bool ResultEnabled
	  {
		  get
		  {
			return resultEnabled;
		  }
		  set
		  {
			this.resultEnabled = value;
		  }
	  }


	  public virtual bool VariablesInResultEnabled
	  {
		  get
		  {
			return variablesInResultEnabled;
		  }
		  set
		  {
			this.variablesInResultEnabled = value;
		  }
	  }

	}

}