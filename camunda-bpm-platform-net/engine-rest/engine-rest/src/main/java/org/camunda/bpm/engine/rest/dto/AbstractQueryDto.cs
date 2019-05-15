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
namespace org.camunda.bpm.engine.rest.dto
{

	using Direction = org.camunda.bpm.engine.impl.Direction;
	using Query = org.camunda.bpm.engine.query.Query;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;


	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// Defines common query operations, such as sorting options and validation.
	/// Also allows to access its setter methods based on <seealso cref="CamundaQueryParam"/> annotations which is
	/// used for processing Http query parameters.
	/// 
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public abstract class AbstractQueryDto<T> : AbstractSearchQueryDto
	{

	  public const string SORT_ORDER_ASC_VALUE = "asc";
	  public const string SORT_ORDER_DESC_VALUE = "desc";

	  public static readonly IList<string> VALID_SORT_ORDER_VALUES;
	  static AbstractQueryDto()
	  {
		VALID_SORT_ORDER_VALUES = new List<string>();
		VALID_SORT_ORDER_VALUES.Add(SORT_ORDER_ASC_VALUE);
		VALID_SORT_ORDER_VALUES.Add(SORT_ORDER_DESC_VALUE);
	  }

	  protected internal string sortBy;
	  protected internal string sortOrder;

	  protected internal IList<SortingDto> sortings;

	  protected internal IDictionary<string, string> expressions = new Dictionary<string, string>();

	  // required for populating via jackson
	  public AbstractQueryDto()
	  {

	  }

	  public AbstractQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }


	  [CamundaQueryParam("sortBy")]
	  public virtual string SortBy
	  {
		  set
		  {
			if (!isValidSortByValue(value))
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "sortBy parameter has invalid value: " + value);
			}
			this.sortBy = value;
		  }
	  }

	  [CamundaQueryParam("sortOrder")]
	  public virtual string SortOrder
	  {
		  set
		  {
			if (!VALID_SORT_ORDER_VALUES.Contains(value))
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "sortOrder parameter has invalid value: " + value);
			}
			this.sortOrder = value;
		  }
	  }

	  public virtual IList<SortingDto> Sorting
	  {
		  set
		  {
			this.sortings = value;
		  }
		  get
		  {
			return sortings;
		  }
	  }


	  protected internal abstract bool isValidSortByValue(string value);

	  protected internal virtual bool sortOptionsValid()
	  {
		return (!string.ReferenceEquals(sortBy, null) && !string.ReferenceEquals(sortOrder, null)) || (string.ReferenceEquals(sortBy, null) && string.ReferenceEquals(sortOrder, null));
	  }

	  public virtual T toQuery(ProcessEngine engine)
	  {
		T query = createNewQuery(engine);
		applyFilters(query);

		if (!sortOptionsValid())
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Only a single sorting parameter specified. sortBy and sortOrder required");
		}

		applySortingOptions(query, engine);

		return query;
	  }

	  protected internal abstract T createNewQuery(ProcessEngine engine);

	  protected internal abstract void applyFilters(T query);

	  protected internal virtual void applySortingOptions(T query, ProcessEngine engine)
	  {
		if (!string.ReferenceEquals(sortBy, null))
		{
		  applySortBy(query, sortBy, null, engine);
		}
		if (!string.ReferenceEquals(sortOrder, null))
		{
		  applySortOrder(query, sortOrder);
		}

		if (sortings != null)
		{
		  foreach (SortingDto sorting in sortings)
		  {
			string sortingOrder = sorting.SortOrder;
			string sortingBy = sorting.SortBy;

			if (!string.ReferenceEquals(sortingBy, null))
			{
			  applySortBy(query, sortingBy, sorting.Parameters, engine);
			}
			if (!string.ReferenceEquals(sortingOrder, null))
			{
			  applySortOrder(query, sortingOrder);
			}
		  }
		}
	  }

	  protected internal abstract void applySortBy(T query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine);

	  protected internal virtual void applySortOrder(T query, string sortOrder)
	  {
		if (!string.ReferenceEquals(sortOrder, null))
		{
		  if (sortOrder.Equals(SORT_ORDER_ASC_VALUE))
		  {
			query.asc();
		  }
		  else if (sortOrder.Equals(SORT_ORDER_DESC_VALUE))
		  {
			query.desc();
		  }
		}
	  }

	  public static string sortOrderValueForDirection(Direction direction)
	  {
		if (Direction.ASCENDING.Equals(direction))
		{
		  return SORT_ORDER_ASC_VALUE;
		}
		else if (Direction.DESCENDING.Equals(direction))
		{
		  return SORT_ORDER_DESC_VALUE;
		}
		else
		{
		  throw new RestException("Unknown query sorting direction " + direction);
		}
	  }

	}

}