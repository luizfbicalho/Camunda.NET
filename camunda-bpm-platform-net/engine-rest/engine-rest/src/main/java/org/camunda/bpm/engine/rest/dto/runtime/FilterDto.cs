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
namespace org.camunda.bpm.engine.rest.dto.runtime
{

	using Filter = org.camunda.bpm.engine.filter.Filter;
	using TaskQueryDto = org.camunda.bpm.engine.rest.dto.task.TaskQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	using JsonInclude = com.fasterxml.jackson.annotation.JsonInclude;
	using Include = com.fasterxml.jackson.annotation.JsonInclude.Include;
	using JsonSubTypes = com.fasterxml.jackson.annotation.JsonSubTypes;
	using JsonTypeInfo = com.fasterxml.jackson.annotation.JsonTypeInfo;

	public class FilterDto
	{

	  protected internal string id;
	  protected internal string resourceType;
	  protected internal string name;
	  protected internal string owner;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.rest.dto.AbstractQueryDto<?> query;
	  protected internal AbstractQueryDto<object> query;
	  protected internal IDictionary<string, object> properties;

	  protected internal long? itemCount;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual string ResourceType
	  {
		  get
		  {
			return resourceType;
		  }
		  set
		  {
			this.resourceType = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual string Owner
	  {
		  get
		  {
			return owner;
		  }
		  set
		  {
			this.owner = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.rest.dto.AbstractQueryDto<?> getQuery()
	  public virtual AbstractQueryDto<object> Query
	  {
		  get
		  {
			return query;
		  }
		  set
		  {
			this.query = value;
		  }
	  }


	  public virtual IDictionary<string, object> Properties
	  {
		  get
		  {
			return properties;
		  }
		  set
		  {
			this.properties = value;
		  }
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonInclude(com.fasterxml.jackson.annotation.JsonInclude.Include.NON_NULL) public System.Nullable<long> getItemCount()
	  public virtual long? ItemCount
	  {
		  get
		  {
			return itemCount;
		  }
		  set
		  {
			this.itemCount = value;
		  }
	  }


	  public static FilterDto fromFilter(Filter filter)
	  {
		FilterDto dto = new FilterDto();
		dto.id = filter.Id;
		dto.resourceType = filter.ResourceType;
		dto.name = filter.Name;
		dto.owner = filter.Owner;

		if (EntityTypes.TASK.Equals(filter.ResourceType))
		{
		  dto.query = TaskQueryDto.fromQuery(filter.Query);
		}

		dto.properties = filter.Properties;
		return dto;
	  }

	  public virtual void updateFilter(Filter filter, ProcessEngine engine)
	  {
		if (!string.ReferenceEquals(ResourceType, null) && !ResourceType.Equals(filter.ResourceType))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Unable to update filter from resource type '" + filter.ResourceType + "' to '" + ResourceType + "'");
		}
		filter.Name = Name;
		filter.Owner = Owner;
		filter.Query = query.toQuery(engine);
		filter.Properties = Properties;
	  }

	}

}