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
namespace org.camunda.bpm.engine.test.api.multitenancy
{
	using TenantIdProvider = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProvider;
	using TenantIdProviderCaseInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderCaseInstanceContext;
	using TenantIdProviderHistoricDecisionInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderHistoricDecisionInstanceContext;
	using TenantIdProviderProcessInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderProcessInstanceContext;

	/// <summary>
	/// Provide the given tenant id for all instances.
	/// </summary>
	public class StaticTenantIdTestProvider : TenantIdProvider
	{

	  protected internal string tenantId;

	  public StaticTenantIdTestProvider(string tenantId)
	  {
		this.tenantId = tenantId;
	  }

	  public virtual string TenantIdProvider
	  {
		  set
		  {
			this.tenantId = value;
		  }
	  }

	  public virtual string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
	  {
		return tenantId;
	  }

	  public virtual string provideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
	  {
		return tenantId;
	  }

	  public virtual string provideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
	  {
		return tenantId;
	  }
	}
}