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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.DELETED_MERGED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.DELETED_PERSISTENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.DELETED_TRANSIENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.MERGED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.PERSISTENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState.TRANSIENT;




	/// <summary>
	/// A simple first level cache for <seealso cref="DbEntity Entities"/>.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DbEntityCache
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  /// <summary>
	  /// The cache itself: maps entity types (classes) to maps indexed by id (primary key).
	  /// 
	  /// The motivation for indexing by type (class) is
	  /// 
	  /// a) multiple entities of different types could have the same value as primary key. In the
	  ///    process engine, TaskEntity and HistoricTaskEntity have the same id value.
	  /// 
	  /// b) performance (?)
	  /// </summary>
	  protected internal IDictionary<Type, IDictionary<string, CachedDbEntity>> cachedEntites = new Dictionary<Type, IDictionary<string, CachedDbEntity>>();

	  protected internal DbEntityCacheKeyMapping cacheKeyMapping;

	  public DbEntityCache()
	  {
		this.cacheKeyMapping = DbEntityCacheKeyMapping.emptyMapping();
	  }

	  public DbEntityCache(DbEntityCacheKeyMapping cacheKeyMapping)
	  {
		this.cacheKeyMapping = cacheKeyMapping;
	  }

	  /// <summary>
	  /// get an object from the cache
	  /// </summary>
	  /// <param name="type"> the type of the object </param>
	  /// <param name="id"> the id of the object </param>
	  /// <returns> the object or 'null' if the object is not in the cache </returns>
	  /// <exception cref="ProcessEngineException"> if an object for the given id can be found but is of the wrong type. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.impl.db.DbEntity> T get(Class<T> type, String id)
	  public virtual T get<T>(Type type, string id) where T : org.camunda.bpm.engine.impl.db.DbEntity
	  {
			  type = typeof(T);
		Type cacheKey = cacheKeyMapping.getEntityCacheKey(type);
		CachedDbEntity cachedDbEntity = getCachedEntity(cacheKey, id);
		if (cachedDbEntity != null)
		{
		  DbEntity dbEntity = cachedDbEntity.Entity;
		  try
		  {
			return (T) dbEntity;
		  }
		  catch (System.InvalidCastException e)
		  {
			throw LOG.entityCacheLookupException(type, id, dbEntity.GetType(), e);
		  }
		}
		else
		{
		  return null;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.impl.db.DbEntity> java.util.List<T> getEntitiesByType(Class<T> type)
	  public virtual IList<T> getEntitiesByType<T>(Type type) where T : org.camunda.bpm.engine.impl.db.DbEntity
	  {
			  type = typeof(T);
		Type cacheKey = cacheKeyMapping.getEntityCacheKey(type);
		IDictionary<string, CachedDbEntity> entities = cachedEntites[cacheKey];
		IList<T> result = new List<T>();
		if (entities == null)
		{
		  return Collections.emptyList();
		}
		else
		{
		  foreach (CachedDbEntity cachedEntity in entities.Values)
		  {
			if (type != cacheKey)
			{
			  // if the cacheKey of this type differs from the actual type,
			  // not all cached entites with the key should be returned.
			  // Then we only add those entities whose type matches the argument type.
			  if (type.IsAssignableFrom(cachedEntity.GetType()))
			  {
				result.Add((T) cachedEntity.Entity);
			  }
			}
			else
			{
			  result.Add((T) cachedEntity.Entity);
			}

		  }
		  return result;
		}
	  }

	  /// <summary>
	  /// Looks up an entity in the cache.
	  /// </summary>
	  /// <param name="type"> the type of the object </param>
	  /// <param name="id"> the id of the CachedEntity to lookup </param>
	  /// <returns> the cached entity or null if the entity does not exist. </returns>
	  public virtual CachedDbEntity getCachedEntity(Type type, string id)
	  {
		Type cacheKey = cacheKeyMapping.getEntityCacheKey(type);
		IDictionary<string, CachedDbEntity> entitiesByType = cachedEntites[cacheKey];
		if (entitiesByType != null)
		{
		  return entitiesByType[id];
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Looks up an entity in the cache. </summary>
	  /// <param name="dbEntity"> the entity for which the CachedEntity should be looked up </param>
	  /// <returns> the cached entity or null if the entity does not exist. </returns>
	  public virtual CachedDbEntity getCachedEntity(DbEntity dbEntity)
	  {
		return getCachedEntity(dbEntity.GetType(), dbEntity.Id);
	  }

	  /// <summary>
	  /// Put a new, <seealso cref="DbEntityState#TRANSIENT"/> object into the cache.
	  /// </summary>
	  /// <param name="e"> the object to put into the cache </param>
	  public virtual void putTransient(DbEntity e)
	  {
		CachedDbEntity cachedDbEntity = new CachedDbEntity();
		cachedDbEntity.Entity = e;
		cachedDbEntity.EntityState = TRANSIENT;
		putInternal(cachedDbEntity);
	  }

	  /// <summary>
	  /// Put a <seealso cref="DbEntityState#PERSISTENT"/> object into the cache.
	  /// </summary>
	  /// <param name="e"> the object to put into the cache </param>
	  public virtual void putPersistent(DbEntity e)
	  {
		CachedDbEntity cachedDbEntity = new CachedDbEntity();
		cachedDbEntity.Entity = e;
		cachedDbEntity.EntityState = PERSISTENT;
		cachedDbEntity.determineEntityReferences();
		cachedDbEntity.makeCopy();

		putInternal(cachedDbEntity);
	  }

	  /// <summary>
	  /// Put a <seealso cref="DbEntityState#MERGED"/> object into the cache.
	  /// </summary>
	  /// <param name="e"> the object to put into the cache </param>
	  public virtual void putMerged(DbEntity e)
	  {
		CachedDbEntity cachedDbEntity = new CachedDbEntity();
		cachedDbEntity.Entity = e;
		cachedDbEntity.EntityState = MERGED;
		cachedDbEntity.determineEntityReferences();
		// no copy required

		putInternal(cachedDbEntity);
	  }

	  protected internal virtual void putInternal(CachedDbEntity entityToAdd)
	  {
		Type type = entityToAdd.Entity.GetType();
		Type cacheKey = cacheKeyMapping.getEntityCacheKey(type);

		IDictionary<string, CachedDbEntity> map = cachedEntites[cacheKey];
		if (map == null)
		{
		  map = new Dictionary<string, CachedDbEntity>();
		  cachedEntites[cacheKey] = map;
		}

		// check whether this object is already present in the cache
		CachedDbEntity existingCachedEntity = map[entityToAdd.Entity.Id];
		if (existingCachedEntity == null)
		{
		  // no such entity exists -> put it into the cache
		  map[entityToAdd.Entity.Id] = entityToAdd;

		}
		else
		{
		  // the same entity is already cached
		  switch (entityToAdd.EntityState)
		  {

		  case TRANSIENT:
			// cannot put TRANSIENT entity if entity with same id already exists in cache.
			if (existingCachedEntity.EntityState == TRANSIENT)
			{
			  throw LOG.entityCacheDuplicateEntryException("TRANSIENT", entityToAdd.Entity.Id, entityToAdd.Entity.GetType(), existingCachedEntity.EntityState);
			}
			else
			{
			  throw LOG.alreadyMarkedEntityInEntityCacheException(entityToAdd.Entity.Id, entityToAdd.Entity.GetType(), existingCachedEntity.EntityState);
			}

		  case PERSISTENT:
			if (existingCachedEntity.EntityState == PERSISTENT)
			{
			  // use new entity state, replacing the existing one.
			  map[entityToAdd.Entity.Id] = entityToAdd;
			  break;
			}
			if (existingCachedEntity.EntityState == DELETED_PERSISTENT || existingCachedEntity.EntityState == DELETED_MERGED)
			{
			  // ignore put -> this is already marked to be deleted
			  break;
			}

			// otherwise fail:
			throw LOG.entityCacheDuplicateEntryException("PERSISTENT", entityToAdd.Entity.Id, entityToAdd.Entity.GetType(), existingCachedEntity.EntityState);

		  case MERGED:
			if (existingCachedEntity.EntityState == PERSISTENT || existingCachedEntity.EntityState == MERGED)
			{
			  // use new entity state, replacing the existing one.
			  map[entityToAdd.Entity.Id] = entityToAdd;
			  break;
			}
			if (existingCachedEntity.EntityState == DELETED_PERSISTENT || existingCachedEntity.EntityState == DELETED_MERGED)
			{
			  // ignore put -> this is already marked to be deleted
			  break;
			}

			// otherwise fail:
			throw LOG.entityCacheDuplicateEntryException("MERGED", entityToAdd.Entity.Id, entityToAdd.Entity.GetType(), existingCachedEntity.EntityState);

		  default:
			// deletes are always added
			map[entityToAdd.Entity.Id] = entityToAdd;
			break;
		  }
		}
	  }

	  /// <summary>
	  /// Remove an entity from the cache </summary>
	  /// <param name="e"> the entity to remove
	  /// @return </param>
	  public virtual bool remove(DbEntity e)
	  {
		Type cacheKey = cacheKeyMapping.getEntityCacheKey(e.GetType());
		IDictionary<string, CachedDbEntity> typeMap = cachedEntites[cacheKey];
		if (typeMap != null)
		{
		  return typeMap.Remove(e.Id) != null;
		}
		else
		{
		  return false;
		}
	  }

	  /// <param name="cachedDbEntity"> </param>
	  public virtual void remove(CachedDbEntity cachedDbEntity)
	  {
		remove(cachedDbEntity.Entity);
	  }

	  /// <summary>
	  /// Allows checking whether the provided entity is present in the cache
	  /// </summary>
	  /// <param name="dbEntity"> the entity to check </param>
	  /// <returns> true if the the provided entity is present in the cache </returns>
	  public virtual bool contains(DbEntity dbEntity)
	  {
		return getCachedEntity(dbEntity) != null;
	  }

	  /// <summary>
	  /// Allows checking whether the provided entity is present in the cache
	  /// and is <seealso cref="DbEntityState#PERSISTENT"/>.
	  /// </summary>
	  /// <param name="dbEntity"> the entity to check </param>
	  /// <returns> true if the provided entity is present in the cache and is
	  /// <seealso cref="DbEntityState#PERSISTENT"/>. </returns>
	  public virtual bool isPersistent(DbEntity dbEntity)
	  {
		CachedDbEntity cachedDbEntity = getCachedEntity(dbEntity);
		if (cachedDbEntity == null)
		{
		  return false;
		}
		else
		{
		  return cachedDbEntity.EntityState == PERSISTENT;
		}
	  }

	  /// <summary>
	  /// Allows checking whether the provided entity is present in the cache
	  /// and is marked to be deleted.
	  /// </summary>
	  /// <param name="dbEntity"> the entity to check </param>
	  /// <returns> true if the provided entity is present in the cache and is
	  /// marked to be deleted </returns>
	  public virtual bool isDeleted(DbEntity dbEntity)
	  {
		CachedDbEntity cachedDbEntity = getCachedEntity(dbEntity);
		if (cachedDbEntity == null)
		{
		  return false;
		}
		else
		{
		  return cachedDbEntity.EntityState == DELETED_MERGED || cachedDbEntity.EntityState == DELETED_PERSISTENT || cachedDbEntity.EntityState == DELETED_TRANSIENT;
		}
	  }

	  /// <summary>
	  /// Allows checking whether the provided entity is present in the cache
	  /// and is <seealso cref="DbEntityState#TRANSIENT"/>.
	  /// </summary>
	  /// <param name="dbEntity"> the entity to check </param>
	  /// <returns> true if the provided entity is present in the cache and is
	  /// <seealso cref="DbEntityState#TRANSIENT"/>. </returns>
	  public virtual bool isTransient(DbEntity dbEntity)
	  {
		CachedDbEntity cachedDbEntity = getCachedEntity(dbEntity);
		if (cachedDbEntity == null)
		{
		  return false;
		}
		else
		{
		  return cachedDbEntity.EntityState == TRANSIENT;
		}
	  }

	  public virtual IList<CachedDbEntity> CachedEntities
	  {
		  get
		  {
			IList<CachedDbEntity> result = new List<CachedDbEntity>();
			foreach (IDictionary<string, CachedDbEntity> typeCache in cachedEntites.Values)
			{
			  ((IList<CachedDbEntity>)result).AddRange(typeCache.Values);
			}
			return result;
		  }
	  }

	  /// <summary>
	  /// Sets an object to a deleted state. It will not be removed from the cache but
	  /// transition to one of the DELETED states, depending on it's current state.
	  /// </summary>
	  /// <param name="dbEntity"> the object to mark deleted. </param>
	  public virtual DbEntity Deleted
	  {
		  set
		  {
			CachedDbEntity cachedEntity = getCachedEntity(value);
			if (cachedEntity != null)
			{
			  if (cachedEntity.EntityState == TRANSIENT)
			  {
				cachedEntity.EntityState = DELETED_TRANSIENT;
    
			  }
			  else if (cachedEntity.EntityState == PERSISTENT)
			  {
				cachedEntity.EntityState = DELETED_PERSISTENT;
    
			  }
			  else if (cachedEntity.EntityState == MERGED)
			  {
				cachedEntity.EntityState = DELETED_MERGED;
			  }
			}
			else
			{
			  // put a deleted merged into the cache
			  CachedDbEntity cachedDbEntity = new CachedDbEntity();
			  cachedDbEntity.Entity = value;
			  cachedDbEntity.EntityState = DELETED_MERGED;
			  putInternal(cachedDbEntity);
    
			}
		  }
	  }

	  public virtual void undoDelete(DbEntity dbEntity)
	  {
		CachedDbEntity cachedEntity = getCachedEntity(dbEntity);
		if (cachedEntity.EntityState == DbEntityState.DELETED_TRANSIENT)
		{
		  cachedEntity.EntityState = DbEntityState.TRANSIENT;
		}
		else
		{
		  cachedEntity.EntityState = DbEntityState.MERGED;
		}
	  }

	}

}