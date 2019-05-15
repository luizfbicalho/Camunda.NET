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
namespace org.camunda.bpm.engine.impl.runtime
{
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	public class ConditionSet
	{

	  protected internal readonly string businessKey;
	  protected internal readonly string processDefinitionId;
	  protected internal readonly VariableMap variables;
	  protected internal readonly string tenantId;
	  protected internal readonly bool isTenantIdSet;

	  public ConditionSet(ConditionEvaluationBuilderImpl builder)
	  {
		this.businessKey = builder.BusinessKey;
		this.processDefinitionId = builder.ProcessDefinitionId;
		this.variables = builder.getVariables();
		this.tenantId = builder.TenantId;
		this.isTenantIdSet = builder.TenantIdSet;
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual VariableMap Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual bool TenantIdSet
	  {
		  get
		  {
			return isTenantIdSet;
		  }
	  }

	  public override string ToString()
	  {
		return "ConditionSet [businessKey=" + businessKey + ", processDefinitionId=" + processDefinitionId + ", variables=" + variables + ", tenantId=" + tenantId + ", isTenantIdSet=" + isTenantIdSet + "]";
	  }

	}

}