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

	using User = org.camunda.bpm.engine.identity.User;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class DbUserQueryImpl : UserQueryImpl
	{

	  private const long serialVersionUID = 1L;

	  public DbUserQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public DbUserQueryImpl() : base()
	  {
	  }

	  // results //////////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DbReadOnlyIdentityServiceProvider identityProvider = getIdentityProvider(commandContext);
		DbReadOnlyIdentityServiceProvider identityProvider = getIdentityProvider(commandContext);
		return identityProvider.findUserCountByQueryCriteria(this);
	  }

	  public override IList<User> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DbReadOnlyIdentityServiceProvider identityProvider = getIdentityProvider(commandContext);
		DbReadOnlyIdentityServiceProvider identityProvider = getIdentityProvider(commandContext);
		return identityProvider.findUserByQueryCriteria(this);
	  }

	  private DbReadOnlyIdentityServiceProvider getIdentityProvider(CommandContext commandContext)
	  {
		return (DbReadOnlyIdentityServiceProvider) commandContext.ReadOnlyIdentityProvider;
	  }


	}

}