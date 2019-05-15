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

	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class MeterLogEntity : DbEntity, HasDbReferences
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;

	  protected internal DateTime timestamp;
	  protected internal long? milliseconds;

	  protected internal string name;

	  protected internal string reporter;

	  protected internal long value;

	  public MeterLogEntity(string name, long value, DateTime timestamp) : this(name, null, value, timestamp)
	  {
	  }

	  public MeterLogEntity(string name, string reporter, long value, DateTime timestamp)
	  {
		this.name = name;
		this.reporter = reporter;
		this.value = value;
		this.timestamp = timestamp;
		this.milliseconds = timestamp.Ticks;
	  }

	  public MeterLogEntity()
	  {
	  }

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


	  public virtual DateTime Timestamp
	  {
		  get
		  {
			return timestamp;
		  }
		  set
		  {
			this.timestamp = value;
		  }
	  }


	  public virtual long? Milliseconds
	  {
		  get
		  {
			return milliseconds;
		  }
		  set
		  {
			this.milliseconds = value;
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


	  public virtual long Value
	  {
		  get
		  {
			return value;
		  }
		  set
		  {
			this.value = value;
		  }
	  }


	  public virtual string Reporter
	  {
		  get
		  {
			return reporter;
		  }
		  set
		  {
			this.reporter = value;
		  }
	  }


	  public virtual object PersistentState
	  {
		  get
		  {
			// immutable
			return typeof(MeterLogEntity);
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