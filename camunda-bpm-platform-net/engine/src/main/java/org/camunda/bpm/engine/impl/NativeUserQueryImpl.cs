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
namespace org.camunda.bpm.engine.impl
{
	using NativeUserQuery = org.camunda.bpm.engine.identity.NativeUserQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using DbReadOnlyIdentityServiceProvider = org.camunda.bpm.engine.impl.identity.db.DbReadOnlyIdentityServiceProvider;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	///  @author Svetlana Dorokhova
	/// </summary>
	[Serializable]
	public class NativeUserQueryImpl : AbstractNativeQuery<NativeUserQuery, User>, NativeUserQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeUserQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeUserQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }


	 //results ////////////////////////////////////////////////////////////////

	  public override IList<User> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.identity.db.DbReadOnlyIdentityServiceProvider identityProvider = getIdentityProvider(commandContext);
		DbReadOnlyIdentityServiceProvider identityProvider = getIdentityProvider(commandContext);
		return identityProvider.findUserByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public override long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.identity.db.DbReadOnlyIdentityServiceProvider identityProvider = getIdentityProvider(commandContext);
		DbReadOnlyIdentityServiceProvider identityProvider = getIdentityProvider(commandContext);
		return identityProvider.findUserCountByNativeQuery(parameterMap);
	  }

	  private DbReadOnlyIdentityServiceProvider getIdentityProvider(CommandContext commandContext)
	  {
		return (DbReadOnlyIdentityServiceProvider) commandContext.ReadOnlyIdentityProvider;
	  }

	}

}