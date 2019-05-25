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
namespace org.camunda.bpm.engine.impl.filter
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using Filter = org.camunda.bpm.engine.filter.Filter;
	using FilterQuery = org.camunda.bpm.engine.filter.FilterQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	[Serializable]
	public class FilterQueryImpl : AbstractQuery<FilterQuery, Filter>, FilterQuery
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string filterId_Conflict;
	  protected internal string resourceType;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string owner;

	  public FilterQueryImpl()
	  {
	  }

	  public FilterQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual FilterQuery filterId(string filterId)
	  {
		ensureNotNull("filterId", filterId);
		this.filterId_Conflict = filterId;
		return this;
	  }

	  public virtual FilterQuery filterResourceType(string resourceType)
	  {
		ensureNotNull("resourceType", resourceType);
		this.resourceType = resourceType;
		return this;
	  }

	  public virtual FilterQuery filterName(string name)
	  {
		ensureNotNull("name", name);
		this.name = name;
		return this;
	  }

	  public virtual FilterQuery filterNameLike(string nameLike)
	  {
		ensureNotNull("nameLike", nameLike);
		this.nameLike = nameLike;
		return this;
	  }

	  public virtual FilterQuery filterOwner(string owner)
	  {
		ensureNotNull("owner", owner);
		this.owner = owner;
		return this;
	  }

	  public virtual FilterQuery orderByFilterId()
	  {
		return orderBy(FilterQueryProperty_Fields.FILTER_ID);
	  }

	  public virtual FilterQuery orderByFilterResourceType()
	  {
		return orderBy(FilterQueryProperty_Fields.RESOURCE_TYPE);
	  }

	  public virtual FilterQuery orderByFilterName()
	  {
		return orderBy(FilterQueryProperty_Fields.NAME);
	  }

	  public virtual FilterQuery orderByFilterOwner()
	  {
		return orderBy(FilterQueryProperty_Fields.OWNER);
	  }

	  public override IList<Filter> executeList(CommandContext commandContext, Page page)
	  {
		return commandContext.FilterManager.findFiltersByQueryCriteria(this);
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		return commandContext.FilterManager.findFilterCountByQueryCriteria(this);
	  }

	}

}