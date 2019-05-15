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
namespace org.camunda.bpm.engine.impl.identity.db
{

	using Group = org.camunda.bpm.engine.identity.Group;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class DbGroupQueryImpl : GroupQueryImpl
	{

	  private const long serialVersionUID = 1L;

	  public DbGroupQueryImpl() : base()
	  {
	  }

	  public DbGroupQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  //results ////////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		DbReadOnlyIdentityServiceProvider identityProvider = getIdentityProvider(commandContext);
		return identityProvider.findGroupCountByQueryCriteria(this);
	  }

	  public override IList<Group> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		DbReadOnlyIdentityServiceProvider identityProvider = getIdentityProvider(commandContext);
		return identityProvider.findGroupByQueryCriteria(this);
	  }

	  protected internal virtual DbReadOnlyIdentityServiceProvider getIdentityProvider(CommandContext commandContext)
	  {
		return (DbReadOnlyIdentityServiceProvider) commandContext.ReadOnlyIdentityProvider;
	  }

	}

}