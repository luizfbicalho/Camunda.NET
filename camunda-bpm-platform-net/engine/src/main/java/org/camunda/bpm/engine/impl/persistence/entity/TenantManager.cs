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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using TenantCheck = org.camunda.bpm.engine.impl.db.TenantCheck;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;

	/// <summary>
	/// @author Kristin Polenz
	/// 
	/// </summary>
	public class TenantManager : AbstractManager
	{

	  public virtual ListQueryParameterObject configureQuery(ListQueryParameterObject query)
	  {
		TenantCheck tenantCheck = query.TenantCheck;

		configureTenantCheck(tenantCheck);

		return query;
	  }

	  public virtual void configureTenantCheck(TenantCheck tenantCheck)
	  {
		if (TenantCheckEnabled)
		{
		  Authentication currentAuthentication = CurrentAuthentication;

		  tenantCheck.TenantCheckEnabled = true;
		  tenantCheck.AuthTenantIds = currentAuthentication.TenantIds;

		}
		else
		{
		  tenantCheck.TenantCheckEnabled = false;
		  tenantCheck.AuthTenantIds = null;
		}
	  }

	  public virtual ListQueryParameterObject configureQuery(object parameters)
	  {
		ListQueryParameterObject queryObject = new ListQueryParameterObject();
		queryObject.Parameter = parameters;

		return configureQuery(queryObject);
	  }

	  public virtual bool isAuthenticatedTenant(string tenantId)
	  {
		if (!string.ReferenceEquals(tenantId, null) && TenantCheckEnabled)
		{

		  Authentication currentAuthentication = CurrentAuthentication;
		  IList<string> authenticatedTenantIds = currentAuthentication.TenantIds;
		  if (authenticatedTenantIds != null)
		  {
			return authenticatedTenantIds.Contains(tenantId);

		  }
		  else
		  {
			return false;
		  }
		}
		else
		{
		  return true;
		}
	  }

	  public virtual bool TenantCheckEnabled
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.TenantCheckEnabled && Context.CommandContext.TenantCheckEnabled && CurrentAuthentication != null && !AuthorizationManager.isCamundaAdmin(CurrentAuthentication);
		  }
	  }

	}

}