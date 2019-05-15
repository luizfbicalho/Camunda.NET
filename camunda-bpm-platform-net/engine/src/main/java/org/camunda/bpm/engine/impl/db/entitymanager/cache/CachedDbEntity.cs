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


	/// <summary>
	/// A cached entity
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class CachedDbEntity : Recyclable
	{

	  protected internal DbEntity dbEntity;

	  protected internal object copy;

	  protected internal DbEntityState entityState;

	  /// <summary>
	  /// Ids of referenced entities of the same entity type
	  /// </summary>
	  protected internal ISet<string> flushRelevantEntityReferences = null;

	  public virtual void recycle()
	  {
		// clean out state
		dbEntity = null;
		copy = null;
		entityState = null;
	  }

	  /// <summary>
	  /// Allows checking whether this entity is dirty. </summary>
	  /// <returns> true if the entity is dirty (state has changed since it was put into the cache) </returns>
	  public virtual bool Dirty
	  {
		  get
		  {
			return !dbEntity.PersistentState.Equals(copy);
		  }
	  }

	  public virtual void forceSetDirty()
	  {
		// set the value of the copy to some value which will always be different from the new entity state.
		this.copy = -1;
	  }

	  public virtual void makeCopy()
	  {
		copy = dbEntity.PersistentState;
	  }

	  public override string ToString()
	  {
		return entityState + " " + dbEntity.GetType().Name + "[" + dbEntity.Id + "]";
	  }

	  public virtual void determineEntityReferences()
	  {
		if (dbEntity is HasDbReferences)
		{
		  flushRelevantEntityReferences = ((HasDbReferences) dbEntity).ReferencedEntityIds;
		}
		else
		{
		  flushRelevantEntityReferences = Collections.emptySet();
		}
	  }

	  public virtual bool areFlushRelevantReferencesDetermined()
	  {
		return flushRelevantEntityReferences != null;
	  }

	  public virtual ISet<string> FlushRelevantEntityReferences
	  {
		  get
		  {
			return flushRelevantEntityReferences;
		  }
	  }

	  // getters / setters ////////////////////////////

	  public virtual DbEntity Entity
	  {
		  get
		  {
			return dbEntity;
		  }
		  set
		  {
			this.dbEntity = value;
		  }
	  }


	  public virtual DbEntityState EntityState
	  {
		  get
		  {
			return entityState;
		  }
		  set
		  {
			this.entityState = value;
		  }
	  }


	  public virtual Type EntityType
	  {
		  get
		  {
			return dbEntity.GetType();
		  }
	  }

	}

}