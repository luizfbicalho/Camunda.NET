using System;
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
namespace org.camunda.bpm.identity.impl.ldap
{

	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using Page = org.camunda.bpm.engine.impl.Page;
	using TenantQueryImpl = org.camunda.bpm.engine.impl.TenantQueryImpl;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// Since multi-tenancy is not supported for the LDAP plugin, the query always
	/// returns <code>0</code> or an empty list.
	/// </summary>
	[Serializable]
	public class LdapTenantQuery : TenantQueryImpl
	{

	  private const long serialVersionUID = 1L;

	  public LdapTenantQuery() : base()
	  {
	  }

	  public LdapTenantQuery(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		return 0;
	  }

	  public override IList<Tenant> executeList(CommandContext commandContext, Page page)
	  {
		return Collections.emptyList();
	  }

	}

}