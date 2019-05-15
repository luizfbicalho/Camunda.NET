using System;

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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using Filter = org.camunda.bpm.engine.filter.Filter;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using FilterEntity = org.camunda.bpm.engine.impl.persistence.entity.FilterEntity;
	using Query = org.camunda.bpm.engine.query.Query;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	[Serializable]
	public abstract class AbstractExecuteFilterCmd
	{

	  private const long serialVersionUID = 1L;

	  protected internal string filterId;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.query.Query<?, ?> extendingQuery;
	  protected internal Query<object, ?> extendingQuery;

	  public AbstractExecuteFilterCmd(string filterId)
	  {
		this.filterId = filterId;
	  }

	  public AbstractExecuteFilterCmd<T1>(string filterId, Query<T1> extendingQuery)
	  {
		this.filterId = filterId;
		this.extendingQuery = extendingQuery;
	  }

	  protected internal virtual Filter getFilter(CommandContext commandContext)
	  {
		ensureNotNull("No filter id given to execute", "filterId", filterId);
		FilterEntity filter = commandContext.FilterManager.findFilterById(filterId);

		ensureNotNull("No filter found for id '" + filterId + "'", "filter", filter);

		if (extendingQuery != null)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: ((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) extendingQuery).validate();
		  ((AbstractQuery<object, ?>) extendingQuery).validate();
		  filter = (FilterEntity) filter.extend(extendingQuery);
		}

		return filter;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.query.Query<?, ?> getFilterQuery(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  protected internal virtual Query<object, ?> getFilterQuery(CommandContext commandContext)
	  {
		Filter filter = getFilter(commandContext);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.query.Query<?, ?> query = filter.getQuery();
		Query<object, ?> query = filter.Query;
		if (query is TaskQuery)
		{
		  ((TaskQuery) query).initializeFormKeys();
		}
		return query;
	  }

	}

}