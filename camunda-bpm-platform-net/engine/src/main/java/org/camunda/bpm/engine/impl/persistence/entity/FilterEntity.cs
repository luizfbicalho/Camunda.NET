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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNull;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using StoredQueryValidator = org.camunda.bpm.engine.impl.QueryValidators.StoredQueryValidator;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using DbEntityLifecycleAware = org.camunda.bpm.engine.impl.db.DbEntityLifecycleAware;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using JsonObjectConverter = org.camunda.bpm.engine.impl.json.JsonObjectConverter;
	using JsonTaskQueryConverter = org.camunda.bpm.engine.impl.json.JsonTaskQueryConverter;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	[Serializable]
	public class FilterEntity : Filter, DbEntity, HasDbRevision, HasDbReferences, DbEntityLifecycleAware
	{

	  private const long serialVersionUID = 1L;
	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public final static java.util.Map<String, org.camunda.bpm.engine.impl.json.JsonObjectConverter<?>> queryConverter = new java.util.HashMap<String, org.camunda.bpm.engine.impl.json.JsonObjectConverter<?>>();
	  public static readonly IDictionary<string, JsonObjectConverter<object>> queryConverter = new Dictionary<string, JsonObjectConverter<object>>();

	  static FilterEntity()
	  {
		queryConverter[EntityTypes.TASK] = new JsonTaskQueryConverter();
	  }

	  protected internal string id;
	  protected internal string resourceType;
	  protected internal string name;
	  protected internal string owner;
	  protected internal AbstractQuery query;
	  protected internal IDictionary<string, object> properties;
	  protected internal int revision = 0;

	  protected internal FilterEntity()
	  {

	  }

	  public FilterEntity(string resourceType)
	  {
		ResourceType = resourceType;
		QueryInternal = "{}";
	  }

	  public virtual string Id
	  {
		  set
		  {
			this.id = value;
		  }
		  get
		  {
			return id;
		  }
	  }


	  public virtual Filter setResourceType(string resourceType)
	  {
		ensureNotEmpty(typeof(NotValidException), "Filter resource type must not be null or empty", "resourceType", resourceType);
		ensureNull(typeof(NotValidException), "Cannot overwrite filter resource type", "resourceType", this.resourceType);

		this.resourceType = resourceType;
		return this;
	  }

	  public virtual string ResourceType
	  {
		  get
		  {
			return resourceType;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual Filter setName(string name)
	  {
		ensureNotEmpty(typeof(NotValidException), "Filter name must not be null or empty", "name", name);
		this.name = name;
		return this;
	  }

	  public virtual string Owner
	  {
		  get
		  {
			return owner;
		  }
	  }

	  public virtual Filter setOwner(string owner)
	  {
		this.owner = owner;
		return this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.query.Query<?, ?>> T getQuery()
	  public virtual T getQuery<T>()
	  {
		return (T) query;
	  }

	  public virtual string QueryInternal
	  {
		  get
		  {
			JsonObjectConverter<object> converter = Converter;
			return converter.toJson(query);
		  }
		  set
		  {
			ensureNotNull(typeof(NotValidException), "query", value);
			JsonObjectConverter<object> converter = Converter;
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: this.query = (org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) converter.toObject(org.camunda.bpm.engine.impl.util.JsonUtil.asObject(value));
			this.query = (AbstractQuery<object, ?>) converter.toObject(JsonUtil.asObject(value));
		  }
	  }

	  public virtual Filter setQuery<T>(T query)
	  {
		ensureNotNull(typeof(NotValidException), "query", query);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: this.query = (org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query;
		this.query = (AbstractQuery<object, ?>) query;
		return this;
	  }


	  public virtual IDictionary<string, object> Properties
	  {
		  get
		  {
			if (properties != null)
			{
			  JsonObject json = JsonUtil.asObject(properties);
			  return JsonUtil.asMap(json);
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual string PropertiesInternal
	  {
		  get
		  {
			return JsonUtil.asString(properties);
		  }
		  set
		  {
			if (!string.ReferenceEquals(value, null))
			{
			  JsonObject json = JsonUtil.asObject(value);
			  this.properties = JsonUtil.asMap(json);
			}
			else
			{
			  this.properties = null;
			}
		  }
	  }

	  public virtual Filter setProperties(IDictionary<string, object> properties)
	  {
		this.properties = properties;
		return this;
	  }


	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }


	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.query.Query<?, ?>> org.camunda.bpm.engine.filter.Filter extend(T extendingQuery)
	  public virtual Filter extend<T>(T extendingQuery)
	  {
		ensureNotNull(typeof(NotValidException), "extendingQuery", extendingQuery);

		if (!extendingQuery.GetType().Equals(query.GetType()))
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw LOG.queryExtensionException(query.GetType().FullName, extendingQuery.GetType().FullName);
		}

		FilterEntity copy = copyFilter();
		copy.setQuery(query.extend(extendingQuery));

		return copy;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected <T> org.camunda.bpm.engine.impl.json.JsonObjectConverter<T> getConverter()
	  protected internal virtual JsonObjectConverter<T> getConverter<T>()
	  {
		  get
		  {
			JsonObjectConverter<T> converter = (JsonObjectConverter<T>) queryConverter[resourceType];
			if (converter != null)
			{
			  return converter;
			}
			else
			{
			  throw LOG.unsupportedResourceTypeException(resourceType);
			}
		  }
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["name"] = this.name;
			persistentState["owner"] = this.owner;
			persistentState["query"] = this.query;
			persistentState["properties"] = this.properties;
			return persistentState;
		  }
	  }

	  protected internal virtual FilterEntity copyFilter()
	  {
		FilterEntity copy = new FilterEntity(ResourceType);
		copy.Name = Name;
		copy.Owner = Owner;
		copy.QueryInternal = QueryInternal;
		copy.PropertiesInternal = PropertiesInternal;
		return copy;
	  }

	  public virtual void postLoad()
	  {
		if (query != null)
		{
		  query.addValidator(StoredQueryValidator.get());
		}

	  }

	  public virtual ISet<string> ReferencedEntityIds
	  {
		  get
		  {
			ISet<string> referencedEntityIds = new HashSet<string>();
			return referencedEntityIds;
		  }
	  }

	  public virtual IDictionary<string, Type> ReferencedEntitiesIdAndClass
	  {
		  get
		  {
			IDictionary<string, Type> referenceIdAndClass = new Dictionary<string, Type>();
			return referenceIdAndClass;
		  }
	  }
	}

}