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

	using SchemaLogQuery = org.camunda.bpm.engine.management.SchemaLogQuery;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	public class SchemaLogQueryDto : AbstractQueryDto<SchemaLogQuery>
	{

	  private const string SORT_BY_TIMESTAMP_VALUE = "timestamp";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static SchemaLogQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_TIMESTAMP_VALUE);
	  }

	  internal string version;

	  public SchemaLogQueryDto()
	  {
	  }

	  public SchemaLogQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  public virtual string Version
	  {
		  get
		  {
			return version;
		  }
		  set
		  {
			this.version = value;
		  }
	  }


	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override SchemaLogQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.ManagementService.createSchemaLogQuery();
	  }

	  protected internal override void applyFilters(SchemaLogQuery query)
	  {
		if (!string.ReferenceEquals(this.version, null))
		{
		  query.version(this.version);
		}
	  }

	  protected internal override void applySortBy(SchemaLogQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_TIMESTAMP_VALUE))
		{
		  query.orderByTimestamp();
		}
	  }
	}

}