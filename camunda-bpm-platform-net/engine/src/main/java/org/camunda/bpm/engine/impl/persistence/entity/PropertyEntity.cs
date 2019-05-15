using System;

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

	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class PropertyEntity : DbEntity, HasDbRevision
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;
	  private const long serialVersionUID = 1L;

	  internal string name;
	  internal int revision;
	  internal string value;

	  public PropertyEntity()
	  {
	  }

	  public PropertyEntity(string name, string value)
	  {
		this.name = name;
		this.value = value;
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


	  public virtual string Value
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


	  // persistent object methods ////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			throw LOG.notAllowedIdException(value);
		  }
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			return value;
		  }
	  }


	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[name=" + name + ", revision=" + revision + ", value=" + value + "]";
	  }
	}

}