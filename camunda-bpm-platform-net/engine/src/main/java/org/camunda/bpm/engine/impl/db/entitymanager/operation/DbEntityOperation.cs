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
namespace org.camunda.bpm.engine.impl.db.entitymanager.operation
{

	using ClassNameUtil = org.camunda.bpm.engine.impl.util.ClassNameUtil;

	/// <summary>
	/// An operation on a single DbEntity
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DbEntityOperation : DbOperation
	{

	  /// <summary>
	  /// The entity the operation is performed on.
	  /// </summary>
	  protected internal DbEntity entity;

	  protected internal ISet<string> flushRelevantEntityReferences;

	  /// <summary>
	  /// Indicates whether the operation failed to execute due to OptimisticLocking
	  /// </summary>
	  protected internal bool failed = false;

	  public override void recycle()
	  {
		entity = null;
		base.recycle();
	  }

	  public virtual DbEntity Entity
	  {
		  get
		  {
			return entity;
		  }
		  set
		  {
			this.entityType = value.GetType();
			this.entity = value;
		  }
	  }


	  public virtual bool Failed
	  {
		  set
		  {
			this.failed = value;
		  }
		  get
		  {
			return failed;
		  }
	  }


	  public virtual ISet<string> FlushRelevantEntityReferences
	  {
		  set
		  {
			this.flushRelevantEntityReferences = value;
		  }
		  get
		  {
			return flushRelevantEntityReferences;
		  }
	  }


	  public override string ToString()
	  {
		return operationType + " " + ClassNameUtil.getClassNameWithoutPackage(entity) + "[" + entity.Id + "]";
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((entity == null) ? 0 : entity.GetHashCode());
		result = prime * result + ((operationType == null) ? 0 : operationType.GetHashCode());
		return result;
	  }

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		DbEntityOperation other = (DbEntityOperation) obj;
		if (entity == null)
		{
		  if (other.entity != null)
		  {
			return false;
		  }
		}
		else if (!entity.Equals(other.entity))
		{
		  return false;
		}
		if (operationType != other.operationType)
		{
		  return false;
		}
		return true;
	  }

	}

}