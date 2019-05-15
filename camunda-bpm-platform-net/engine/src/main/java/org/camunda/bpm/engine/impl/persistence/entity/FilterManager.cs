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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.FILTER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using Filter = org.camunda.bpm.engine.filter.Filter;
	using StoredQueryValidator = org.camunda.bpm.engine.impl.QueryValidators.StoredQueryValidator;
	using FilterQueryImpl = org.camunda.bpm.engine.impl.filter.FilterQueryImpl;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterManager : AbstractManager
	{

	  public virtual Filter createNewFilter(string resourceType)
	  {
		checkAuthorization(CREATE, FILTER, ANY);
		return new FilterEntity(resourceType);
	  }

	  public virtual Filter insertOrUpdateFilter(Filter filter)
	  {

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.impl.AbstractQuery<?, ?> query = filter.getQuery();
		AbstractQuery<object, ?> query = filter.Query;
		query.validate(StoredQueryValidator.get());

		if (string.ReferenceEquals(filter.Id, null))
		{
		  checkAuthorization(CREATE, FILTER, ANY);
		  DbEntityManager.insert((FilterEntity) filter);
		  createDefaultAuthorizations(filter);
		}
		else
		{
		  checkAuthorization(UPDATE, FILTER, filter.Id);
		  DbEntityManager.merge((FilterEntity) filter);
		}

		return filter;
	  }

	  public virtual void deleteFilter(string filterId)
	  {
		checkAuthorization(DELETE, FILTER, filterId);

		FilterEntity filter = findFilterByIdInternal(filterId);
		ensureNotNull("No filter found for filter id '" + filterId + "'", "filter", filter);

		// delete all authorizations for this filter id
		deleteAuthorizations(FILTER, filterId);
		// delete the filter itself
		DbEntityManager.delete(filter);
	  }

	  public virtual FilterEntity findFilterById(string filterId)
	  {
		ensureNotNull("Invalid filter id", "filterId", filterId);
		checkAuthorization(READ, FILTER, filterId);
		return findFilterByIdInternal(filterId);
	  }

	  protected internal virtual FilterEntity findFilterByIdInternal(string filterId)
	  {
		return DbEntityManager.selectById(typeof(FilterEntity), filterId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.filter.Filter> findFiltersByQueryCriteria(org.camunda.bpm.engine.impl.filter.FilterQueryImpl filterQuery)
	  public virtual IList<Filter> findFiltersByQueryCriteria(FilterQueryImpl filterQuery)
	  {
		configureQuery(filterQuery, FILTER);
		return DbEntityManager.selectList("selectFilterByQueryCriteria", filterQuery);
	  }

	  public virtual long findFilterCountByQueryCriteria(FilterQueryImpl filterQuery)
	  {
		configureQuery(filterQuery, FILTER);
		return (long?) DbEntityManager.selectOne("selectFilterCountByQueryCriteria", filterQuery).Value;
	  }

	  // authorization utils /////////////////////////////////

	  protected internal virtual void createDefaultAuthorizations(Filter filter)
	  {
		if (AuthorizationEnabled)
		{
		  saveDefaultAuthorizations(ResourceAuthorizationProvider.newFilter(filter));
		}
	  }
	}

}