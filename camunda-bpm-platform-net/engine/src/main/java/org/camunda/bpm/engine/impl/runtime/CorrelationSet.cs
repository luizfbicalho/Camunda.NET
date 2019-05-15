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
namespace org.camunda.bpm.engine.impl.runtime
{

	public class CorrelationSet
	{

	  protected internal readonly string businessKey;
	  protected internal readonly IDictionary<string, object> correlationKeys;
	  protected internal readonly IDictionary<string, object> localCorrelationKeys;
	  protected internal readonly string processInstanceId;
	  protected internal readonly string processDefinitionId;
	  protected internal readonly string tenantId;
	  protected internal readonly bool isTenantIdSet;

	  public CorrelationSet(MessageCorrelationBuilderImpl builder)
	  {
		this.businessKey = builder.BusinessKey;
		this.processInstanceId = builder.ProcessInstanceId;
		this.correlationKeys = builder.CorrelationProcessInstanceVariables;
		this.localCorrelationKeys = builder.CorrelationLocalVariables;
		this.processDefinitionId = builder.ProcessDefinitionId;
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

	  public virtual IDictionary<string, object> CorrelationKeys
	  {
		  get
		  {
			return correlationKeys;
		  }
	  }

	  public virtual IDictionary<string, object> LocalCorrelationKeys
	  {
		  get
		  {
			return localCorrelationKeys;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
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
		return "CorrelationSet [businessKey=" + businessKey + ", processInstanceId=" + processInstanceId + ", processDefinitionId=" + processDefinitionId + ", correlationKeys=" + correlationKeys + ", localCorrelationKeys=" + localCorrelationKeys + ", tenantId=" + tenantId + ", isTenantIdSet=" + isTenantIdSet + "]";
	  }

	}

}