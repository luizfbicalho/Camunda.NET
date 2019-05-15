﻿using System;
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
namespace org.camunda.bpm.engine.impl.cmd
{
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	[Serializable]
	public class ExecuteFilterListPageCmd : AbstractExecuteFilterCmd, Command<IList<JavaToDotNetGenericWildcard>>
	{

	  private const long serialVersionUID = 1L;

	  protected internal int firstResult;
	  protected internal int maxResults;

	  public ExecuteFilterListPageCmd(string filterId, int firstResult, int maxResults) : base(filterId)
	  {
		this.firstResult = firstResult;
		this.maxResults = maxResults;
	  }

	  public ExecuteFilterListPageCmd<T1>(string filterId, Query<T1> extendingQuery, int firstResult, int maxResults) : base(filterId, extendingQuery)
	  {
		this.firstResult = firstResult;
		this.maxResults = maxResults;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<?> execute(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual IList<object> execute(CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.query.Query<?, ?> query = getFilterQuery(commandContext);
		Query<object, ?> query = getFilterQuery(commandContext);
		return query.listPage(firstResult, maxResults);
	  }

	}

}