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
namespace org.camunda.bpm.engine.impl.db.entitymanager.operation
{

	/// <summary>
	/// A database operation.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class DbOperation : Recyclable
	{

	  /// <summary>
	  /// The type of the operation.
	  /// </summary>
	  protected internal DbOperationType operationType;

	  protected internal int rowsAffected;

	  /// <summary>
	  /// The type of the DbEntity this operation is executed on.
	  /// </summary>
	  protected internal Type entityType;

	  public virtual void recycle()
	  {
		// clean out the object state
		operationType = null;
		entityType = null;
	  }

	  // getters / setters //////////////////////////////////////////

	  public abstract bool Failed {get;}

	  public virtual Type EntityType
	  {
		  get
		  {
			return entityType;
		  }
		  set
		  {
			this.entityType = value;
		  }
	  }


	  public virtual DbOperationType OperationType
	  {
		  get
		  {
			return operationType;
		  }
		  set
		  {
			this.operationType = value;
		  }
	  }


	  public virtual int RowsAffected
	  {
		  get
		  {
			return rowsAffected;
		  }
		  set
		  {
			this.rowsAffected = value;
		  }
	  }


	}

}