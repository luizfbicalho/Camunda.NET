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

	using Filter = org.camunda.bpm.engine.filter.Filter;
	using FilterQuery = org.camunda.bpm.engine.filter.FilterQuery;
	using CreateFilterCmd = org.camunda.bpm.engine.impl.cmd.CreateFilterCmd;
	using DeleteFilterCmd = org.camunda.bpm.engine.impl.cmd.DeleteFilterCmd;
	using ExecuteFilterCountCmd = org.camunda.bpm.engine.impl.cmd.ExecuteFilterCountCmd;
	using ExecuteFilterListCmd = org.camunda.bpm.engine.impl.cmd.ExecuteFilterListCmd;
	using ExecuteFilterListPageCmd = org.camunda.bpm.engine.impl.cmd.ExecuteFilterListPageCmd;
	using ExecuteFilterSingleResultCmd = org.camunda.bpm.engine.impl.cmd.ExecuteFilterSingleResultCmd;
	using GetFilterCmd = org.camunda.bpm.engine.impl.cmd.GetFilterCmd;
	using SaveFilterCmd = org.camunda.bpm.engine.impl.cmd.SaveFilterCmd;
	using FilterQueryImpl = org.camunda.bpm.engine.impl.filter.FilterQueryImpl;
	using Query = org.camunda.bpm.engine.query.Query;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterServiceImpl : ServiceImpl, FilterService
	{

	  public virtual Filter newTaskFilter()
	  {
		return commandExecutor.execute(new CreateFilterCmd(EntityTypes.TASK));
	  }

	  public virtual Filter newTaskFilter(string filterName)
	  {
		return newTaskFilter().setName(filterName);
	  }

	  public virtual FilterQuery createFilterQuery()
	  {
		return new FilterQueryImpl(commandExecutor);
	  }

	  public virtual FilterQuery createTaskFilterQuery()
	  {
		return (new FilterQueryImpl(commandExecutor)).filterResourceType(EntityTypes.TASK);
	  }

	  public virtual Filter saveFilter(Filter filter)
	  {
		return commandExecutor.execute(new SaveFilterCmd(filter));
	  }

	  public virtual Filter getFilter(string filterId)
	  {
		return commandExecutor.execute(new GetFilterCmd(filterId));
	  }

	  public virtual void deleteFilter(string filterId)
	  {
		commandExecutor.execute(new DeleteFilterCmd(filterId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> java.util.List<T> list(String filterId)
	  public virtual IList<T> list<T>(string filterId)
	  {
		return (IList<T>) commandExecutor.execute(new ExecuteFilterListCmd(filterId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T, Q extends org.camunda.bpm.engine.query.Query<?, T>> java.util.List<T> list(String filterId, Q extendingQuery)
	  public virtual IList<T> list<T, Q>(string filterId, Q extendingQuery)
	  {
		return (IList<T>) commandExecutor.execute(new ExecuteFilterListCmd(filterId, extendingQuery));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> java.util.List<T> listPage(String filterId, int firstResult, int maxResults)
	  public virtual IList<T> listPage<T>(string filterId, int firstResult, int maxResults)
	  {
		return (IList<T>) commandExecutor.execute(new ExecuteFilterListPageCmd(filterId, firstResult, maxResults));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T, Q extends org.camunda.bpm.engine.query.Query<?, T>> java.util.List<T> listPage(String filterId, Q extendingQuery, int firstResult, int maxResults)
	  public virtual IList<T> listPage<T, Q>(string filterId, Q extendingQuery, int firstResult, int maxResults)
	  {
		return (IList<T>) commandExecutor.execute(new ExecuteFilterListPageCmd(filterId, extendingQuery, firstResult, maxResults));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T singleResult(String filterId)
	  public virtual T singleResult<T>(string filterId)
	  {
		return (T) commandExecutor.execute(new ExecuteFilterSingleResultCmd(filterId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T, Q extends org.camunda.bpm.engine.query.Query<?, T>> T singleResult(String filterId, Q extendingQuery)
	  public virtual T singleResult<T, Q>(string filterId, Q extendingQuery)
	  {
		return (T) commandExecutor.execute(new ExecuteFilterSingleResultCmd(filterId, extendingQuery));
	  }

	  public virtual long? count(string filterId)
	  {
		return commandExecutor.execute(new ExecuteFilterCountCmd(filterId));
	  }

	  public virtual long? count<T1>(string filterId, Query<T1> extendingQuery)
	  {
		return commandExecutor.execute(new ExecuteFilterCountCmd(filterId, extendingQuery));
	  }

	}

}