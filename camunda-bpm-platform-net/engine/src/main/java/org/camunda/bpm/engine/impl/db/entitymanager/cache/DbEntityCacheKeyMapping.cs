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
namespace org.camunda.bpm.engine.impl.db.entitymanager.cache
{

	using HistoricDetailEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDetailEventEntity;
	using HistoricFormPropertyEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricFormPropertyEventEntity;
	using HistoricVariableUpdateEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricVariableUpdateEventEntity;
	using HistoricDetailVariableInstanceUpdateEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricDetailVariableInstanceUpdateEntity;
	using HistoricFormPropertyEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricFormPropertyEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;

	/// <summary>
	/// Provides the keys used by <seealso cref="DbEntityCache"/> for organizing the different <seealso cref="DbEntity"/> types.
	/// Especially for polymorphic types, it is important that they are accessible in the cache under one
	/// common key such that querying the cache with a superclass or with a subclass both return the cached
	/// entities.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class DbEntityCacheKeyMapping
	{

	  protected internal IDictionary<Type, Type> entityCacheKeys;

	  public DbEntityCacheKeyMapping()
	  {
		this.entityCacheKeys = new Dictionary<Type, Type>();
	  }

	  public virtual Type getEntityCacheKey(Type entityType)
	  {
		Type entityCacheKey = entityCacheKeys[entityType];
		if (entityCacheKey == null)
		{
		  return entityType;
		}

		return entityCacheKey;
	  }

	  public virtual void registerEntityCacheKey(Type entityType, Type cacheKey)
	  {
		this.entityCacheKeys[entityType] = cacheKey;
	  }

	  public static DbEntityCacheKeyMapping defaultEntityCacheKeyMapping()
	  {
		DbEntityCacheKeyMapping mapping = new DbEntityCacheKeyMapping();

		// subclasses of JobEntity
		mapping.registerEntityCacheKey(typeof(MessageEntity), typeof(JobEntity));
		mapping.registerEntityCacheKey(typeof(TimerEntity), typeof(JobEntity));

		// subclasses of HistoricDetailEventEntity
		mapping.registerEntityCacheKey(typeof(HistoricFormPropertyEntity), typeof(HistoricDetailEventEntity));
		mapping.registerEntityCacheKey(typeof(HistoricFormPropertyEventEntity), typeof(HistoricDetailEventEntity));
		mapping.registerEntityCacheKey(typeof(HistoricVariableUpdateEventEntity), typeof(HistoricDetailEventEntity));
		mapping.registerEntityCacheKey(typeof(HistoricDetailVariableInstanceUpdateEntity), typeof(HistoricDetailEventEntity));

		return mapping;
	  }

	  public static DbEntityCacheKeyMapping emptyMapping()
	  {
		return new DbEntityCacheKeyMapping();
	  }
	}

}