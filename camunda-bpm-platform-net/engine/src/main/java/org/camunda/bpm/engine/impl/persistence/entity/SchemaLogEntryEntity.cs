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
	using SchemaLogEntry = org.camunda.bpm.engine.management.SchemaLogEntry;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	[Serializable]
	public class SchemaLogEntryEntity : SchemaLogEntry, DbEntity
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal DateTime timestamp;
	  protected internal string version;

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


	  // persistent object methods ////////////////////////////////////////////////

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


	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["id"] = this.id;
			persistentState["timestamp"] = this.timestamp;
			persistentState["version"] = this.version;
			return persistentState;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", timestamp=" + timestamp + ", version=" + version + "]";
	  }
	}

}